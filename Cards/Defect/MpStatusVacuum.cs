using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Defect
{
    /// <summary>不要就给我：消耗所有玩家的状态牌，在你手牌中生成等量随机状态牌。</summary>
    public sealed class MpStatusVacuum() : ModCardTemplate(1, CardType.Skill, CardRarity.Common, TargetType.Self)
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

            var rng = Owner.RunState.Rng.CombatCardGeneration;
            for (var i = 0; i < n; i++)
            {
                var created = RandomStatusCard(CombatState, Owner, rng.NextInt(5));
                await MpHelpers.AddToHand(choiceContext, created);
            }
        }

        private static CardModel RandomStatusCard(CombatState cs, Player owner, int idx)
        {
            return idx switch
            {
                0 => cs.CreateCard<Dazed>(owner),
                1 => cs.CreateCard<Wound>(owner),
                2 => cs.CreateCard<Burn>(owner),
                3 => cs.CreateCard<Slimed>(owner),
                _ => cs.CreateCard<MindRot>(owner),
            };
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
