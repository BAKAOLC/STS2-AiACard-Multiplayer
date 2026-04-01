using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Defect
{
    /// <summary>不要就给我：消耗所有玩家的状态牌，在你手牌中生成等量随机状态牌。</summary>
    public sealed class MpStatusVacuum() : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var n = 0;
            foreach (var p in CombatState.Players)
            foreach (var c in p.PlayerCombatState!.AllCards.ToList())
            {
                if (c.Type != CardType.Status) continue;

                await CardCmd.Exhaust(choiceContext, c);
                n++;
            }

            if (n == 0)
                return;

            var templates = StatusTemplatesEligibleForCombatRandom(Owner);
            if (templates.Count == 0)
                return;

            var rng = Owner.RunState.Rng.CombatCardGeneration;
            for (var i = 0; i < n; i++)
            {
                var template = rng.NextItem(templates);
                if (template == null)
                    break;

                var created = CombatState.CreateCard(template, Owner);
                await MpHelpers.AddToHand(choiceContext, created);
            }
        }

        /// <summary>
        ///     与 <see cref="CardFactory.GetForCombat" /> 一致：全库 <see cref="CardType.Status" />，经
        ///     <see cref="CardFactory.FilterForCombat" /> 与人数相关的 <see cref="CardMultiplayerConstraint" /> 过滤。
        /// </summary>
        private static List<CardModel> StatusTemplatesEligibleForCombatRandom(Player owner)
        {
            var fromDb = ModelDb.AllCards.Where(c => c.Type == CardType.Status);
            var filtered = CardFactory.FilterForCombat(fromDb).ToList();
            var run = owner.RunState;
            if (run.Players.Count > 1)
                filtered = filtered
                    .Where(c => c.MultiplayerConstraint != CardMultiplayerConstraint.SingleplayerOnly)
                    .ToList();
            else
                filtered = filtered
                    .Where(c => c.MultiplayerConstraint != CardMultiplayerConstraint.MultiplayerOnly)
                    .ToList();

            return filtered;
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
