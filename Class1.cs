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
            Log.LogInfo("[Boulders Begone] Plugin loaded.");

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
            if (!timerStarted && Cursor.lockState == CursorLockMode.Locked)
            {
                timerStarted = true;
                timeSinceLock = 0f;
            }

            if (timerStarted && !inWorld)
            {
                timeSinceLock += UnityEngine.Time.deltaTime;

                if (timeSinceLock >= 12f)
                {
                    Vector2 currentMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                    if (currentMouse != Vector2.zero && currentMouse != lastMouse)
                    {
                        inWorld = true;
                        RunBoulderPurge();
                        RunColliderPurge();
                    }

                    lastMouse = currentMouse;
                }
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                RunBoulderPurge();
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                RunColliderPurge();
            }
        }

        private void RunColliderPurge()
        {
            int count = 0;
            var colliders = UnityEngine.Object.FindObjectsOfType<UnityEngine.Collider>();

            foreach (var col in colliders)
            {
                var obj = col.gameObject;
                string name = obj.name.ToLowerInvariant();

                if (name.Contains("rock") || name.Contains("boulder") || name.Contains("static"))
                {
                    UnityEngine.Object.Destroy(obj);
                    count++;
                }
            }

            Plugin.Log.LogInfo($"[Boulders Begone] {count} standalone colliders destroyed.");
        }

        private bool IsRealBoulder(string name)
        {
            string lower = name.ToLowerInvariant();
            if (lower.Contains("crockpot") || lower.Contains("campfire") || lower.Contains("crock"))
                return false;

            return
                lower.Contains("rock_") ||
                lower.Contains("rockforest") ||
                lower.Contains("rockshoreline") ||
                lower.Contains("rockhighlands") ||
                lower.Contains("rock5") ||
                lower.Contains("navcollider_rock") ||
                lower.Contains("cave_rock");
        }

        private void RunBoulderPurge()
        {
            int count = 0;
            var objects = UnityEngine.Object.FindObjectsOfType<UnityEngine.GameObject>();

            foreach (var obj in objects)
            {
                if (IsRealBoulder(obj.name))
                {
                    obj.transform.position += new UnityEngine.Vector3(0, -1000f, 0);

                    var renderers = obj.GetComponentsInChildren<UnityEngine.Renderer>(true);
                    foreach (var r in renderers)
                        r.enabled = false;

                    count++;
                }
            }

            Plugin.Log.LogInfo($"[Boulders Begone] {count} rocks banished to the void.");
        }
    }
}
