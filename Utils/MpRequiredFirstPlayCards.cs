using STS2RitsuLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace STS2_AiACard_Multiplayer.Utils
{
    internal static class MpRequiredFirstPlayCards
    {
        private static readonly AttachedState<CardModel, bool> MarkedCards = new();

        public static void Mark(CardModel card)
        {
            MarkedCards[card] = true;
        }

        public static bool IsMarked(CardModel card)
        {
            return card is Enthralled || MarkedCards.TryGetValue(card, out var marked) && marked;
        }

        public static bool HasMarkedCardInHand(Player player)
        {
            var pcs = player.PlayerCombatState;
            return pcs?.Hand.Cards.Any(IsMarked) == true;
        }

    }
}
