using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;

namespace MIM40kFactions.Ideology
{
    public class ThoughtWorker_Precept_Psylink : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.RoyaltyActive)
                return ThoughtState.Inactive;

            if (p.Faction == null || !p.IsColonist || !p.RaceProps.Humanlike)
            {
                return ThoughtState.Inactive;
            }

            int num = 0;
            int num2 = 0;
            List<Pawn> list = p.Map.mapPawns.SpawnedPawnsInFaction(p.Faction);
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].IsQuestLodger() && list[i].RaceProps.Humanlike && !list[i].IsSlave && !list[i].IsPrisoner && !list[i].DevelopmentalStage.Baby())
                {
                    num2++;
                    if (list[i] != p && list[i].health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
                    {
                        num++;
                    }
                }
            }

            if (num == 0)
            {
                return ThoughtState.Inactive;
            }

            return ThoughtState.ActiveAtStage(Mathf.RoundToInt((float)num / (float)(num2 - 1) * (float)(def.stages.Count - 1)));
        }
    }
}