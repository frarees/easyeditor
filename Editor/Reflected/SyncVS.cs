namespace EasyEditor.Reflected
{
    using System;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Profiling;

    [InitializeOnLoad]
    internal static class SyncVS
    {
        public static bool IsValid { get; private set; }

        private static Type type;
        private static MethodInfo createIfDoesntExist;
        private static MethodInfo syncIfFirstFileOpenSinceDomainLoad;

        private static bool GetMembers()
        {
            type = typeof(Editor).
                Assembly.
                GetType("UnityEditor.SyncVS");

            Debug.Assert(type != null);
            if (type == null)
            {
                return false;
            }

            createIfDoesntExist = type.
                GetMethod("CreateIfDoesntExist",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                null, CallingConventions.Any, new Type[] { }, null);

            Debug.Assert(createIfDoesntExist != null);
            if (createIfDoesntExist == null)
            {
                return false;
            }

            syncIfFirstFileOpenSinceDomainLoad = type.
                GetMethod("SyncIfFirstFileOpenSinceDomainLoad",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                null, CallingConventions.Any, new Type[] { }, null);

            Debug.Assert(syncIfFirstFileOpenSinceDomainLoad != null);
            return syncIfFirstFileOpenSinceDomainLoad != null;
        }

        static SyncVS()
        {
            IsValid = GetMembers();
        }

        public static void CreateIfDoesntExist()
        {
            _ = createIfDoesntExist.Invoke(null, new object[] { });
        }

        public static void SyncIfFirstFileOpenSinceDomainLoad()
        {
            _ = syncIfFirstFileOpenSinceDomainLoad.Invoke(null, new object[] { });
        }
    }
}

