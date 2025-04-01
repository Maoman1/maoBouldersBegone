using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace maoBouldersBegone
{
    [BepInPlugin("mao.bouldersbegone", "Boulders Begone", "1.0.0")]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource Log;

        public override void Load()
        {
            Log = base.Log;
            Log.LogInfo("[Boulders Begone] Plugin loaded. Press F6 in-world to smite terrain rocks.");

            ClassInjector.RegisterTypeInIl2Cpp<BoulderNuker>();

            var go = new GameObject("BoulderNuker");
            go.AddComponent<BoulderNuker>();
            Object.DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    public class BoulderNuker : MonoBehaviour
    {
        public BoulderNuker(System.IntPtr ptr) : base(ptr) { }

        bool inWorld = false;
        bool timerStarted = false;
        float timeSinceLock = 0f;
        Vector2 lastMouse = Vector2.zero;

        void Update()
        {
            // Wait until cursor is locked before starting the timer
            if (!timerStarted && Cursor.lockState == CursorLockMode.Locked)
            {
                timerStarted = true;
                timeSinceLock = 0f; // reset in case it flickered earlier
                Plugin.Log.LogInfo("[Boulders Begone] Cursor lock detected. Starting delay countdown.");
            }

            if (timerStarted && !inWorld)
            {
                timeSinceLock += UnityEngine.Time.deltaTime;

                if (timeSinceLock >= 10f)
                {
                    Vector2 currentMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                    if (currentMouse != Vector2.zero && currentMouse != lastMouse)
                    {
                        inWorld = true;
                        Plugin.Log.LogInfo("[Boulders Begone] In-world confirmed after lock + delay + movement.");
                        RunBoulderPurge();
                    }

                    lastMouse = currentMouse;
                }
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                RunBoulderPurge();
            }
        }

        private bool IsRealBoulder(string name)
        {
            // Exclude false positives first
            if (name.ToLowerInvariant().Contains("crockpot") || name.ToLowerInvariant().Contains("campfire") || name.ToLowerInvariant().Contains("crock"))
                return false;

            // Match only known terrain rock types
            return
                name.ToLowerInvariant().Contains("rock_") ||
                name.ToLowerInvariant().Contains("rockforest") ||
                name.ToLowerInvariant().Contains("rockshoreline") ||
                name.ToLowerInvariant().Contains("rockhighlands") ||
                name.ToLowerInvariant().Contains("rock5") ||
                name.ToLowerInvariant().Contains("navcollider_rock") ||
                name.ToLowerInvariant().Contains("cave_rock");
        }

        private void RunBoulderPurge()
        {
            Plugin.Log.LogInfo("[Boulders Begone] Purging terrain rocks...");

            int count = 0;
            var objects = UnityEngine.Object.FindObjectsOfType<UnityEngine.GameObject>();

            foreach (var obj in objects)
            {

                if (IsRealBoulder(obj.name))
                {
                    //log what was found and deleted
                    //Plugin.Log.LogInfo($"[RockScan] Found: {obj.name}");
                    
                    obj.SetActive(false);
                    count++;
                }
            }

            Plugin.Log.LogInfo($"[Boulders Begone] Disabled {count} terrain objects.");
        }
    }
}
