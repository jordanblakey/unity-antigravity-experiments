using UnityEditor;
using UnityEngine;

namespace Antigravity.Editor
{
    public static class AntigravityDiagnosticsMenuOption
    {
        static AntigravityDiagnosticsMenuOption()
        {
        }

        [MenuItem("Antigravity/Show Project Diagnostics %#d", false, 10)]
        public static void ShowProjectDiagnostics()
        {
            Debug.Log("--- Antigravity Diagnostics ---");

            // EditorApplication
            Debug.Log($"[EditorApplication] applicationContentsPath: {EditorApplication.applicationContentsPath}");
            Debug.Log($"[EditorApplication] applicationPath: {EditorApplication.applicationPath}");
            Debug.Log($"[EditorApplication] applicationToolsPath: {EditorApplication.applicationToolsPath}");
            Debug.Log($"[EditorApplication] isCompiling: {EditorApplication.isCompiling}");
            Debug.Log($"[EditorApplication] isFocused: {EditorApplication.isFocused}");
            Debug.Log($"[EditorApplication] isPaused: {EditorApplication.isPaused}");
            Debug.Log($"[EditorApplication] isPlaying: {EditorApplication.isPlaying}");
            Debug.Log($"[EditorApplication] isPlayingOrWillChangePlaymode: {EditorApplication.isPlayingOrWillChangePlaymode}");
            Debug.Log($"[EditorApplication] isRemoteConnected: {EditorApplication.isRemoteConnected}");
            Debug.Log($"[EditorApplication] isTemporaryProject: {EditorApplication.isTemporaryProject}");
            Debug.Log($"[EditorApplication] isUpdating: {EditorApplication.isUpdating}");
            Debug.Log($"[EditorApplication] sevenZipPath: {EditorApplication.sevenZipPath}");
            Debug.Log($"[EditorApplication] timeSinceStartup: {EditorApplication.timeSinceStartup}");
            Debug.Log("-------------------------------");

            // Application
            Debug.Log($"[Application] unityVersion: {Application.unityVersion}");
            Debug.Log($"[Application] version: {Application.version}");
            Debug.Log($"[Application] platform: {Application.platform}");
            Debug.Log($"[Application] dataPath: {Application.dataPath}");
            Debug.Log($"[Application] persistentDataPath: {Application.persistentDataPath}");
            Debug.Log($"[Application] streamingAssetsPath: {Application.streamingAssetsPath}");
            Debug.Log($"[Application] temporaryCachePath: {Application.temporaryCachePath}");
            Debug.Log($"[Application] productName: {Application.productName}");
            Debug.Log($"[Application] companyName: {Application.companyName}");
            Debug.Log($"[Application] isEditor: {Application.isEditor}");
            Debug.Log($"[Application] isBatchMode: {Application.isBatchMode}");
            Debug.Log($"[Application] systemLanguage: {Application.systemLanguage}");
            Debug.Log($"[Application] internetReachability: {Application.internetReachability}");
            Debug.Log("-------------------------------");

            // SystemInfo
            Debug.Log($"[SystemInfo] deviceName: {SystemInfo.deviceName}");
            Debug.Log($"[SystemInfo] deviceModel: {SystemInfo.deviceModel}");
            Debug.Log($"[SystemInfo] operatingSystem: {SystemInfo.operatingSystem}");
            Debug.Log($"[SystemInfo] systemMemorySize: {SystemInfo.systemMemorySize} MB");
            Debug.Log($"[SystemInfo] graphicsDeviceName: {SystemInfo.graphicsDeviceName}");
            Debug.Log($"[SystemInfo] processorCount: {SystemInfo.processorCount}");
            Debug.Log("-------------------------------");

            // EditorUserBuildSettings
            Debug.Log($"[EditorUserBuildSettings] activeBuildTarget: {EditorUserBuildSettings.activeBuildTarget}");
            Debug.Log($"[EditorUserBuildSettings] selectedBuildTargetGroup: {EditorUserBuildSettings.selectedBuildTargetGroup}");
            Debug.Log($"[EditorUserBuildSettings] development: {EditorUserBuildSettings.development}");
            Debug.Log("-------------------------------");

            // EditorBuildSettings
            Debug.Log($"[EditorBuildSettings] globalScenes.Length: {EditorBuildSettings.globalScenes.Length}");
            Debug.Log($"[EditorBuildSettings] scenes.Length: {EditorBuildSettings.scenes.Length}");
            Debug.Log("-------------------------------");

            // Scenes
            Debug.Log($"[SceneManager] sceneCount: {UnityEngine.SceneManagement.SceneManager.sceneCount}");
            Debug.Log($"[SceneManager] activeScene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            Debug.Log("-------------------------------");

            // EditorUserBuildSettings
            Debug.Log($"[EditorUserBuildSettings] activeBuildTarget: {EditorUserBuildSettings.activeBuildTarget}");
            Debug.Log($"[EditorUserBuildSettings] selectedBuildTargetGroup: {EditorUserBuildSettings.selectedBuildTargetGroup}");
            Debug.Log($"[EditorUserBuildSettings] development: {EditorUserBuildSettings.development}");
            Debug.Log($"[EditorUserBuildSettings] activeScriptCompilationDefines: {EditorUserBuildSettings.activeScriptCompilationDefines}");
            Debug.Log($"[EditorUserBuildSettings] allowDebugging: {EditorUserBuildSettings.allowDebugging}");
            Debug.Log($"[EditorUserBuildSettings] androidBuildSubtarget: {EditorUserBuildSettings.androidBuildSubtarget}");
            Debug.Log($"[EditorUserBuildSettings] androidBuildSystem: {EditorUserBuildSettings.androidBuildSystem}");
            Debug.Log($"[EditorUserBuildSettings] androidBuildType: {EditorUserBuildSettings.androidBuildType}");
            Debug.Log($"[EditorUserBuildSettings] buildAppBundle: {EditorUserBuildSettings.buildAppBundle}");
            Debug.Log($"[EditorUserBuildSettings] buildScriptsOnly: {EditorUserBuildSettings.buildScriptsOnly}");
            Debug.Log($"[EditorUserBuildSettings] buildWithDeepProfilingSupport: {EditorUserBuildSettings.buildWithDeepProfilingSupport}");
            Debug.Log($"[EditorUserBuildSettings] compressFilesInPackage: {EditorUserBuildSettings.compressFilesInPackage}");
            Debug.Log($"[EditorUserBuildSettings] connectProfiler: {EditorUserBuildSettings.connectProfiler}");
            Debug.Log($"[EditorUserBuildSettings] development: {EditorUserBuildSettings.development}");
            Debug.Log($"[EditorUserBuildSettings] explicitArrayBoundsChecks: {EditorUserBuildSettings.explicitArrayBoundsChecks}");
            Debug.Log($"[EditorUserBuildSettings] explicitDivideByZeroChecks: {EditorUserBuildSettings.explicitDivideByZeroChecks}");
            Debug.Log($"[EditorUserBuildSettings] explicitNullChecks: {EditorUserBuildSettings.explicitNullChecks}");
            Debug.Log($"[EditorUserBuildSettings] exportAsGoogleAndroidProject: {EditorUserBuildSettings.exportAsGoogleAndroidProject}");
            Debug.Log($"[EditorUserBuildSettings] forceInstallation: {EditorUserBuildSettings.forceInstallation}");
            Debug.Log($"[EditorUserBuildSettings] installInBuildFolder: {EditorUserBuildSettings.installInBuildFolder}");
            Debug.Log($"[EditorUserBuildSettings] iOSXcodeBuildConfig: {EditorUserBuildSettings.iOSXcodeBuildConfig}");
            Debug.Log($"[EditorUserBuildSettings] macOSXcodeBuildConfig: {EditorUserBuildSettings.macOSXcodeBuildConfig}");
            Debug.Log($"[EditorUserBuildSettings] managedDebuggerFixedPort: {EditorUserBuildSettings.managedDebuggerFixedPort}");
            Debug.Log($"[EditorUserBuildSettings] movePackageToDiscOuterEdge: {EditorUserBuildSettings.movePackageToDiscOuterEdge}");
            Debug.Log($"[EditorUserBuildSettings] needSubmissionMaterials: {EditorUserBuildSettings.needSubmissionMaterials}");
            Debug.Log($"[EditorUserBuildSettings] overrideMaxTextureSize: {EditorUserBuildSettings.overrideMaxTextureSize}");
            Debug.Log($"[EditorUserBuildSettings] overrideTextureCompression: {EditorUserBuildSettings.overrideTextureCompression}");
            Debug.Log($"[EditorUserBuildSettings] pathOnRemoteDevice: {EditorUserBuildSettings.pathOnRemoteDevice}");
            Debug.Log($"[EditorUserBuildSettings] ps4BuildSubtarget: {EditorUserBuildSettings.ps4BuildSubtarget}");
            Debug.Log($"[EditorUserBuildSettings] ps4HardwareTarget: {EditorUserBuildSettings.ps4HardwareTarget}");
            Debug.Log($"[EditorUserBuildSettings] remoteDeviceAddress: {EditorUserBuildSettings.remoteDeviceAddress}");
            Debug.Log($"[EditorUserBuildSettings] remoteDeviceExports: {EditorUserBuildSettings.remoteDeviceExports}");
            Debug.Log($"[EditorUserBuildSettings] remoteDeviceUsername: {EditorUserBuildSettings.remoteDeviceUsername}");
            Debug.Log($"[EditorUserBuildSettings] selectedBuildTargetGroup: {EditorUserBuildSettings.selectedBuildTargetGroup}");
            Debug.Log($"[EditorUserBuildSettings] standaloneBuildSubtarget: {EditorUserBuildSettings.standaloneBuildSubtarget}");
            Debug.Log($"[EditorUserBuildSettings] switchCreateRomFile: {EditorUserBuildSettings.switchCreateRomFile}");
            Debug.Log($"[EditorUserBuildSettings] switchEnableDebugPad: {EditorUserBuildSettings.switchEnableDebugPad}");
            Debug.Log($"[EditorUserBuildSettings] switchEnableHostIO: {EditorUserBuildSettings.switchEnableHostIO}");
            Debug.Log($"[EditorUserBuildSettings] switchEnableMemoryTracker: {EditorUserBuildSettings.switchEnableMemoryTracker}");
            Debug.Log($"[EditorUserBuildSettings] switchEnableRomCompression: {EditorUserBuildSettings.switchEnableRomCompression}");
            Debug.Log($"[EditorUserBuildSettings] switchEnableUnpublishableErrors: {EditorUserBuildSettings.switchEnableUnpublishableErrors}");
            Debug.Log($"[EditorUserBuildSettings] switchHTCSScriptDebugging: {EditorUserBuildSettings.switchHTCSScriptDebugging}");
            Debug.Log($"[EditorUserBuildSettings] switchNVNAftermath: {EditorUserBuildSettings.switchNVNAftermath}");
            Debug.Log($"[EditorUserBuildSettings] switchNVNDrawValidation_Heavy: {EditorUserBuildSettings.switchNVNDrawValidation_Heavy}");
            Debug.Log($"[EditorUserBuildSettings] switchNVNDrawValidation_Light: {EditorUserBuildSettings.switchNVNDrawValidation_Light}");
            Debug.Log($"[EditorUserBuildSettings] switchNVNGraphicsDebugger: {EditorUserBuildSettings.switchNVNGraphicsDebugger}");
            Debug.Log($"[EditorUserBuildSettings] switchRomCompressionConfig: {EditorUserBuildSettings.switchRomCompressionConfig}");
            Debug.Log($"[EditorUserBuildSettings] switchRomCompressionLevel: {EditorUserBuildSettings.switchRomCompressionLevel}");
            Debug.Log($"[EditorUserBuildSettings] switchRomCompressionType: {EditorUserBuildSettings.switchRomCompressionType}");
            Debug.Log($"[EditorUserBuildSettings] switchSaveADF: {EditorUserBuildSettings.switchSaveADF}");
            Debug.Log($"[EditorUserBuildSettings] switchUseLegacyNvnPoolAllocator: {EditorUserBuildSettings.switchUseLegacyNvnPoolAllocator}");
            Debug.Log($"[EditorUserBuildSettings] switchWaitForMemoryTrackerOnStartup: {EditorUserBuildSettings.switchWaitForMemoryTrackerOnStartup}");
            Debug.Log($"[EditorUserBuildSettings] symlinkSources: {EditorUserBuildSettings.symlinkSources}");
            Debug.Log($"[EditorUserBuildSettings] waitForManagedDebugger: {EditorUserBuildSettings.waitForManagedDebugger}");
            Debug.Log($"[EditorUserBuildSettings] waitForPlayerConnection: {EditorUserBuildSettings.waitForPlayerConnection}");
            Debug.Log($"[EditorUserBuildSettings] webGLBuildSubtarget: {EditorUserBuildSettings.webGLBuildSubtarget}");
            Debug.Log($"[EditorUserBuildSettings] webGLClientBrowserPath: {EditorUserBuildSettings.webGLClientBrowserPath}");
            Debug.Log($"[EditorUserBuildSettings] webGLClientBrowserType: {EditorUserBuildSettings.webGLClientBrowserType}");
            Debug.Log($"[EditorUserBuildSettings] windowsBuildAndRunDeployTarget: {EditorUserBuildSettings.windowsBuildAndRunDeployTarget}");
            Debug.Log($"[EditorUserBuildSettings] windowsDevicePortalAddress: {EditorUserBuildSettings.windowsDevicePortalAddress}");
            Debug.Log($"[EditorUserBuildSettings] windowsDevicePortalPassword: {EditorUserBuildSettings.windowsDevicePortalPassword}");
            Debug.Log($"[EditorUserBuildSettings] windowsDevicePortalUsername: {EditorUserBuildSettings.windowsDevicePortalUsername}");
            Debug.Log($"[EditorUserBuildSettings] wsaArchitecture: {EditorUserBuildSettings.wsaArchitecture}");
            Debug.Log($"[EditorUserBuildSettings] wsaBuildAndRunDeployTarget: {EditorUserBuildSettings.wsaBuildAndRunDeployTarget}");
            Debug.Log($"[EditorUserBuildSettings] wsaMinUWPSDK: {EditorUserBuildSettings.wsaMinUWPSDK}");
            Debug.Log($"[EditorUserBuildSettings] wsaUWPVisualStudioVersion: {EditorUserBuildSettings.wsaUWPVisualStudioVersion}");

            Debug.Log("-------------------------------");
        }
    }
}
