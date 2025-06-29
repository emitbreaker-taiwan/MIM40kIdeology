using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MIM40kFactions.Ideology
{
    public class RoleRequirement_MustBePsycaster : RoleRequirement
    {
        [NoTranslate]
        private string labelCached;

        public override string GetLabel(Precept_Role role)
        {
            if (labelCached == null)
                labelCached = (string)"EMWH_MustBePsycaster".Translate();
            return labelCached;
        }

        public override bool Met(Pawn p, Precept_Role role)
        {
            if (ModsConfig.RoyaltyActive == true && !p.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
                return false;
            return true;
        }
    }
}
