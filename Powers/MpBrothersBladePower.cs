using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>是兄弟就来砍它：另一名玩家打出君王之剑时，将你在弃牌/抽牌/消耗堆中的君王之剑移回手牌（不新生成）。</summary>
    public sealed class MpBrothersBladePower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<SovereignBlade>();

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card is not SovereignBlade) return;

            var ownerPlayer = Owner.Player;
            if (ownerPlayer == null) return;

            var actor = cardPlay.Card.Owner;
            if (actor == ownerPlayer) return;

            var blade = FindRetrievableBlade(ownerPlayer);
            if (blade == null) return;

            await CardPileCmd.Add(blade, PileType.Hand);
        }

        /// <summary>优先弃牌堆，其次抽牌堆，最后消耗堆；不含手牌与打出中。</summary>
        private static SovereignBlade? FindRetrievableBlade(Player player)
        {
            var pcs = player.PlayerCombatState;
            if (pcs == null) return null;

            return pcs.AllCards
                .OfType<SovereignBlade>()
                .Where(b => b.Pile != null && b.Pile.Type is not (PileType.Hand or PileType.Play))
                .OrderBy(b => PileRetrievePriority(b.Pile!.Type))
                .FirstOrDefault();
        }

        private static int PileRetrievePriority(PileType t)
        {
            return t switch
            {
                PileType.Discard => 0,
                PileType.Draw => 1,
                PileType.Exhaust => 2,
                _ => 99,
            };
        }
    }
}
