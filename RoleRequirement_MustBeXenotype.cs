using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MIM40kFactions.Ideology
{
    public class RoleRequirement_MustBeXenotype : RoleRequirement
    {
        [MayRequireBiotech]
        public List<XenotypeDef> allowedXenotypes;
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
                    labelCached = "EMWH_MustHaveXenotype";
            }
            return labelCached.Translate();
        }

        public override bool Met(Pawn p, Precept_Role role)
        {
            if (ModsConfig.BiotechActive == true)
            {
                if (allowedXenotypes == null)
                    return true;
                foreach (XenotypeDef x in allowedXenotypes)
                {
                    if (p.genes.Xenotype == x)
                        return true;
                }
                return false;
            }
            return true;
        }
    }
}
