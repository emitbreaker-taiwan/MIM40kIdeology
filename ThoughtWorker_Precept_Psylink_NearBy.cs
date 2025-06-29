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
    public class ThoughtWorker_Precept_Psylink_NearBy : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.RoyaltyActive)
                return ThoughtState.Inactive;

            if (p.Faction == null || !p.IsColonist || !p.RaceProps.Humanlike)
                return ThoughtState.Inactive;

            List<Pawn> pawnlist = GenRadial.RadialDistinctThingsAround(p.Position, p.Map, 15f, true).OfType<Pawn>().ToList();
            pawnlist.RemoveAll(pawn => !pawn.RaceProps.Humanlike);

            int psykerNearBy = 0;

            foreach (Pawn psyker in pawnlist)
            {
                if (psyker != p || !psyker.IsQuestLodger())
                {
                    if (psyker.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
                    {
                        psykerNearBy++;
                    }
                }
            }

            if (psykerNearBy >= 0)
                return ThoughtState.ActiveAtStage(psykerNearBy);

            return false;
        }
    }
}