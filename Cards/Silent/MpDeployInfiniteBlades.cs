using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Silent
{
    /// <summary>开把刀出来：每名玩家将无尽刀刃与幻影之刃置入手牌（升级后为升级版本）。</summary>
    public sealed class MpDeployInfiniteBlades()
        : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<InfiniteBlades>(IsUpgraded)
                .Concat(HoverTipFactory.FromCardWithCardHoverTips<PhantomBlades>(IsUpgraded));

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                var ib = MpHelpers.CreateCard<InfiniteBlades>(CombatState, p, IsUpgraded);
                await MpHelpers.AddToHand(choiceContext, ib);

                var phantom = MpHelpers.CreateCard<PhantomBlades>(CombatState, p, IsUpgraded);
                await MpHelpers.AddToHand(choiceContext, phantom);
            }
        }
    }
}
