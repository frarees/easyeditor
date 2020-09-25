namespace EasyEditor.Reflected
{
    using System;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    internal static class MonoInstallationFinder
    {
        public static bool IsValid { get; private set; }

        private static Type type;
        private static MethodInfo getProfilesDirectory;
        private static FieldInfo monoInstallation;
        private static FieldInfo monoBleedingEdgeInstallation;

        private static bool GetMembers()
        {
            type = typeof(Editor).
                Assembly.
                GetType("UnityEditor.Utils.MonoInstallationFinder");

            Debug.Assert(type != null);
            if (type == null)
            {
                return false;
            }

            getProfilesDirectory = type.GetMethod("GetProfilesDirectory",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                null, CallingConventions.Any, new Type[] { typeof(string) }, null);

            Debug.Assert(getProfilesDirectory != null);
            if (getProfilesDirectory == null)
            {
                return false;
            }

            monoInstallation = type.GetField("MonoInstallation", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(monoInstallation != null);
            if (monoInstallation == null)
            {
                return false;
            }

            monoBleedingEdgeInstallation = type.GetField("MonoBleedingEdgeInstallation", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(monoBleedingEdgeInstallation != null);
            return monoBleedingEdgeInstallation != null;
        }

        static MonoInstallationFinder()
        {
            IsValid = GetMembers();
        }

        public static string GetProfilesDirectory(string monoInstallation)
        {
            return (string)getProfilesDirectory.Invoke(null, new object[] { monoInstallation });
        }

        public static string MonoInstallation => (string)monoInstallation.GetValue(null);
        public static string MonoBleedingEdgeInstallation => (string)monoBleedingEdgeInstallation.GetValue(null);
    }
}

