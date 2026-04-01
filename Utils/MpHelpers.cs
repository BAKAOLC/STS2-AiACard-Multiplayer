using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_AiACard_Multiplayer;

namespace STS2_AiACard_Multiplayer.Utils
{
    internal static class MpHelpers
    {
        public static Player RequireTargetPlayer(CardPlay cardPlay)
        {
            var c = cardPlay.Target ?? throw new InvalidOperationException("Expected a targeted player.");
            return c.Player ?? throw new InvalidOperationException("Target is not a player.");
        }

        public static CardModel CreateCard<T>(CombatState combatState, Player owner, bool upgraded) where T : CardModel
        {
            var card = combatState.CreateCard<T>(owner);
            if (upgraded)
            {
                CardCmd.Upgrade(card);
            }

            return card;
        }

        public static async Task AddToHand(PlayerChoiceContext ctx, CardModel card)
        {
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, true);
        }

        public static async Task DealHpLoss(PlayerChoiceContext ctx, Creature target, decimal amount, CardModel source)
        {
            await CreatureCmd.Damage(ctx, target, amount, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
                source);
        }

        public static IReadOnlyList<CardModel> SnapshotHand(Player player)
        {
            return player.PlayerCombatState!.Hand.Cards.ToList();
        }

        public static async Task DrawUntilHandFull(PlayerChoiceContext ctx, Player player)
        {
            var pcs = player.PlayerCombatState!;
            while (pcs.Hand.Cards.Count < Const.CombatHandMax)
            {
                await CardPileCmd.Draw(ctx, 1, player);
            }
        }

        public static void MakeEtherealEnergyOneThisTurn(CardModel card)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
            card.EnergyCost.SetThisTurnOrUntilPlayed(1, reduceOnly: true);
        }
    }
}
