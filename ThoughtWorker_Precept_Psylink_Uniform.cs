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
    public class ThoughtWorker_Precept_Psylink_Uniform : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.RoyaltyActive)
                return ThoughtState.Inactive;

            if (p.Faction == null || !p.IsColonist || !p.RaceProps.Humanlike)
            {
                return ThoughtState.Inactive;
            }

            List<Pawn> list = p.Map.mapPawns.SpawnedPawnsInFaction(p.Faction);
            int num = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (((list[i] != p && list[i].RaceProps.Humanlike) || list[i].DevelopmentalStage.Baby()) && !list[i].IsSlave && !list[i].IsQuestLodger())
                {
                    if (list[i].health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
                    {
                        return false;
                    }

                    num++;
                }
            }

            return num > 0;
        }
    }
}