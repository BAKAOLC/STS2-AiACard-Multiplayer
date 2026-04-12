using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Silent
{
    /// <summary>你话太多了：目标获得格挡且本回合无法抽牌；你将手牌抽至上限。</summary>
    public sealed class MpTooMuchTalk()
        : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Common, TargetType.AnyAlly)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new BlockVar(8m, ValueProp.Move)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpTooMuchTalk, Const.Paths.CardPortraits.MpTooMuchTalk);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<NoDrawPower>()];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            await CreatureCmd.GainBlock(target.Creature, DynamicVars.Block, cardPlay);
            await PowerCmd.Apply<NoDrawPower>(target.Creature, 1, Owner.Creature, this);
            await MpHelpers.DrawUntilHandFull(choiceContext, Owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(6m);
        }
    }
}
