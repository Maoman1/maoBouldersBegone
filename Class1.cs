﻿using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace maoBouldersBegone
{
    [BepInPlugin("mao.bouldersbegone", "Boulders Begone", "1.3")]
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
        private float colliderCheckTimer = 0f;
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
            colliderCheckTimer += Time.deltaTime;
            if (colliderCheckTimer >= 3f)
            {
                colliderCheckTimer = 0f;
                RunColliderPurge();
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
                //RunBoulderPurge();
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                //RunColliderPurge();
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                RunBoulderPurge();
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
                var name = obj.name.ToLowerInvariant();
                var rootName = obj.transform.root?.name.ToLowerInvariant() ?? name;

                // Skip anything related to Jotun at any level
                if (name.Contains("jotun") || rootName.Contains("jotun"))
                {
                    //Plugin.Log.LogInfo($"[BBG] Skipped Jotun collider: {obj.name} (root: {rootName})");
                    continue;
                }

                // Skip any colliders attached to objects with a renderer (visible objects)
                if (obj.GetComponent<Renderer>() != null || obj.GetComponentInChildren<Renderer>() != null)
                    continue;

                // Heuristic: if it's named like a rock but has no visuals, kill it
                if (name.Contains("rock") || name.Contains("boulder") || name.Contains("static"))
                {
                    UnityEngine.Object.Destroy(obj);
                    count++;
                    //Plugin.Log.LogInfo($"[BouldersBegone] Removed collider: {obj.name}");
                }
            }

            //Plugin.Log.LogInfo($"[Boulders Begone] {count} standalone colliders destroyed.");
        }




        private bool IsRealBoulder(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            string lower = name.ToLowerInvariant();

            // Exclusions (make sure these never get deleted)
            if (lower.Contains("jotun") || lower.Contains("crockpot") || lower.Contains("campfire") || lower.Contains("crock"))
                return false;

            // Inclusions (rocks we want to purge)
            if (lower.StartsWith("rock_") ||
                lower.Contains("rockforest") ||
                lower.Contains("rockshoreline") ||
                lower.Contains("rockhighlands") ||
                lower.Contains("rock5") ||
                lower.Contains("navcollider_rock") ||
                lower.Contains("cave_rock"))
                return true;

            return false;
        }

        private void RunBoulderPurge()
        {
            int count = 0;
            var objects = UnityEngine.Object.FindObjectsOfType<UnityEngine.GameObject>();

            foreach (var obj in objects)
            {
                if (IsRealBoulder(obj.name))
                {
                    Plugin.Log.LogInfo($"[Boulders Begone] Smiting to Oblivion: {obj.name}");
                    obj.transform.position += new UnityEngine.Vector3(0, -1000f, 0);

                    var renderers = obj.GetComponentsInChildren<UnityEngine.Renderer>(true);
                    foreach (var r in renderers)
                        r.enabled = false;

                    count++;
                }
            }

            //Plugin.Log.LogInfo($"[Boulders Begone] {count} rocks banished to the void.");
        }
    }
}
