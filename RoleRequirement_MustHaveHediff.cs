using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MIM40kFactions.Ideology
{
    public class RoleRequirement_MustHaveHediff : RoleRequirement
    {
        public List<HediffDef> requiredHediffs;
        [NoTranslate]
        private string labelCached;

        public override string GetLabel(Precept_Role role)
        {
            ErrorMessageExtension modExtension = role.def.GetModExtension<ErrorMessageExtension>();
            if (labelCached == null)
            {
                if (modExtension != null && !modExtension.keyedMessage.NullOrEmpty())
                    labelCached = modExtension.keyedMessage;
                else
                    labelCached = "EMWH_MustHaveHediff";
            }
            return labelCached.Translate();
        }

        public override bool Met(Pawn p, Precept_Role role)
        {
            if (requiredHediffs == null)
                return true;

            foreach (HediffDef x in requiredHediffs)
            {
                if (p.health.hediffSet.HasHediff(x))
                    return true;
            }
            return false;
        }
    }
}
