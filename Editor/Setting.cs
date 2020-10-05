namespace EasyEditor
{
    using UnityEditor;

    internal readonly struct Setting
    {
        public readonly string id;
        public readonly string description;
        public readonly string tooltip;

        public readonly string key;

        public Setting(string id, string description, string tooltip, string scope)
        {
            this.id = id;
            this.description = description;
            this.tooltip = tooltip;
            key = $"{nameof(EasyEditor)}.{scope}.{id}";
        }

        public Setting(string id, string description, string tooltip)
        {
            this.id = id;
            this.description = description;
            this.tooltip = tooltip;
            key = $"{nameof(EasyEditor)}.{id}";
        }

        public void SetBool(bool value)
        {
            EditorPrefs.SetBool(key, value);
        }

        public bool GetBool()
        {
            return EditorPrefs.GetBool(key);
        }

        public void SetInt(int value)
        {
            EditorPrefs.SetInt(key, value);
        }

        public int GetInt()
        {
            return EditorPrefs.GetInt(key);
        }

        public void SetFloat(float value)
        {
            EditorPrefs.SetFloat(key, value);
        }

        public float GetFloat()
        {
            return EditorPrefs.GetFloat(key);
        }

        public void SetString(string value)
        {
            EditorPrefs.SetString(key, value);
        }

        public string GetString()
        {
            return EditorPrefs.GetString(key);
        }
    }
}
