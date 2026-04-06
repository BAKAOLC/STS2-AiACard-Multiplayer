using System.Collections.Concurrent;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace STS2_AiACard_Multiplayer.Utils
{
    /// <summary>玩家死亡时暂存各战斗牌堆的卡牌引用与顺序，复活时原样塞回牌堆。</summary>
    internal static class MpPlayerDeathCardStash
    {
        private static readonly ConcurrentDictionary<ulong, Snapshot> Stash = new();

        /// <summary>不改动牌堆；快照里弃牌=原弃牌+原手牌顺序，复活时手牌空、牌回弃牌堆。</summary>
        internal static void StoreSnapshot(ulong netId, PlayerCombatState pcs)
        {
            var hand = pcs.Hand.Cards.ToList();
            var discard = pcs.DiscardPile.Cards.ToList();
            var mergedDiscard = new List<CardModel>(discard.Count + hand.Count);
            mergedDiscard.AddRange(discard);
            mergedDiscard.AddRange(hand);

            Stash[netId] = new(
                [],
                pcs.DrawPile.Cards.ToList(),
                mergedDiscard,
                pcs.ExhaustPile.Cards.ToList(),
                pcs.PlayPile.Cards.ToList());
        }

        internal static bool TryConsumeAndRestore(Player player, PlayerCombatState pcs)
        {
            if (!Stash.TryGetValue(player.NetId, out var snap))
                return false;

            if (pcs.AllCards.Any())
                return false;

            Stash.TryRemove(player.NetId, out _);

            Restore(snap.Hand, pcs.Hand);
            Restore(snap.Draw, pcs.DrawPile);
            Restore(snap.Discard, pcs.DiscardPile);
            Restore(snap.Exhaust, pcs.ExhaustPile);
            Restore(snap.Play, pcs.PlayPile);
            return true;
        }

        private static void Restore(IReadOnlyList<CardModel> order, CardPile pile)
        {
            foreach (var c in order)
            {
                c.HasBeenRemovedFromState = false;
                pile.AddInternal(c);
            }

            for (var i = 0; i < order.Count; i++)
                pile.InvokeCardAddFinished();
        }

        private sealed class Snapshot(
            IReadOnlyList<CardModel> hand,
            IReadOnlyList<CardModel> draw,
            IReadOnlyList<CardModel> discard,
            IReadOnlyList<CardModel> exhaust,
            IReadOnlyList<CardModel> play)
        {
            internal IReadOnlyList<CardModel> Hand { get; } = hand;
            internal IReadOnlyList<CardModel> Draw { get; } = draw;
            internal IReadOnlyList<CardModel> Discard { get; } = discard;
            internal IReadOnlyList<CardModel> Exhaust { get; } = exhaust;
            internal IReadOnlyList<CardModel> Play { get; } = play;
        }
    }
}
