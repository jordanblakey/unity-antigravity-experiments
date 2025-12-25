using UnityEditor;
using UnityEngine;

namespace Antigravity.Editor
{
    public static class AntigravitySettings
    {
        private const string DebugLogsKey = "Antigravity.DebugLogs";

        public static bool ShowDebugLogs
        {
            get => EditorPrefs.GetBool(DebugLogsKey, true);
            set => EditorPrefs.SetBool(DebugLogsKey, value);
        }

    }
}
