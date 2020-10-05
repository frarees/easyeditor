namespace EasyEditor
{
    using EasyEditor.Reflected;
    using UnityEditor;
    using UnityEngine.Profiling;

    [InitializeOnLoad]
    internal static class SyncUtil
    {
        public static bool IsReloading { get; private set; }

        static SyncUtil()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;

            if (!SyncVS.IsValid)
            {
                Preferences.Settings.autoSync.SetBool(false);
            }
        }

        private static void OnBeforeAssemblyReload()
        {
            IsReloading = true;
        }

        private static void OnAfterAssemblyReload()
        {
            IsReloading = false;

            if (Preferences.Settings.autoSync.GetBool())
            {
                Sync();
            }
        }

        public static void Sync()
        {
            Profiler.BeginSample("Sync Solution");
            SyncVS.CreateIfDoesntExist();
            SyncVS.SyncIfFirstFileOpenSinceDomainLoad();
            Profiler.EndSample();
        }
    }
}

