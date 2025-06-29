using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace MIM40kFactions.Ideology
{
    public class RitualOutcomeEffectWorker_RubricofAhriman : RitualOutcomeEffectWorker_FromQuality
    {
        public override bool SupportsAttachableOutcomeEffect => false;

        public RitualOutcomeEffectWorker_RubricofAhriman()
        {
        }

        public RitualOutcomeEffectWorker_RubricofAhriman(RitualOutcomeEffectDef def)
            : base(def)
        {
        }

        public override void Apply(float progress, Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual)
        {
            float quality = GetQuality(jobRitual, progress);
            RitualOutcomePossibility outcome = GetOutcome(quality, jobRitual);
            LookTargets letterLookTargets = jobRitual.selectedTarget;
            string extraLetterText = null;
            if (jobRitual.Ritual != null)
            {
                ApplyAttachableOutcome(totalPresence, jobRitual, outcome, out extraLetterText, ref letterLookTargets);
            }

            Pawn pawn = jobRitual.PawnWithRole("moralist");
            Pawn pawn2 = jobRitual.PawnWithRole("convertee");
            float ideoCertaintyOffset = outcome.ideoCertaintyOffset;
            if (pawn2.ideo.Ideo != pawn.Faction.ideos.PrimaryIdeo)
            {
                if (ideoCertaintyOffset <= -1f)
                {
                    pawn2.ideo.SetIdeo(pawn.Ideo);
                }
                else
                {
                    pawn2.ideo.OffsetCertainty(ideoCertaintyOffset);
                }
            }

            if (pawn2.ideo.Ideo == pawn.Faction.ideos.PrimaryIdeo)
                DoMutation(pawn, pawn2);

            TaggedString text = outcome.description.Formatted(jobRitual.Ritual.Label).CapitalizeFirst();
            string text2 = def.OutcomeMoodBreakdown(outcome);
            if (!text2.NullOrEmpty())
            {
                text += "\n\n" + text2;
            }

            if (extraLetterText != null)
            {
                text += "\n\n" + extraLetterText;
            }

            text += "\n\n" + OutcomeQualityBreakdownDesc(quality, progress, jobRitual);
            ApplyDevelopmentPoints(jobRitual.Ritual, outcome, out var extraOutcomeDesc);
            if (extraOutcomeDesc != null)
            {
                text += "\n\n" + extraOutcomeDesc;
            }

            Find.LetterStack.ReceiveLetter("OutcomeLetterLabel".Translate(outcome.label.Named("OUTCOMELABEL"), jobRitual.Ritual.Label.Named("RITUALLABEL")), text, outcome.Positive ? LetterDefOf.RitualOutcomePositive : LetterDefOf.RitualOutcomeNegative, letterLookTargets);
        }
        private static void DoMutation(Pawn pawn, Pawn pawn2)
        {
            if (ModsConfig.BiotechActive && ModsConfig.IsActive("emitbreaker.MIM.WH40k.CSM.TS"))
            {
                if ((ModsConfig.RoyaltyActive && pawn2.HasPsylink) || pawn2.story.traits.HasTrait(TraitDef.Named("PsychicSensitivity"), 2) || pawn2.story.traits.HasTrait(TraitDef.Named("PsychicSensitivity"), 1))
                {
                    DoMakeSorcerer(pawn2, pawn.Faction, pawn2.story.Childhood, pawn2.story.Adulthood);
                }
                else
                {
                    DoMakeRubricae(pawn2, pawn.Faction, pawn2.story.Childhood, pawn2.story.Adulthood);
                }
            }
        }
        private static void DoMakeSorcerer(Pawn EMCM_TSvictim, Faction homeFaction, BackstoryDef childhood, BackstoryDef adulthood)
        {
            if (ModsConfig.IsActive("emitbreaker.MIM.WH40k.CSM.TS"))
            {
                BodyPartRecord targetPart = EMCM_TSvictim.health.hediffSet.GetNotMissingParts().FirstOrDefault(part => part.def == BodyPartDefOf.Torso);
                EMCM_TSvictim.health.AddHediff(HediffDef.Named("EMCM_TSGeneSeed"), targetPart);

                if (ModsConfig.BiotechActive == true)
                    EMCM_TSvictim.genes.SetXenotype(DefDatabase<XenotypeDef>.GetNamed("EMCM_HereticAstartes_ThousandSons"));

                EMCM_TSvictim.kindDef = PawnKindDef.Named("EMTS_Mutation_Sorcerer");
                SetPawnBaseStats(EMCM_TSvictim, homeFaction, childhood, adulthood);

                if (ModsConfig.RoyaltyActive == true)
                    ChangePsylinkLevel(EMCM_TSvictim, false);

                BodyPartRecord targetPart2 = EMCM_TSvictim.health.hediffSet.GetBrain();
                EMCM_TSvictim.health.AddHediff(HediffDef.Named("EMTS_WarpPotential"), targetPart2);

                ChangeHediffSeverity(EMCM_TSvictim, EMCM_TSvictim.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("EMCM_TSGeneSeed")));
            }
        }
        private static void SetPawnBaseStats(Pawn EMCM_TSvictim, Faction homeFaction, BackstoryDef childhood, BackstoryDef adulthood)
        {
            if (EMCM_TSvictim.Faction != homeFaction)
                EMCM_TSvictim.SetFaction(homeFaction);
            if (EMCM_TSvictim.story.Childhood != childhood)
                EMCM_TSvictim.story.Childhood = childhood;
            if (EMCM_TSvictim.story.Adulthood != adulthood)
                EMCM_TSvictim.story.Adulthood = adulthood;
        }
        private static void ChangePsylinkLevel(Pawn EMCM_TSvictim, bool sendLetter)
        {
            if (ModsConfig.RoyaltyActive == true)
            {
                Hediff_Psylink mainPsylinkSource = EMCM_TSvictim.GetMainPsylinkSource();
                if (mainPsylinkSource == null)
                {
                    Hediff_Psylink hediffPsylink = (Hediff_Psylink)HediffMaker.MakeHediff(HediffDefOf.PsychicAmplifier, EMCM_TSvictim);
                    try
                    {
                        hediffPsylink.suppressPostAddLetter = !sendLetter;
                        EMCM_TSvictim.health.AddHediff(hediffPsylink, EMCM_TSvictim.health.hediffSet.GetBrain());
                    }
                    finally
                    {
                        hediffPsylink.suppressPostAddLetter = false;
                    }
                    mainPsylinkSource = EMCM_TSvictim.GetMainPsylinkSource();
                    mainPsylinkSource.ChangeLevel(6, sendLetter);
                }
                else
                    mainPsylinkSource.ChangeLevel(6, sendLetter);
            }
        }
        private static void DoMakeRubricae(Pawn EMCM_TSvictim, Faction homeFaction, BackstoryDef childhood, BackstoryDef adulthood)
        {
            if (ModsConfig.IsActive("emitbreaker.MIM.WH40k.CSM.TS"))
            {
                BodyPartRecord targetPart = EMCM_TSvictim.health.hediffSet.GetNotMissingParts().FirstOrDefault(part => part.def == BodyPartDefOf.Torso);
                EMCM_TSvictim.health.AddHediff(HediffDef.Named("EMCM_TSGeneSeed"), targetPart);

                if (ModsConfig.BiotechActive == true)
                    EMCM_TSvictim.genes.SetXenotype(DefDatabase<XenotypeDef>.GetNamed("EMCM_HereticAstartes"));

                EMCM_TSvictim.kindDef = PawnKindDef.Named("EMTS_Mutation_RubricMarine");
                SetPawnBaseStats(EMCM_TSvictim, homeFaction, childhood, adulthood);
                SetCosmetics(EMCM_TSvictim);

                BodyPartRecord targetPart2 = EMCM_TSvictim.health.hediffSet.GetNotMissingParts().FirstOrDefault(part => part.def == BodyPartDefOf.Torso);
                EMCM_TSvictim.health.AddHediff(HediffDef.Named("EMTS_RubricofAhriman"), targetPart2);

                ChangeHediffSeverity(EMCM_TSvictim, EMCM_TSvictim.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("EMCM_TSGeneSeed")));
            }
        }
        private static void SetCosmetics(Pawn EMCM_TSvictim)
        {
            EMCM_TSvictim.story.hairDef = HairDefOf.Bald;
            EMCM_TSvictim.style.beardDef = BeardDefOf.NoBeard;
            EMCM_TSvictim.style.FaceTattoo = TattooDefOf.NoTattoo_Face;
            EMCM_TSvictim.style.BodyTattoo = TattooDefOf.NoTattoo_Body;
        }
        private static void ChangeHediffSeverity(Pawn EMCM_TSvictim, Hediff hediff)
        {
            Hediff_Level geneSeed = EMCM_TSvictim.health.hediffSet.GetFirstHediffOfDef(hediff.def) as Hediff_Level;
            geneSeed.ChangeLevel((int)geneSeed.def.maxSeverity);
        }
    }
}
