using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace STS2_AiACard_Multiplayer.Utils
{
    internal static class MpRequiredFirstPlayCards
    {
        private static readonly ConditionalWeakTable<CardModel, Marker> MarkedCards = new();

        public static void Mark(CardModel card)
        {
            MarkedCards.GetValue(card, static _ => new Marker());
        }

        public static bool IsMarked(CardModel card)
        {
            return MarkedCards.TryGetValue(card, out _);
        }

        public static bool HasMarkedCardInHand(Player player)
        {
            var pcs = player.PlayerCombatState;
            return pcs?.Hand.Cards.Any(IsMarked) == true;
        }

        private sealed class Marker;
    }
}
