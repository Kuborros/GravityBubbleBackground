using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using UnityEngine;

namespace GravBubbleBg
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static AssetBundle moddedBundle;

        internal static new ManualLogSource Logger;
        internal static GameObject background;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            string assetPath = Path.Combine(Path.GetFullPath("."), "mod_overrides");
            moddedBundle = AssetBundle.LoadFromFile(Path.Combine(assetPath, "gravitybackground.assets"));
            if (moddedBundle == null)
            {
                Logger.LogError("Failed to load AssetBundle! Mod cannot work without it, exiting. Please reinstall it.");
                return;
            }

            background = moddedBundle.LoadAsset<GameObject>("GravityBubble_City");

            Harmony.CreateAndPatchAll(typeof(PatchGravityBubble));
        }

        class PatchGravityBubble
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(FPStage), "Start", MethodType.Normal)]
            static void Postfix(string ___stageName)
            {
                if (___stageName == "Gravity Bubble")
                {
                    foreach (SpriteRenderer backgrounds in FindObjectsOfType<SpriteRenderer>())
                    {
                        if (backgrounds.name == "Background")
                        {
                            Logger.LogDebug("Found the bingus: " + backgrounds.name);
                            GameObject bg0 = Instantiate(background, backgrounds.transform);
                            bg0.SetActive(true);
                        }
                        if (backgrounds.name == "Background (1)")
                        {
                            Logger.LogDebug("Found the bingus: " + backgrounds.name);
                            GameObject bg1 = Instantiate(background, backgrounds.transform);
                            bg1.SetActive(true);
                        }
                        if (backgrounds.name == "Background (2)")
                        {
                            Logger.LogDebug("Found the bingus: " + backgrounds.name);
                            GameObject bg2 = Instantiate(background, backgrounds.transform);
                            bg2.SetActive(true);
                        }
                    }
                }
            }
        }

    }
}
