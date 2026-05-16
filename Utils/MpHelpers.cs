using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace STS2_AiACard_Multiplayer.Utils
{
    internal static class MpHelpers
    {
        public static Player RequireTargetPlayer(CardPlay cardPlay)
        {
            var c = cardPlay.Target ?? throw new InvalidOperationException("Expected a targeted player.");
            return c.Player ?? throw new InvalidOperationException("Target is not a player.");
        }

        /// <summary>按类型生成卡牌；<paramref name="upgraded" /> 为 true 时升级为 + 版。</summary>
        public static T CreateCard<T>(CombatState combatState, Player owner, bool upgraded) where T : CardModel
        {
            var card = combatState.CreateCard<T>(owner);
            if (upgraded) CardCmd.Upgrade(card);

            return card;
        }

        /// <summary>按模板（如卡池随机到的 canonical）生成卡牌；<paramref name="upgraded" /> 为 true 时升级。</summary>
        public static CardModel CreateCard(CombatState combatState, Player owner, CardModel canonical, bool upgraded)
        {
            ArgumentNullException.ThrowIfNull(canonical);
            var card = combatState.CreateCard(canonical, owner);
            if (upgraded) CardCmd.Upgrade(card);

            return card;
        }

        /// <summary>
        ///     将战斗中已创建且尚未入堆的卡牌加入指定战斗牌堆（<see cref="PileType.Draw" /> / <see cref="PileType.Hand" /> /
        ///     <see cref="PileType.Discard" /> 等）；
        ///     经 <see cref="CardPileCmd.AddGeneratedCardsToCombat" /> 写入历史，便于多人同步。
        ///     <paramref name="previewPileAdd" />：为 true 时对本地所属玩家播放 <see cref="CardCmd.PreviewCardPileAdd" />（飞入牌堆动画；加入手牌时一般为
        ///     false，与 <c>GunkUp</c> / <c>GlimpseBeyond</c> 等一致）。
        /// </summary>
        public static async Task<IReadOnlyList<CardPileAddResult>> AddGeneratedCardsToCombatPile(
            IEnumerable<CardModel> cards,
            PileType pileType,
            CardPilePosition position = CardPilePosition.Bottom,
            bool previewPileAdd = false)
        {
            var list = cards.ToList();
            if (list.Count == 0) return Array.Empty<CardPileAddResult>();

            var results = await CardPileCmd.AddGeneratedCardsToCombat(list, pileType, true, position);
            if (previewPileAdd) CardCmd.PreviewCardPileAdd(results);

            return results;
        }

        /// <inheritdoc cref="AddGeneratedCardsToCombatPile" />
        public static Task<IReadOnlyList<CardPileAddResult>> AddGeneratedCardToCombatPile(
            CardModel card,
            PileType pileType,
            CardPilePosition position = CardPilePosition.Bottom,
            bool previewPileAdd = false)
        {
            return AddGeneratedCardsToCombatPile(new[] { card }, pileType, position, previewPileAdd);
        }

        public static async Task AddToHand(PlayerChoiceContext ctx, CardModel card)
        {
            await AddGeneratedCardToCombatPile(card, PileType.Hand);
        }

        public static async Task DealHpLoss(PlayerChoiceContext ctx, Creature target, decimal amount, CardModel source)
        {
            await CreatureCmd.Damage(ctx, target, amount, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
                source);
        }

        public static IReadOnlyList<CardModel> SnapshotHand(Player player)
        {
            var pcs = player.PlayerCombatState;
            ArgumentNullException.ThrowIfNull(pcs);
            return pcs.Hand.Cards.ToList();
        }

        public static async Task DrawUntilHandFull(PlayerChoiceContext ctx, Player player)
        {
            var pcs = player.PlayerCombatState;
            ArgumentNullException.ThrowIfNull(pcs);
            while (pcs.Hand.Cards.Count < Const.CombatHandMax)
            {
                var before = pcs.Hand.Cards.Count;
                await CardPileCmd.Draw(ctx, 1, player);
                if (pcs.Hand.Cards.Count == before) break;
            }
        }

        public static void MakeEtherealEnergyOneThisTurn(CardModel card)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
            card.EnergyCost.SetThisTurnOrUntilPlayed(1, true);
        }

        /// <summary>战斗牌堆全空时，从 Run 卡组克隆进抽牌堆并洗牌。</summary>
        public static void RepopulateCombatPilesFromDeckIfEmpty(Player player, CombatState combatState)
        {
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(combatState);

            var pcs = player.PlayerCombatState;
            if (pcs == null || pcs.AllCards.Any()) return;

            var rng = player.RunState.Rng.Shuffle;
            foreach (var item in player.Deck.Cards.ToList())
            {
                var cardModel = combatState.CloneCard(item);
                cardModel.DeckVersion = item;
                pcs.DrawPile.AddInternal(cardModel);
            }

            pcs.DrawPile.RandomizeOrderInternal(player, rng, combatState);
        }
    }
}
