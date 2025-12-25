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
            EditorApplication.contextualPropertyMenu += OnContextualPropertyMenu;
            EditorApplication.delayCall += OnDelayCall;
            EditorApplication.focusChanged += OnFocusChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.modifierKeysChanged += OnModifierKeysChanged;
            EditorApplication.pauseStateChanged += OnPauseStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.projectChanged += OnProjectChanged;
            EditorApplication.quitting += OnQuitting;
            EditorApplication.searchChanged += OnSearchChanged;
            EditorApplication.update += OnUpdate;
            EditorApplication.updateMainWindowTitle += OnUpdateMainWindowTitle;
            EditorApplication.wantsToQuit += OnWantsToQuit;

            // LOUD
            // EditorApplication.projectWindowItemInstanceOnGUI += OnProjectWindowItemInstanceOnGUI;
            // EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;

            if (EnableDebugLogs)
            {
                Debug.Log("[EditorLifecycle] Initialized and listening to events.");
            }
        }

        private static void OnContextualPropertyMenu(GenericMenu menu, SerializedProperty property)
        {
            if (EnableDebugLogs) Debug.Log($"[EditorLifecycle] ContextualPropertyMenu for property: {property.propertyPath}");
        }

        private static void OnDelayCall()
        {
            if (EnableDebugLogs) Debug.Log("[EditorLifecycle] DelayCall executed");
        }

        private static void OnFocusChanged(bool focused)
        {
            if (EnableDebugLogs) Debug.Log($"[EditorLifecycle] FocusChanged: {focused}");
            if (!focused && AutoSaveOnFocusLoss)
            {
                // Save assets
                AssetDatabase.SaveAssets();
                // Save scenes
                int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
                for (int i = 0; i < sceneCount; i++)
                {
                    UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                    if (scene.isDirty)
                    {
                        if (EnableDebugLogs) Debug.Log($"[EditorLifecycle] Auto-saving scene: {scene.name}");
                        EditorSceneManager.SaveScene(scene);
                    }
                }
            }
        }

        private static void OnHierarchyChanged()
        {
            if (EnableDebugLogs) Debug.Log("[EditorLifecycle] HierarchyChanged");
        }

        private static void OnModifierKeysChanged()
        {
            // if (EnableDebugLogs) Debug.Log("[EditorLifecycle] ModifierKeysChanged");
        }

        private static void OnPauseStateChanged(PauseState state)
        {
            if (EnableDebugLogs) Debug.Log($"[EditorLifecycle] PauseStateChanged: {state}");
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (EnableDebugLogs) Debug.Log($"[EditorLifecycle] PlayModeStateChanged: {state}");
        }

        private static void OnProjectChanged()
        {
            if (EnableDebugLogs) Debug.Log("[EditorLifecycle] ProjectChanged");
        }

        private static void OnQuitting()
        {
            if (EnableDebugLogs) Debug.Log("[EditorLifecycle] Quitting");
        }

        private static void OnSearchChanged()
        {
            if (EnableDebugLogs) Debug.Log("[EditorLifecycle] SearchChanged");
        }

        private static void OnUpdate()
        {
            // Update runs very frequently, so we typically don't log it unless debugging specifically for it
            // if (EnableDebugLogs) Debug.Log("[EditorLifecycle] Update");
        }

        private static void OnUpdateMainWindowTitle(ApplicationTitleDescriptor titleDescriptor)
        {
            if (EnableDebugLogs) Debug.Log($"[EditorLifecycle] UpdateMainWindowTitle: {titleDescriptor}");
        }

        private static bool OnWantsToQuit()
        {
            if (EnableDebugLogs) Debug.Log("[EditorLifecycle] WantsToQuit");
            return true; // Return true to allow quitting
        }

        // LOUD
        // private static void OnProjectWindowItemInstanceOnGUI(int instanceID, Rect selectionRect)
        // {
        //     if (EnableDebugLogs) Debug.Log($"[EditorLifecycle] ProjectWindowItemInstanceOnGUI for instanceID: {instanceID}, Rect: {selectionRect}");
        // }

        // private static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
        // {
        //     if (EnableDebugLogs) Debug.Log($"[EditorLifecycle] ProjectWindowItemOnGUI for guid: {guid}, Rect: {selectionRect}");
        // }

    }
}