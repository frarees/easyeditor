# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.4.3] - 2020-09-25

### Added
- EditorConfig file.
- Links to releases in the changelog.

### Changed
- Deleted BOM on source files.
- Code linting via dotnet-format.

## [0.4.2] - 2020-09-25

### Added
- gVim 8.1 and 8.2 support for Windows.
- Sublime Text 3 support for Windows.

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
- Clarified comments on former changelogs.

## [0.3.0] - 2020-09-20

### Added
- Per launcher settings.
- Tooltips on settigns.

### Changed
- Robust code editor installation matching.

## [0.2.0] - 2020-09-16

### Added
- Ability to match a proper C# compiler version: 7.3 for 2020.1 and below, 8.0 for 2020.2 and up.

### Changed
- Setting names namespaced. Breaks compatibility with previous saved settings.
- Only recognized installations are considered.
- LaunchDescriptor no longer public.

### Removed
- Preference tab.

## [0.1.0] - 2020-09-14

### Added
- Core functionality.
- Support for MacVim and Sublime Text on macOS.

[0.4.3]: https://github.com/olivierlacan/keep-a-changelog/compare/0.4.2...0.4.3
[0.4.2]: https://github.com/olivierlacan/keep-a-changelog/compare/0.4.1...0.4.2
[0.4.1]: https://github.com/olivierlacan/keep-a-changelog/compare/0.4.0...0.4.1
[0.4.0]: https://github.com/olivierlacan/keep-a-changelog/compare/0.3.0...0.4.0
[0.3.0]: https://github.com/olivierlacan/keep-a-changelog/compare/0.2.0...0.3.0
[0.2.0]: https://github.com/olivierlacan/keep-a-changelog/compare/0.1.0...0.2.0
[0.1.0]: https://github.com/olivierlacan/keep-a-changelog/releases/tag/0.1.0

