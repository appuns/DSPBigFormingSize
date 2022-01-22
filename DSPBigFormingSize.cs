using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System;
using System.IO;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using static UnityEngine.GUILayout;
using UnityEngine.Rendering;
using Steamworks;
using rail;
using xiaoye97;

namespace DSPBigFormingSize
{

    [BepInPlugin("Appun.DSP.plugin.BigFormingSize", "DSPBigFormingSize", "0.1.0")]
    [BepInProcess("DSPGAME.exe")]

    public class DSPBigFormingSize : BaseUnityPlugin
    {
        public static ConfigEntry<int> maxFormSize ;
        //public static ConfigEntry<int> maxDismantleSize;
        public static int formSize;
        public static int formSizeSquare;
        //public static int dismantleSize;

        public void Start()
        {
            LogManager.Logger = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            maxFormSize = Config.Bind("General", "maxFormSize", 20, "Maximum size of form brush");
            //maxDismantleSize = Config.Bind("General", "maxDismantleSize", 20, "Maximum size of dismantle brush");

            formSize = maxFormSize.Value;
            formSizeSquare = maxFormSize.Value * maxFormSize.Value;
            //dismantleSize = maxDismantleSize.Value;
        }


        //整地モード：cursorSizeの変更
        [HarmonyPatch(typeof(BuildTool_Reform), "ReformAction")]
        class Transpiler_replace1
        {
            [HarmonyTranspiler]

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i].operand == 10)
                    {
                        codes[i].opcode = OpCodes.Ldsfld;
                        codes[i].operand = AccessTools.Field(typeof(DSPBigFormingSize), nameof(DSPBigFormingSize.formSize)); ;
                    }
                }
                return codes.AsEnumerable();
                //}

            }
        }

        //cursorPointsaとcursorIndicesの配列サイズの変更
        [HarmonyPatch(typeof(BuildTool_Reform), MethodType.Constructor)]
        class Transpiler_replace2
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i].operand == 100)
                    {
                        codes[i].opcode = OpCodes.Ldsfld;
                        codes[i].operand = AccessTools.Field(typeof(DSPBigFormingSize), nameof(DSPBigFormingSize.formSizeSquare)); ;
                    }
                }
                return codes.AsEnumerable();
            }
        }

        //解体モード：cursorSizeの変更
        //[HarmonyPatch(typeof(BuildTool_Dismantle), "DeterminePreviews")]
        //class Transpiler_replace3
        //{
        //    [HarmonyTranspiler]
        //    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        //    {
        //        var codes = new List<CodeInstruction>(instructions);
        //        for (int i = 0; i < codes.Count; i++)
        //        {
        //            if (codes[i].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i].operand == 11)
        //            {
        //                codes[i].opcode = OpCodes.Ldsfld;
        //                codes[i].operand = AccessTools.Field(typeof(DSPBigFormingSize), nameof(DSPBigFormingSize.dismantleSize)); ;
        //            }
        //        }
        //        return codes.AsEnumerable();
        //    }
        //}


    }

    public class LogManager
    {
        public static ManualLogSource Logger;
    }

}