using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Grammar;
using System.Globalization;
using UnityEngine.Bindings;
using UnityEngine.Scripting;


namespace MIM40kFactions.Ideology
{

    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        private static readonly Type patchType = typeof(HarmonyPatches);
 
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("rimworld.emitbreaker.MIM.WH40k.Ideology");
            harmony.Patch(AccessTools.Method(typeof(Pawn_StyleObserverTracker), "StyleObserverTick"), new HarmonyMethod(typeof(EMWH_IdeologyPatch).GetMethod("StyleObserverTickPrefix"))); 
         }
    }

    public class EMWH_IdeologyPatch
    {
        [HarmonyPrefix]
        public static void StyleObserverTickPrefix(Pawn_StyleObserverTracker __instance)
        {
            if (ModsConfig.IsActive("emitbreaker.MIM.WH40k.CSM.TS") && __instance.pawn.health.hediffSet.HasHediff(HediffDef.Named("EMTS_RubricofAhriman")))
                return;
            if (ModsConfig.BiotechActive && ModsConfig.IsActive("emitbreaker.MIM.WH40k.CSM.DG") && __instance.pawn.genes.Xenotype == Named("EMCH_Poxwalker"))
                return;
        }
        private static XenotypeDef Named(string defName)
        {
            return DefDatabase<XenotypeDef>.GetNamed(defName);
        }
    }
}
