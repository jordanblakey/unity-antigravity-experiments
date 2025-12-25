using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Antigravity.Editor
{
    /// <summary>
    /// Antigravity Console v2.3
    /// Maximized performance, clean architecture, and robust stability.
    /// </summary>
    public class AntigravityConsole : EditorWindow
    {
        [MenuItem("Antigravity/Editor Logs %#i", false, 10)]
        public static void Open() => GetWindow<AntigravityConsole>("Editor Logs");

        // --- Data Models ---

        public class LogEntry
        {
            public int Id;
            public string RawHeader;
            public string RawBody;
            public string Timestamp;
            public LogType Type;

            public bool IsExpanded;
            public float LastCachedHeight = -1;
            public float LastHeaderHeight = -1;

            // Rich text displays
            public string DisplayHeader;
            public string DisplayBody;

            public void ProcessVisuals(Regex fileLineRegex)
            {
                // Mute file/line info
                string header = fileLineRegex.Replace(RawHeader, m => $"<color=#888888>{m.Value}</color>");
                // Mute timestamp
                DisplayHeader = $"<color=#888888>[{Timestamp}]</color> {header}";
                DisplayBody = RawBody?.Trim();
            }

            public string GetFullText()
            {
                if (string.IsNullOrEmpty(RawBody)) return $"[{Timestamp}] {RawHeader}";
                return $"[{Timestamp}] {RawHeader}\n{RawBody}";
            }
        }

        private class CircularBuffer<T>
        {
            private readonly T[] _buffer;
            private int _head;
            private int _count;
            private readonly int _capacity;

            public CircularBuffer(int capacity)
            {
                _capacity = capacity;
                _buffer = new T[capacity];
                _head = 0;
                _count = 0;
            }

            public void Add(T item)
            {
                if (_count < _capacity)
                {
                    _buffer[_count] = item;
                    _count++;
                }
                else
                {
                    _buffer[_head] = item;
                    _head = (_head + 1) % _capacity;
                }
            }

            public void Clear()
            {
                _head = 0;
                _count = 0;
            }

            public int Count => _count;

            public T this[int index]
            {
                get
                {
                    if (index < 0 || index >= _count) throw new IndexOutOfRangeException();
                    // If buffer is full, physical index is (head + logical_index) % capacity
                    // If not full, physical index is just logical_index
                    int physicalIndex = (_count < _capacity) ? index : (_head + index) % _capacity;
                    return _buffer[physicalIndex];
                }
            }

            public IEnumerable<T> Iterate()
            {
                for (int i = 0; i < _count; i++) yield return this[i];
            }
        }

        // --- Constants & Regex ---
        private const int MAX_ENTRIES = 1000;
        private static readonly Regex _ansiRegex = new Regex(@"\x1B\[[0-9;]*[mK]");
        private static readonly Regex _timestampRegex = new Regex(@"^\[?\d{2}:\d{2}:\d{2}(\.\d{3,4})?\]?");
        private static readonly Regex _fileLineRegex = new Regex(@" \((at |Filename: ).*?\) *$");

        // --- State ---
        private CircularBuffer<LogEntry> _masterBuffer = new CircularBuffer<LogEntry>(MAX_ENTRIES);
        private List<LogEntry> _filteredEntries = new List<LogEntry>(MAX_ENTRIES);

        private string _logPath;
        private long _lastFilePosition = 0;
        private string _leftoverText = "";
        private int _idCounter = 0;
        private int _focusedEntryId = -1;

        private Vector2 _scrollPos;
        private string _searchFilter = "";
        private bool _autoScroll = true;
        private bool _expandAll = false;

        private GUIStyle _headerStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _bodyStyle;

        // --- Lifecycle ---

        private void OnEnable()
        {
            // Set Title & Icon
            titleContent = new GUIContent("Editor Logs", EditorGUIUtility.IconContent("d_Profiler.GlobalIllumination@2x").image);

            ResetInternalState(true); // True = full reset including file position

            string home = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _logPath = Path.Combine(home, ".config/unity3d/Editor.log");

            if (File.Exists(_logPath)) BackfillLogs();
            RefreshFilters();
        }

        private void ResetInternalState(bool resetFilePosition)
        {
            _masterBuffer.Clear();
            _filteredEntries.Clear();
            _leftoverText = "";
            _idCounter = 0;
            _focusedEntryId = -1;

            // Clear styles to prevent stale "foldout" data across reloads
            _headerStyle = null;
            _labelStyle = null;
            _bodyStyle = null;

            if (resetFilePosition)
            {
                _lastFilePosition = 0;
            }
            else
            {
                // "Clear Logs" -> Ignore history, fast forward
                if (File.Exists(_logPath))
                {
                    try { _lastFilePosition = new FileInfo(_logPath).Length; }
                    catch { _lastFilePosition = 0; }
                }
            }
        }

        private void Update()
        {
            if (string.IsNullOrEmpty(_logPath) || !File.Exists(_logPath)) return;

            // Safe IO Block
            try
            {
                using (var fs = new FileStream(_logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    long len = fs.Length;
                    if (len < _lastFilePosition)
                    {
                        // File shrunk (log rotation?) -> Reset to start
                        _lastFilePosition = 0;
                    }

                    if (len > _lastFilePosition)
                    {
                        fs.Seek(_lastFilePosition, SeekOrigin.Begin);
                        using (var reader = new StreamReader(fs))
                        {
                            string chunk = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(chunk)) ProcessChunk(chunk);
                        }
                        _lastFilePosition = len;
                    }
                }
            }
            catch (Exception)
            {
                // Swallow async IO errors (file locks, disposal races)
                // This prevents editor spam if Unity locks the log briefly
            }
        }

        // --- IO & Parsing ---

        private void BackfillLogs()
        {
            _masterBuffer.Clear();
            if (!File.Exists(_logPath)) return;

            try
            {
                using (var fs = new FileStream(_logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    long len = fs.Length;
                    long startPos = Math.Max(0, len - 512000); // 512KB tail

                    fs.Seek(startPos, SeekOrigin.Begin);
                    using (var reader = new StreamReader(fs))
                    {
                        if (startPos > 0) reader.ReadLine(); // Discard partial
                        ProcessChunk(reader.ReadToEnd());
                    }
                    _lastFilePosition = len;
                }
            }
            catch { }
            RefreshFilters();
        }

        private void ProcessChunk(string chunk)
        {
            string content = _leftoverText + chunk;
            string[] lines = content.Split('\n');
            _leftoverText = lines[lines.Length - 1];

            bool changed = false;
            for (int i = 0; i < lines.Length - 1; i++)
            {
                string line = _ansiRegex.Replace(lines[i].TrimEnd('\r'), "");
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (IsHeader(line))
                {
                    var entry = CreateEntry(line);
                    _masterBuffer.Add(entry);

                    // Add to filter if matches
                    if (PassesFilter(entry)) _filteredEntries.Add(entry);

                    // Maintain SYNC: If filtered list exceeds max, prune from top
                    // Note: CircularBuffer auto-prunes master. Filtered list needs manual cap.
                    if (_filteredEntries.Count > MAX_ENTRIES) _filteredEntries.RemoveAt(0);

                    changed = true;
                }
                else if (_masterBuffer.Count > 0)
                {
                    // Append to last entry (it's a multi-line message)
                    var last = _masterBuffer[_masterBuffer.Count - 1];
                    last.RawBody += line + "\n";
                    last.DisplayBody = last.RawBody.Trim();
                    last.LastCachedHeight = -1;
                }
            }
            if (changed) Repaint();
        }

        private bool IsHeader(string line)
        {
            // Stack Trace / Footer detection
            // Note: Stack traces in Editor.log often come as unindented lines following the message.
            // We must detect these to prevent them from becoming their own entries.

            // 1. Footer: (Filename: ... Line: ...)
            if (line.StartsWith("(Filename:")) return false;

            // 2. Stack frame source location: (at ...)
            if (line.Contains("(at ")) return false;

            // 3. Common stack frame signatures (Namespace.Class:Method)
            // Heuristic needs to be careful not to kill valid logs.
            // Unity stack traces typically look like: "Namespace.Class:Method (Args)"
            if (line.Contains(":") && line.Contains("(") && line.IndexOf(':') < line.IndexOf('('))
            {
                // This covers "UnityEngine.Debug:Log (object)" and similar
                // But could also match "MyLog: Something happened (context)"
                // To be safer, we can check for known prefixes or lack of spaces before colon
                int colon = line.IndexOf(':');
                // If there is no space before the colon, it's likely a method signature
                if (colon > 0 && line[colon - 1] != ' ') return false;
            }

            string trimmed = line.TrimStart();
            if (_timestampRegex.IsMatch(trimmed)) return true;
            if (trimmed.Contains("Antigravity:") || trimmed.StartsWith("Error") || trimmed.StartsWith("Warning")) return true;
            if (line.StartsWith("[") && !line.Contains("UnityEngine.Debug")) return true;
            return !line.StartsWith(" ") && !line.StartsWith("\t");
        }

        private LogEntry CreateEntry(string line)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string header = line.TrimStart();
            var match = _timestampRegex.Match(header);
            if (match.Success)
            {
                timestamp = match.Value.Trim('[', ']');
                header = header.Substring(match.Length).Trim();
            }

            LogType type = LogType.Log;
            if (header.StartsWith("Error")) type = LogType.Error;
            else if (header.StartsWith("Warning")) type = LogType.Warning;

            var entry = new LogEntry
            {
                Id = _idCounter++,
                Timestamp = timestamp,
                RawHeader = header,
                Type = type
            };
            entry.ProcessVisuals(_fileLineRegex);
            return entry;
        }

        private void RefreshFilters()
        {
            _filteredEntries.Clear();
            foreach (var e in _masterBuffer.Iterate())
            {
                e.LastCachedHeight = -1;
                if (PassesFilter(e)) _filteredEntries.Add(e);
            }
            Repaint();
        }

        private bool PassesFilter(LogEntry e)
        {
            if (string.IsNullOrEmpty(_searchFilter)) return true;
            return (e.RawHeader + " " + e.RawBody).IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // --- UI Rendering ---

        private void OnGUI()
        {
            InitStyles();
            TrackSelection();

            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), new Color(0.18f, 0.18f, 0.18f));
            DrawToolbar();

            float contentWidth = position.width - 30;
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                // Clear Selection on Scroll (Ghosting Fix)
                if (Event.current.type == EventType.ScrollWheel && Event.current.delta.y != 0)
                {
                    if (_autoScroll && Event.current.delta.y < 0) _autoScroll = false;
                    GUI.FocusControl(null);
                    _focusedEntryId = -1;
                    Repaint();
                }

                float currentY = 0;
                float viewTop = _scrollPos.y;
                float viewBottom = viewTop + position.height;

                for (int i = 0; i < _filteredEntries.Count; i++)
                {
                    var entry = _filteredEntries[i];

                    if (entry.LastCachedHeight < 0) CalculateEntryHeight(entry, contentWidth);

                    float h = entry.LastCachedHeight;
                    bool isVisible = (currentY + h > viewTop && currentY < viewBottom);

                    if (isVisible || entry.Id == _focusedEntryId)
                    {
                        DrawEntry(entry, currentY, contentWidth);
                    }

                    GUILayout.Space(h);
                    currentY += h;
                }

                if (_autoScroll && Event.current.type == EventType.Layout) _scrollPos.y = 1000000;

                // Smart Resume
                if (!_autoScroll && Event.current.type == EventType.Repaint)
                {
                    if (_scrollPos.y + (position.height - 40) >= currentY - 5) _autoScroll = true;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void CalculateEntryHeight(LogEntry entry, float width)
        {
            bool hasBody = !string.IsNullOrEmpty(entry.DisplayBody);
            GUIStyle headStyle = hasBody ? _headerStyle : _labelStyle;
            // Use DisplayHeader because it includes the timestamp, which affects wrapping!
            entry.LastHeaderHeight = headStyle.CalcHeight(new GUIContent(entry.DisplayHeader), width - 20) + 2;
            entry.LastCachedHeight = entry.LastHeaderHeight; // Compact: No extra vertical padding
            if (hasBody && entry.IsExpanded)
            {
                entry.LastCachedHeight += _bodyStyle.CalcHeight(new GUIContent(entry.DisplayBody), width - 22) + 12;
            }
        }

        private void DrawEntry(LogEntry entry, float y, float width)
        {
            Rect entryRect = new Rect(0, y, width + 30, entry.LastCachedHeight);
            bool hasBody = !string.IsNullOrEmpty(entry.DisplayBody);

            GUI.BeginGroup(entryRect);
            {
                Color color = Color.white;
                if (entry.Type == LogType.Error) color = new Color(1f, 0.45f, 0.45f);
                else if (entry.Type == LogType.Warning) color = new Color(1f, 0.95f, 0.45f);
                else if (entry.RawHeader.Contains("Antigravity:")) color = new Color(0.6f, 1f, 0.6f);

                var oldColor = GUI.contentColor;
                GUI.contentColor = color;

                if (hasBody)
                {
                    bool next = EditorGUI.Foldout(new Rect(4, 1, 15, entry.LastHeaderHeight), entry.IsExpanded, "", true);
                    if (next != entry.IsExpanded) { entry.IsExpanded = next; entry.LastCachedHeight = -1; }

                    GUI.SetNextControlName("log_header_" + entry.Id);
                    EditorGUI.SelectableLabel(new Rect(20, 1, width - 20, entry.LastHeaderHeight), entry.DisplayHeader, _headerStyle);

                    if (entry.IsExpanded)
                    {
                        float bodyY = entry.LastHeaderHeight + 2;
                        float bodyH = entry.LastCachedHeight - entry.LastHeaderHeight - 4;
                        GUI.SetNextControlName("log_body_" + entry.Id);
                        EditorGUI.SelectableLabel(new Rect(22, bodyY, width - 20, bodyH), entry.DisplayBody, _bodyStyle);
                    }
                }
                else
                {
                    GUI.SetNextControlName("log_header_" + entry.Id);
                    EditorGUI.SelectableLabel(new Rect(20, 1, width - 20, entry.LastHeaderHeight), entry.DisplayHeader, _labelStyle);
                }
                GUI.contentColor = oldColor;
            }
            GUI.EndGroup();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                // Clear List, but DON'T reset file pos to 0 (keeps old logs hidden)
                if (GUILayout.Button("Clear Logs", EditorStyles.toolbarButton, GUILayout.Width(65)))
                {
                    ResetInternalState(false);
                    Repaint();
                }

                EditorGUI.BeginChangeCheck();
                _expandAll = GUILayout.Toggle(_expandAll, _expandAll ? "Collapse All" : "Expand All", EditorStyles.toolbarButton, GUILayout.Width(75));
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var e in _masterBuffer.Iterate()) { e.IsExpanded = _expandAll; e.LastCachedHeight = -1; }
                }

                if (GUILayout.Button("Copy All", EditorStyles.toolbarButton, GUILayout.Width(60)))
                {
                    var sb = new StringBuilder();
                    foreach (var e in _filteredEntries) sb.AppendLine(e.GetFullText());
                    EditorGUIUtility.systemCopyBuffer = sb.ToString();
                }

                var oldColor = GUI.contentColor;
                if (_autoScroll) GUI.contentColor = new Color(0.5f, 1f, 0.5f);
                if (GUILayout.Button("Tail", EditorStyles.toolbarButton, GUILayout.Width(35)))
                {
                    _autoScroll = true;
                    _scrollPos.y = 1000000;
                    Repaint();
                }
                GUI.contentColor = oldColor;
                GUILayout.Space(5);

                EditorGUI.BeginChangeCheck();
                _searchFilter = EditorGUILayout.TextField(_searchFilter, EditorStyles.toolbarSearchField);
                if (EditorGUI.EndChangeCheck()) RefreshFilters();

                if (GUILayout.Button("", "ToolbarSearchCancelButton")) { _searchFilter = ""; GUI.FocusControl(null); RefreshFilters(); }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void InitStyles()
        {
            if (_headerStyle == null)
            {
                var font = AssetDatabase.LoadAssetAtPath<Font>("Packages/com.antigravity.toolkit/Fonts/AnkaCoder-C87-r.ttf");

                _headerStyle = new GUIStyle(EditorStyles.label) { richText = true, padding = new RectOffset(0, 0, 0, 0), font = font };
                _headerStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
            }
            if (_labelStyle == null)
            {
                var font = AssetDatabase.LoadAssetAtPath<Font>("Packages/com.antigravity.toolkit/Fonts/AnkaCoder-C87-r.ttf");

                _labelStyle = new GUIStyle(EditorStyles.label) { richText = true, padding = new RectOffset(0, 0, 0, 0), font = font };
                _labelStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
            }
            if (_bodyStyle == null)
            {
                var font = AssetDatabase.LoadAssetAtPath<Font>("Packages/com.antigravity.toolkit/Fonts/AnkaCoder-C87-r.ttf");

                _bodyStyle = new GUIStyle(EditorStyles.label) { wordWrap = true, richText = true, font = font };
                _bodyStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
            }
        }

        private void TrackSelection()
        {
            string focused = GUI.GetNameOfFocusedControl();
            if (focused.StartsWith("log_body_")) int.TryParse(focused.Substring(9), out _focusedEntryId);
            else if (focused.StartsWith("log_header_")) int.TryParse(focused.Substring(11), out _focusedEntryId);
            else _focusedEntryId = -1;
        }
    }
}
