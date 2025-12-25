using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Antigravity.Editor
{
    [InitializeOnLoad]
    public static class EditorLifecycle
    {
        // Toggle this to see logs in the console
        public static bool EnableDebugLogs => AntigravitySettings.ShowDebugLogs;

        // Auto-save feature
        public static bool AutoSaveOnFocusLoss = true;

        static EditorLifecycle()
        {
            // Subscribe to all relevant events
            EditorApplication.update += OnUpdate;
            EditorApplication.delayCall += OnDelayCall;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.pauseStateChanged += OnPauseStateChanged;
            EditorApplication.quitting += OnQuitting;
            EditorApplication.wantsToQuit += OnWantsToQuit;
            EditorApplication.projectChanged += OnProjectChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.modifierKeysChanged += OnModifierKeysChanged;

            // Focus changed event (available in newer Unity versions, generally 2018+)
            EditorApplication.focusChanged += OnFocusChanged;

            // Contextual menu callback
            EditorApplication.contextualPropertyMenu += OnContextualPropertyMenu;

            if (EnableDebugLogs)
            {
                Debug.Log("[EditorLifecycle] Initialized and listening to events.");
            }
        }

        private static void OnUpdate()
        {
            // Update runs very frequently, so we typically don't log it unless debugging specifically for it
            // if (EnableDebugLogs) Debug.Log("[EditorLifecycle] Update");
        }

        private static void OnDelayCall()
        {
            if (EnableDebugLogs) Debug.Log("[EditorLifecycle] DelayCall executed");
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (EnableDebugLogs) Debug.Log($"[EditorLifecycle] PlayModeStateChanged: {state}");
        }

        private static void OnPauseStateChanged(PauseState state)
        {
            if (EnableDebugLogs) Debug.Log($"[EditorLifecycle] PauseStateChanged: {state}");
        }

        private static void OnQuitting()
        {
            if (EnableDebugLogs) Debug.Log("[EditorLifecycle] Quitting");
        }

        private static bool OnWantsToQuit()
        {
            if (EnableDebugLogs) Debug.Log("[EditorLifecycle] WantsToQuit");
            return true; // Return true to allow quitting
        }

        private static void OnProjectChanged()
        {
            if (EnableDebugLogs) Debug.Log("[EditorLifecycle] ProjectChanged");
        }

        private static void OnHierarchyChanged()
        {
            if (EnableDebugLogs) Debug.Log("[EditorLifecycle] HierarchyChanged");
        }

        private static void OnModifierKeysChanged()
        {
            if (EnableDebugLogs) Debug.Log("[EditorLifecycle] ModifierKeysChanged");
        }

        private static void OnFocusChanged(bool focused)
        {
            if (EnableDebugLogs) Debug.Log($"[EditorLifecycle] FocusChanged: {focused}");

            if (!focused && AutoSaveOnFocusLoss)
            {
                if (EditorSceneManager.SaveOpenScenes())
                {
                    // Debug.Log("[EditorLifecycle] Auto-saved scenes on focus loss.");
                }
                AssetDatabase.SaveAssets();
                if (EnableDebugLogs) Debug.Log($"[EditorLifecycle] Auto-saved scenes and assets (Focus Lost)");
            }
        }

        private static void OnContextualPropertyMenu(GenericMenu menu, SerializedProperty property)
        {
            if (EnableDebugLogs) Debug.Log($"[EditorLifecycle] ContextualPropertyMenu for property: {property.propertyPath}");
        }
    }
}
