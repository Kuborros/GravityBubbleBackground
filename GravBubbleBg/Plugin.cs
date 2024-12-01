using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using UnityEngine;

namespace GravBubbleBg
{
    [BepInPlugin("com.kuborro.plugins.fp2.gravbubblebg", MyPluginInfo.PLUGIN_NAME, "1.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static AssetBundle moddedBundle;

        internal static new ManualLogSource Logger;
        internal static GameObject background,background_noshader;
        internal static Texture palette,palette_baku;

        private static ConfigEntry<bool> configUseShader;
        private static ConfigEntry<bool> configBlinkLights;

        private void Awake()
        {
            Logger = base.Logger;

            string assetPath = Path.Combine(Path.GetFullPath("."), "mod_overrides");
            moddedBundle = AssetBundle.LoadFromFile(Path.Combine(assetPath, "gravitybackground.assets"));
            if (moddedBundle == null)
            {
                Logger.LogError("Failed to load AssetBundle! Mod cannot work without it, exiting. Please reinstall it.");
                return;
            }

            configUseShader = Config.Bind("General",
                                    "Use Shader",
                                    true,
                                    "Enable a shader that cycles the light in the background. Disable to get extra performance on low-end machines.");
             
            configBlinkLights = Config.Bind("General",
                                    "Broken Lights Effect",
                                    true,
                                    "Enable broken lights effect after Bakunawa launches. Disable if the effect is too distracting.");
            

            background = moddedBundle.LoadAsset<GameObject>("GravityBubble_City");
            background_noshader = moddedBundle.LoadAsset<GameObject>("GravityBubble_City_NoShader");
            palette = moddedBundle.LoadAsset<Texture>("palette");
            palette_baku = moddedBundle.LoadAsset<Texture>("palette_broken");

            Harmony.CreateAndPatchAll(typeof(PatchGravityBubble));
            Harmony.CreateAndPatchAll(typeof(PatchBakuLaunch));
        }

        class PatchGravityBubble
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(FPStage), "Start", MethodType.Normal)]
            static void Postfix(string ___stageName)
            {
                if (___stageName == "Gravity Bubble")
                {
                    background.GetComponent<SpriteRenderer>().material.SetTexture("colorMap", palette);

                    GameObject bg;
                    if (configUseShader.Value)
                        bg = background;
                    else
                        bg = background_noshader;

                    foreach (SpriteRenderer backgrounds in FindObjectsOfType<SpriteRenderer>())
                    {
                        if (backgrounds.name == "Background")
                        {
                            GameObject bg0 = Instantiate(bg, backgrounds.transform);
                            bg0.SetActive(true);
                        }
                        if (backgrounds.name == "Background (1)")
                        {
                            GameObject bg1 = Instantiate(bg, backgrounds.transform);
                            bg1.SetActive(true);
                        }
                        if (backgrounds.name == "Background (2)")
                        {
                            GameObject bg2 = Instantiate(bg, backgrounds.transform);
                            bg2.SetActive(true);
                        }
                    }
                }
            }
        }

        class PatchBakuLaunch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(BossBakunawa), "State_Start", MethodType.Normal)]
            static void Postfix()
            {
                if (configBlinkLights.Value && configUseShader.Value)
                    background.GetComponent<SpriteRenderer>().material.SetTexture("colorMap", palette_baku);
            }
        }

    }
}
