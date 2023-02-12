using UnityModManagerNet;
using System;
using System.Text;
using System.Reflection;
using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Localization;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Localization.Shared;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Kingmaker.Visual.CharacterSystem;
using System.Collections.Generic;
using HarmonyLib;
using static HarmonyLib.AccessTools;
using Steamworks;
using UnityEngine.SceneManagement;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Area;
using UnityEngine.Assertions.Must;

namespace CreatureSelect
{
    public class Main
    {

        public static Settings Settings;
        public static bool enabled;

        public static System.Collections.Generic.IEnumerator<Kingmaker.Blueprints.BlueprintUnit> BPE = null;
        public static string GUID = "";

        public static UnityModManager.ModEntry.ModLogger logger = null;

        public static int lastMinCR = -1;
        public static int lastMaxCR = -1;
        public static int index = 0;
        static List<BlueprintUnit> Choices;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                Settings = Settings.Load<Settings>(modEntry);
                modEntry.OnToggle = OnToggle;
                modEntry.OnGUI = OnGUI;
                modEntry.OnSaveGUI = OnSaveGUI;
                logger = modEntry.Logger;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            return true;
        }

        static void ApplyBlueprint()
        {
            if (GUID == "")
            {
                Settings.TextBox = "Nothing Selected";
                GUID = "";
                return;
            }
            BlueprintUnit Blue = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>(GUID);

            // Alternative Beginning: HouseAtTheEdgeOfTime_Enter_Preset
            // BlueprintAreaPreset Preset = ResourcesLibrary.TryGetBlueprint<BlueprintAreaPreset>("14e4706ead1665c4db019d87f6a64e93");

            // Proper normal Game Beginning
            BlueprintAreaPreset Preset = Game.Instance.BlueprintRoot.NewGamePreset;
            Preset.PlayerCharacter = Blue;
            Game.Instance.Player.MainCharacter.Value.Descriptor.Alignment.Set(Blue.Alignment);
            Game.Instance.LoadNewGame(Preset);
        }

        static void MakeListValid()
        {
            int low = Int32.Parse(Settings.MinCR);
            int high = Int32.Parse(Settings.MaxCR);
            if (lastMinCR != low || lastMaxCR != high)
            {
                var filteredprints = ResourcesLibrary.GetBlueprints<BlueprintUnit>().Where(blueprint => blueprint.CR >= low).Where(blueprint => blueprint.CR <= high);
                Choices = filteredprints.ToList().OrderBy(bu => bu.CharacterName).ToList();
                index = 0;
                lastMinCR = low;
                lastMaxCR = high;
            }
        }
        static void UpdateChoice(int newindex)
        {
            if (Choices.Count == 0) return;
            if (newindex >= Choices.Count)
                newindex = 0;
            if (newindex < 0)
                newindex = Choices.Count - 1;
            index = newindex;
            Settings.TextBox = string.Format("{0}/{1}: {2}", index + 1, Choices.Count + 1, Choices[index].CharacterName);
            GUID = Choices[index].AssetGuid;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply", GUILayout.Width(75)))
            {
                ApplyBlueprint();
            }
            if (GUILayout.Button("Prev", GUILayout.Width(75)))
            {
                MakeListValid();
                UpdateChoice(index - 1);
            }
            if (GUILayout.Button("Next", GUILayout.Width(75)))
            {
                MakeListValid();
                UpdateChoice(index + 1);
            }
            if (GUILayout.Button("Reset", GUILayout.Width(75)))
            {
                MakeListValid();
                UpdateChoice(0);
            }
            if(GUILayout.Button("Fix Alignment", GUILayout.Width(150)))
            {
                Game.Instance.Player.MainCharacter.Value.Descriptor.Alignment.Set(Game.Instance.Player.MainCharacter.Value.Descriptor.Blueprint.Alignment);
            }
            GUILayout.Label("Min CR", GUILayout.ExpandWidth(false));
            Settings.MinCR = GUILayout.TextField(Settings.MinCR, GUILayout.Width(50));
            GUILayout.Label("Max CR", GUILayout.ExpandWidth(false));
            Settings.MaxCR = GUILayout.TextField(Settings.MaxCR, GUILayout.Width(50));
            GUILayout.EndHorizontal();
            Settings.TextBox = GUILayout.TextArea(Settings.TextBox, GUILayout.MinHeight(50));
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }

        [HarmonyPatch(typeof(LibraryScriptableObject), "LoadDictionary")]
        static class LibraryScriptableObject_LoadDictionary_Patch
        {
            static void Postfix()
            {
                BPE = ResourcesLibrary.GetLoadedResourcesOfType<BlueprintUnit>().GetEnumerator();
            }
        }
    }
}
