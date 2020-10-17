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
        }

        private static void OnBeforeAssemblyReload()
        {
            IsReloading = true;
        }

        private static void OnAfterAssemblyReload()
        {
            IsReloading = false;
        }

        public static void Sync()
        {
            if (!SyncVS.IsValid)
            {
                return;
            }

            Profiler.BeginSample("Sync Solution");
            SyncVS.CreateIfDoesntExist();
            SyncVS.SyncIfFirstFileOpenSinceDomainLoad();
            Profiler.EndSample();
        }
    }
}

