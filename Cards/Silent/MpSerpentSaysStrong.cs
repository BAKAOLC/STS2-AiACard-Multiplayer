using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Silent
{
    /// <summary>我说蛇咬很强：每名存活玩家将若干张蛇咬加入手牌。</summary>
    public sealed class MpSerpentSaysStrong() : ModCardTemplate(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var n = IsUpgraded ? 3 : 2;
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead)
                {
                    continue;
                }

                for (var i = 0; i < n; i++)
                {
                    var shiv = CombatState.CreateCard<Shiv>(p);
                    await MpHelpers.AddToHand(choiceContext, shiv);
                }
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
