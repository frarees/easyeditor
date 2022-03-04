# Changelog

## [0.8.1] - 2022-03-04

### Added
- Override [LangVersion](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version).

### Removed
- Match compiler version flag.

### ProjectGenerator (matching [1.2.5](https://github.com/Unity-Technologies/com.unity.ide.vscode/tree/1.2.5/Packages/com.unity.ide.vscode/Editor/ProjectGeneration))
- Introduce OnGeneratedCSProjectFiles, OnGeneratedCSProject and OnGeneratedSlnSolution callbacks.
- Always use forward slash in source paths
- Analyzers use absolute paths
- Ruleset files for roslyn analyzers
- Extra snap search paths on Ubuntu
- Specific c# language version for specific unity versions
- No longer hide .gitignore in file explorer

## [0.8.0] - 2022-01-28

### Added
- Matching compiler version up to Unity 2022.1 (C# 9.0).

### Changed
- Updated Roslyn analyzers to [1.12.0](https://github.com/microsoft/Microsoft.Unity.Analyzers/releases/tag/1.12.0).

### Removed
- Postprocessor class (.csproj postprocessor).

### ProjectGenerator (matching [1.2.4](https://github.com/Unity-Technologies/com.unity.ide.vscode/tree/1.2.4/Packages/com.unity.ide.vscode/Editor/ProjectGeneration))
- Allow for per assembly roslyn analyzers
- Optimize package info look up performance. Reducing project generation time
- Use absolute path for Roslyn analyzers
- Always sync on asset updates

## [0.7.0] - 2021-04-01

### Added
- Ability to specify which projects get generated.
- Workaround for RoslynAnalysisRunner issues.

### Changed
- Fix registering editor on initial startup.
- Fix opening files custom extension (set in EditorSettings.projectGenerationUserExtensions).

## [0.6.1] - 2020-10-26

### Changed
- Fix crash on initialization.

## [0.6.0] - 2020-10-17

### Added
- Revised installation paths.
- Fall back on PATH when searching for installations.
- Check for successful MonoInstallationFinder reflection.
- Export FrameworkPathOverride setting for integrations that allow passing environment variables.
- Native opening for editors that require it.
- Custom arguments per editor.
- Generate project files on demand.
- Discovery APIs to find and use installations.
- Support for BBEdit and CotEditor on macOS.

### Removed
- Launcher library and its APIs.
- Setting class.
- OmniSharp reloading.
- Matching compiler version on 2020.2+ (Unity fixed it).

## [0.5.1] - 2020-10-12

### Added
- Notes on limitations.
- Notes on setting up a development environment.
- `package.json` EditorConfig rules.
- .NET EditorConfig rules.

### Removed
- Don't include the README in the package.

## [0.5.0] - 2020-10-06

### Added
- Support for VimR on macOS.
- Setting class to define launcher settings.
- Unregister Easy Editor before the assembly reloads.
- Documentation.

### Changed
- Launchers inherit from abstract class (Launcher) instead of interface (ILauncher).
- Fix changelog links.
- Clarify comments on former changelogs.

## [0.4.3] - 2020-09-25

### Added
- EditorConfig file.
- Links to releases in the changelog.

### Changed
- Delete BOM on source files.
- Code format via dotnet-format.

## [0.4.2] - 2020-09-25

### Added
- Support for gVim 8.1 and 8.2 on Windows.
- Support for Sublime Text 3 on Windows.

## [0.4.1] - 2020-09-25

### Changed
- Don't try to validate an executable if the file doesn't exist in the first place.

## [0.4.0] - 2020-09-25

### Added
- Cross-platform framework path searching.
- Documentation links point to GitHub (Unity 2020.2).

### Changed
- Split into assemblies: EasyEditor, EasyEditor.Launchers and EasyEditor.Reflected.
- Reflected methods now in separate assembly (EasyEditor.Reflected).
- Clarify comments on former changelogs.

## [0.3.0] - 2020-09-20

### Added
- Per launcher settings.
- Tooltips on settigns.

### Changed
- Robust code editor installation matching.

## [0.2.0] - 2020-09-16

### Added
- Setting to match a proper C# compiler version: 7.3 for 2020.1 and below, 8.0 for 2020.2 and up.

### Changed
- Setting names namespaced. Breaks compatibility with previous saved settings.
- Only recognized installations are considered.
- LaunchDescriptor no longer public.

### Removed
- Preference tab.

## [0.1.0] - 2020-09-14

### Added
- Core functionality.
- Support for MacVim on macOS.
- Support for Sublime Text 3 on macOS.

[0.8.1]: https://github.com/frarees/easyeditor/compare/v0.8.0...v0.8.1
[0.8.0]: https://github.com/frarees/easyeditor/compare/v0.7.0...v0.8.0
[0.7.0]: https://github.com/frarees/easyeditor/compare/v0.6.1...v0.7.0
[0.6.1]: https://github.com/frarees/easyeditor/compare/v0.6.0...v0.6.1
[0.6.0]: https://github.com/frarees/easyeditor/compare/v0.5.1...v0.6.0
[0.5.1]: https://github.com/frarees/easyeditor/compare/v0.5.0...v0.5.1
[0.5.0]: https://github.com/frarees/easyeditor/compare/v0.4.3...v0.5.0
[0.4.3]: https://github.com/frarees/easyeditor/compare/v0.4.2...v0.4.3
[0.4.2]: https://github.com/frarees/easyeditor/compare/v0.4.1...v0.4.2
[0.4.1]: https://github.com/frarees/easyeditor/compare/v0.4.0...v0.4.1
[0.4.0]: https://github.com/frarees/easyeditor/compare/v0.3.0...v0.4.0
[0.3.0]: https://github.com/frarees/easyeditor/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/frarees/easyeditor/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/frarees/easyeditor/releases/tag/v0.1.0

