using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace MIM40kFactions.Ideology
{
    public class RitualBehaviorWorker_RubricofAhriman : RitualBehaviorWorker
    {
        public RitualBehaviorWorker_RubricofAhriman()
        {
        }
        public RitualBehaviorWorker_RubricofAhriman(RitualBehaviorDef def)
            : base(def)
        {
        }

        public override string CanStartRitualNow(TargetInfo target, Precept_Ritual ritual, Pawn selectedPawn = null, Dictionary<string, Pawn> forcedForRole = null)
        {
            Precept_Role precept_Role = ritual.ideo.RolesListForReading.FirstOrDefault((Precept_Role r) => r.def == PreceptDefOf.IdeoRole_Moralist);
            if (precept_Role == null)
            {
                return null;
            }

            if (precept_Role.ChosenPawnSingle() == null)
            {
                return "CantStartRitualRoleNotAssigned".Translate(precept_Role.LabelCap);
            }

            bool flag = false;
            foreach (Pawn item in target.Map.mapPawns.FreeColonistsAndPrisonersSpawned)
            {
                if (ValidateConvertee(item, precept_Role.ChosenPawnSingle(), throwMessages: false))
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                return "CantStartRitualNoConvertee".Translate(precept_Role.ChosenPawnSingle().Ideo.name);
            }

            return base.CanStartRitualNow(target, ritual, selectedPawn, forcedForRole);
        }

        public static bool ValidateConvertee(Pawn convertee, Pawn leader, bool throwMessages)
        {
            if (convertee == null)
            {
                return false;
            }

            if (!AbilityUtility.ValidateNoMentalState(convertee, throwMessages, null))
            {
                return false;
            }

            if (!AbilityUtility.ValidateCanWalk(convertee, throwMessages, null))
            {
                return false;
            }

            if (!ValidateMustBeMale(convertee, throwMessages, null))
            {
                return false;
            }

            if (!ValidateMustNotBeSpaceMarine(convertee, throwMessages, null))
            {
                return false;
            }

            return true;
        }

        public override void PostCleanup(LordJob_Ritual ritual)
        {
            Pawn warden = ritual.PawnWithRole("moralist");
            Pawn pawn = ritual.PawnWithRole("convertee");
            if (pawn.IsPrisonerOfColony)
            {
                WorkGiver_Warden_TakeToBed.TryTakePrisonerToBed(pawn, warden);
                pawn.guest.WaitInsteadOfEscapingFor(1250);
            }
        }

        private static bool ValidateMustBeMale(Pawn targetPawn, bool showMessages, Ability ability)
        {
            if (targetPawn.gender != Gender.Male)
            {
                if (showMessages)
                {
                    SendPostProcessedMessage("EMWH_MustBeMale".Translate(), targetPawn, ability);
                }

                return false;
            }

            return true;
        }
        private static bool ValidateMustNotBeSpaceMarine(Pawn targetPawn, bool showMessages, Ability ability)
        {
            if (!ModsConfig.IsActive("emitbreaker.MIM.WH40k.Core"))
                return true;
            if (targetPawn.genes.HasGene(DefDatabase<GeneDef>.GetNamed("EMSM_AdeptusAstartes_BodySize")))
            {
                if (showMessages)
                {
                    SendPostProcessedMessage("EMWH_MustNotBeSpaceMarine".Translate(), targetPawn, ability);
                }

                return false;
            }

            return true;
        }
        private static void SendPostProcessedMessage(string message, LookTargets targets, Ability ability)
        {
            if (ability != null)
            {
                message = "CannotUseAbility".Translate(ability.def.label) + ": " + message;
            }

            Messages.Message(message, targets, MessageTypeDefOf.RejectInput, historical: false);
        }
    }
}
