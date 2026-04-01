using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Defect
{
    /// <summary>不要就给我：每名其他玩家随机耗尽手牌中一张牌，你抽等量张牌。</summary>
    public sealed class MpStatusVacuum() : ModCardTemplate(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var rng = Owner.RunState.Rng.CombatCardGeneration;
            var n = 0;
            foreach (var p in CombatState.Players.Where(x => x != Owner))
            {
                var hand = MpHelpers.SnapshotHand(p);
                if (hand.Count == 0)
                {
                    continue;
                }

                var pick = hand[rng.NextInt(hand.Count)];
                await CardCmd.Exhaust(choiceContext, pick);
                n++;
            }

            if (n > 0)
            {
                await CardPileCmd.Draw(choiceContext, n, Owner);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
