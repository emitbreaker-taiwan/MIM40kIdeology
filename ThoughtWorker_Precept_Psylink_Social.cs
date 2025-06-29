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
    public class ThoughtWorker_Precept_Psylink_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
        {
            if (!ModsConfig.RoyaltyActive)
                return false;

            if (p.Faction == null || !p.IsColonist || !p.RaceProps.Humanlike || !otherPawn.RaceProps.Humanlike)
                return false;

            if (ModsConfig.RoyaltyActive && !p.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
                return otherPawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier);

            return false;
        }
    }
}
