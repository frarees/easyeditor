# Easy Editor

Unity package to automate external code editor integrations:

- List compatible installed editors.
- Per editor settings.
- Project files (re)generation.

## Installation

1. Install via [git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html):

```
https://github.com/frarees/easyeditor.git
```

Alternatively, you can install a specific version:

```
https://github.com/frarees/easyeditor.git#v0.5.0
```

2. Install via [local tarball](https://docs.unity3d.com/Manual/upm-ui-tarball.html):

Each [release](https://github.com/frarees/easyeditor/releases) includes a tarball named `com.frarees.easyeditor-<version>.tgz`.

3. Install via [local folder](https://docs.unity3d.com/Manual/upm-ui-local.html):

Download the repository contents and point Unity to the `package.json` file.

You can download the repository as a compressed file for a specific [tag](https://github.com/frarees/easyeditor/tags).

## Usage

Select your favourite [external script editor](https://docs.unity3d.com/Manual/Preferences.html#External-Tools) in Unity. You will get specific settings per editor. The rest is taken care of.

![Preferences window](Documentation~/prefs.png)

## Supported editors

| Editor         | Windows | macOS | Linux |
| :------------: | :-----: | :---: | :---: |
| MacVim         |         | ✅    |       |
| VimR           |         | ✅    |       |
| gVim           | ✅      |       |       |
| Sublime Text 3 | ✅      | ✅    |       |

_gVim versions 8.1 and 8.2._

Is there an editor you miss on this list? [Ask for it](https://github.com/frarees/easyeditor/issues/new?assignees=frarees&labels=enhancement&template=feature_request.md&title=) or [contribute](.github/CONTRIBUTING.md) your implementation.

## Contributing

See [contributing](.github/CONTRIBUTING.md) and the [code of conduct](.github/CODE_OF_CONDUCT.md).

