@echo off
rem Opens the SE2VR.Content game content project in the Space Engineers 2 Mod SDK editor

set "sdkPath=%ProgramFiles(x86)%\Steam\steamapps\common\Space Engineers 2 - Mod SDK"
set "gamePath=%~dp0GameLibs"
set "projectPath=%~dp0SE2VR.Content\SE2VR.Content.vrgproj"

if not exist "%sdkPath%\Editor\VRage.Editor.exe" (
    echo Mod SDK editor not found at "%sdkPath%", edit sdkPath in this file
    pause
    exit /b 1
)

if not exist "%gamePath%\SpaceEngineers2.exe" (
    echo Game not found at "%gamePath%", run SetupRepository.bat first
    pause
    exit /b 1
)

cd /D "%gamePath%"
start "" "%sdkPath%\Editor\VRage.Editor.exe" --project "%projectPath%" --globalProjectDirs "%sdkPath%\GameData" --globalAssemblyDirs "%sdkPath%\Editor" --projectExecutableSearchPaths "%gamePath%"
