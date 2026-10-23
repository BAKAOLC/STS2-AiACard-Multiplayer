using System.Diagnostics;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;
using STS2RitsuLib.Patching.Models;

namespace STS2_AiACard_Multiplayer.Patches
{
    public sealed class RegentColorlessGenerationPatch : IPatchMethod
    {
        public static string PatchId => Const.ModId + ".regent.colorless_generation";
        public static string Description => "Reduce multiplayer colorless card weight for vanilla Regent generators";
        public static ModPatchTarget[] GetTargets() =>
            [new(typeof(CardFactory), nameof(CardFactory.GetDistinctForCombat),
                [typeof(Player), typeof(IEnumerable<CardModel>), typeof(int), typeof(Rng)])];

        public static bool Prefix(Player player, IEnumerable<CardModel> cards, int count, Rng rng,
            ref IEnumerable<CardModel> __result)
        {
            if (player.Character is not Regent || player.RunState.Players.Count < 2 || !HasVanillaCardSource())
                return true;
            var options = cards.ToList();
            if (options.Count == 0 || options.Any(card => card.Pool is not ColorlessCardPool) ||
                !options.Any(IsMultiplayerCard))
                return true;
            var testOverride = TestRngInjector.ConsumeCombatCardGenerationOverride();
            if (testOverride != null)
            {
                __result = testOverride;
                return false;
            }
            options = CardFactory.FilterForCombat(options)
                .Where(card => card.MultiplayerConstraint != CardMultiplayerConstraint.SingleplayerOnly).ToList();
            __result = SelectWeighted(options, count, rng)
                .Select(card => player.Creature.CombatState!.CreateCard(card, player)).ToArray();
            return false;
        }

        private static bool IsMultiplayerCard(CardModel card) => card.GetType().Assembly == typeof(Main).Assembly;

        internal static List<CardModel> SelectWeighted(IEnumerable<CardModel> cards, int count, Rng rng)
        {
            var options = cards.Distinct().ToList();
            var selected = new List<CardModel>();
            var multiplayerCount = 0;
            while (selected.Count < count && options.Count > 0)
            {
                var roll = rng.NextInt(options.Sum(Weight));
                var index = 0;
                while ((roll -= Weight(options[index])) >= 0)
                    index++;
                var card = options[index];
                selected.Add(card);
                options.RemoveAt(index);
                if (IsMultiplayerCard(card))
                    multiplayerCount++;
                if (count == 3 && multiplayerCount >= 1)
                    options.RemoveAll(IsMultiplayerCard);
            }
            return selected;

            static int Weight(CardModel card) => IsMultiplayerCard(card) ? 50 : 100;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool HasVanillaCardSource()
        {
            foreach (var frame in new StackTrace().GetFrames())
                for (var type = frame.GetMethod()?.DeclaringType; type != null; type = type.DeclaringType)
                {
                    if (typeof(CardModel).IsAssignableFrom(type))
                        return type.Assembly == typeof(CardModel).Assembly;
                    if (typeof(PowerModel).IsAssignableFrom(type) || typeof(PotionModel).IsAssignableFrom(type) ||
                        typeof(RelicModel).IsAssignableFrom(type))
                        return false;
                }
            return false;
        }
    }
}
