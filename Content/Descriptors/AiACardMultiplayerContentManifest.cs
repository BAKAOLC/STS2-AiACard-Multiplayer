using MegaCrit.Sts2.Core.Models.CardPools;
using STS2_AiACard_Multiplayer.Cards.Colorless;
using STS2_AiACard_Multiplayer.Cards.Defect;
using STS2_AiACard_Multiplayer.Cards.Ironclad;
using STS2_AiACard_Multiplayer.Cards.Necrobinder;
using STS2_AiACard_Multiplayer.Cards.Regent;
using STS2_AiACard_Multiplayer.Cards.Silent;
using STS2_AiACard_Multiplayer.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Content.Descriptors
{
    internal static class AiACardMultiplayerContentManifest
    {
        public static IReadOnlyList<IContentRegistrationEntry> ContentEntries { get; } =
        [
            new PowerRegistrationEntry<MpBrothersBladePower>(),
            new PowerRegistrationEntry<MpSerpentBrothersPower>(),
            new PowerRegistrationEntry<MpTigerStudyFlowPower>(),
            new PowerRegistrationEntry<MpLoseEnergyNextTurnPower>(),
            new PowerRegistrationEntry<MpCheckDpsPower>(),
            new PowerRegistrationEntry<MpBountyMarkPower>(),
            new PowerRegistrationEntry<MpDoubleDamageTakenPower>(),
            new PowerRegistrationEntry<MpHangoverPower>(),
            new PowerRegistrationEntry<MpPerEnergySelfChannelPower>(),
            new CardRegistrationEntry<IroncladCardPool, MpDrainOthersDry>(),
            new CardRegistrationEntry<IroncladCardPool, MpPassTheFlame>(),
            new CardRegistrationEntry<IroncladCardPool, MpYourBodySlam>(),
            new CardRegistrationEntry<IroncladCardPool, MpEveryoneRedraw>(),
            new CardRegistrationEntry<SilentCardPool, MpNoCardsNoEnergy>(),
            new CardRegistrationEntry<SilentCardPool, MpSerpentSaysStrong>(),
            new CardRegistrationEntry<SilentCardPool, MpDeployInfiniteBlades>(),
            new CardRegistrationEntry<SilentCardPool, MpTooMuchTalk>(),
            new CardRegistrationEntry<RegentCardPool, MpBrothersChopCard>(),
            new CardRegistrationEntry<RegentCardPool, MpOrbitMinecart>(),
            new CardRegistrationEntry<RegentCardPool, MpMinionRush>(),
            new CardRegistrationEntry<RegentCardPool, MpSharedStarWall>(),
            new CardRegistrationEntry<NecrobinderCardPool, MpSharedDoomSlow>(),
            new CardRegistrationEntry<NecrobinderCardPool, MpGroupSacrifice>(),
            new CardRegistrationEntry<NecrobinderCardPool, MpBloodMistCard>(),
            new CardRegistrationEntry<NecrobinderCardPool, MpCorpseSpeaks>(),
            new CardRegistrationEntry<NecrobinderCardPool, MpCorpseCurseToken>(),
            new CardRegistrationEntry<DefectCardPool, MpBiasedPartyCard>(),
            new CardRegistrationEntry<DefectCardPool, MpShockGift>(),
            new CardRegistrationEntry<DefectCardPool, MpStatusVacuum>(),
            new CardRegistrationEntry<DefectCardPool, MpBrainDock>(),
            new CardRegistrationEntry<ColorlessCardPool, MpAllOnMe>(),
            new CardRegistrationEntry<ColorlessCardPool, MpFourWardBrothers>(),
            new CardRegistrationEntry<ColorlessCardPool, MpSerpentBrothersCard>(),
            new CardRegistrationEntry<ColorlessCardPool, MpChaosEchoParty>(),
            new CardRegistrationEntry<ColorlessCardPool, MpTigerStudyCard>(),
            new CardRegistrationEntry<ColorlessCardPool, MpSharedWealth>(),
            new CardRegistrationEntry<ColorlessCardPool, MpCheckYourDpsCard>(),
            new CardRegistrationEntry<ColorlessCardPool, MpWriggle>(),
            new CardRegistrationEntry<ColorlessCardPool, MpGamblingCard>(),
            new CardRegistrationEntry<ColorlessCardPool, MpCoolSummerDoom>(),
        ];
    }
}
