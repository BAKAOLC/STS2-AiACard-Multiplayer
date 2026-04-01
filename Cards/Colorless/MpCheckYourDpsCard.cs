using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_AiACard_Multiplayer.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>查你DPS：本阶段目标若未打出攻击则受罚，你获得下回合资源。</summary>
    public sealed class MpCheckYourDpsCard()
        : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Common, TargetType.AnyAlly)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<MpCheckDpsPower>(0)];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var t = MpHelpers.RequireTargetPlayer(cardPlay);
            await PowerCmd.Apply<MpCheckDpsPower>(t.Creature, DynamicVars["MpCheckDpsPower"].BaseValue, Owner.Creature,
                this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["MpCheckDpsPower"].UpgradeValueBy(1m);
        }
    }
}
