using UnityEditor;
using UnityEngine;

namespace Antigravity.Editor
{
  public static class AntigravityMenu
  {
    [MenuItem("Antigravity/Toggle Debug Logs")]
    public static void ToggleDebugLogs()
    {
      AntigravitySettings.ShowDebugLogs = !AntigravitySettings.ShowDebugLogs;
      Debug.Log($"Antigravity: Debug Logs set to {AntigravitySettings.ShowDebugLogs}");
    }

    [MenuItem("Antigravity/Toggle Debug Logs", true)]
    public static bool ToggleDebugLogsValidate()
    {
      Menu.SetChecked("Antigravity/Toggle Debug Logs", AntigravitySettings.ShowDebugLogs);
      return true;
    }
  }
}
