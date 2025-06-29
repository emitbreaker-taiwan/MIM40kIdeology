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
    public class ThoughtWorker_Precept_Psylink_LeaderResentmentAlt : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.RoyaltyActive)
                return false;

            if (p.Faction == null || !p.IsColonist || p.IsQuestLodger() || !p.RaceProps.Humanlike)
                return false;

            if (p.Faction.leader == null || !p.Faction.leader.RaceProps.Humanlike)
                return false;

            Pawn pawn = p.Faction?.leader;
            if (!pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
                return true;
            return false;
        }
    }
}