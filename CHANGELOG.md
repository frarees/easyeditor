# Changelog

## [0.5.1] - 2020-10-12

### Added
- Notes on limitations.
- Notes on setting up a development environment.
- `package.json` EditorConfig rules.
- .NET EditorConfig rules.

### Changed
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

[Unreleased]: https://github.com/frarees/easyeditor/compare/v0.5.1...HEAD
[0.5.1]: https://github.com/frarees/easyeditor/compare/v0.5.0...v0.5.1
[0.5.0]: https://github.com/frarees/easyeditor/compare/v0.4.3...v0.5.0
[0.4.3]: https://github.com/frarees/easyeditor/compare/v0.4.2...v0.4.3
[0.4.2]: https://github.com/frarees/easyeditor/compare/v0.4.1...v0.4.2
[0.4.1]: https://github.com/frarees/easyeditor/compare/v0.4.0...v0.4.1
[0.4.0]: https://github.com/frarees/easyeditor/compare/v0.3.0...v0.4.0
[0.3.0]: https://github.com/frarees/easyeditor/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/frarees/easyeditor/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/frarees/easyeditor/releases/tag/v0.1.0

