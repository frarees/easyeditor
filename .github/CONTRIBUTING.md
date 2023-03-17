# Contributing to Easy Editor

Pull requests are welcome.

# Style guidelines

Stick to the defined [EditorConfig](../.editorconfig) rules.

# Development environment

1. Open a Unity project – it can be a new or an existing one.
2. Clone this repository in the `Packages/` directory.
3. Focus Unity. Open the [Package Manager](https://docs.unity3d.com/Manual/upm-ui.html). [Change the scope](https://docs.unity3d.com/Manual/upm-ui-filter.html) to `In Project`.
4. You should see Easy Editor, tagged as `In Development`.
5. In the [project window](https://docs.unity3d.com/Manual/ProjectView.html), navigate to `Packages/Easy Editor`. You can find all this project's files there.

# Adding support for an editor

Setup a development environment following the steps above.

1. Go to `Packages/Easy Editor/Registries`
2. Select the registry that matches your OS.
3. Expand `Discoveries` and add a new entry.
4. After you've configured the discovery, trigger a recompilation. You can do that by modifying or (re)importing a script.
