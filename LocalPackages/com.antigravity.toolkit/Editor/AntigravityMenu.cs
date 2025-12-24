using UnityEditor;
using UnityEngine;

namespace Antigravity.Editor
{
    public static class AntigravityMenu
    {
        [MenuItem("Antigravity/Toggle Play Mode %g")] // Shortcut: Ctrl/Cmd + G
        public static void TogglePlayMode()
        {
            EditorApplication.isPlaying = !EditorApplication.isPlaying;
            
            string status = EditorApplication.isPlaying ? "Entering" : "Exiting";
            Debug.Log($"Antigravity: {status} Play Mode.");
        }
    }
}
