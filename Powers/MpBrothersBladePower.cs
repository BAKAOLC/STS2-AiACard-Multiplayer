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

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpBrothersBladePower, Const.Paths.PowerIcons.MpBrothersBladePower);

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

            SovereignBlade? best = null;
            var bestPriority = int.MaxValue;
            foreach (var b in pcs.AllCards.OfType<SovereignBlade>())
            {
                var pile = b.Pile;
                if (pile == null || pile.Type is PileType.Hand or PileType.Play) continue;

                var pr = PileRetrievePriority(pile.Type);
                if (pr >= bestPriority) continue;

                bestPriority = pr;
                best = b;
            }

            return best;
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
