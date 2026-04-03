using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Silent
{
    /// <summary>我说蛇咬很强：用蛇咬填满所有玩家手牌。</summary>
    public sealed class MpSerpentSaysStrong()
        : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpSerpentSaysStrong, Const.Paths.CardPortraits.MpSerpentSaysStrong);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<Snakebite>(IsUpgraded);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                var pcs = p.PlayerCombatState!;
                while (pcs.Hand.Cards.Count < Const.CombatHandMax)
                {
                    var bite = MpHelpers.CreateCard<Snakebite>(CombatState, p, IsUpgraded);
                    await MpHelpers.AddToHand(choiceContext, bite);
                }
            }
        }
    }
}
