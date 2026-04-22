using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Regent
{
    /// <summary>仆从出击：目标玩家手牌全部变为仆从打击。</summary>
    public sealed class MpMinionRush() : MpOnlyModCardTemplate(3, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpMinionRush, Const.Paths.CardPortraits.MpMinionRush);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<MinionStrike>(false);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var hand = MpHelpers.SnapshotHand(target).ToList();
            foreach (var c in hand)
            {
                var rep = MpHelpers.CreateCard<MinionStrike>(CombatState, target, false);
                await CardCmd.Transform(c, rep);
            }

            if (!IsUpgraded || hand.Count == 0)
                return;

            for (var i = 0; i < hand.Count; i++)
            {
                var rep = MpHelpers.CreateCard<MinionStrike>(CombatState, Owner, false);
                await MpHelpers.AddToHand(choiceContext, rep);
            }
        }
    }
}
