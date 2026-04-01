using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Regent
{
    /// <summary>辉星同享：消耗星辉；每名玩家将一张已升级粒子障壁加入手牌。</summary>
    public sealed class MpSharedStarWall() : ModCardTemplate(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override int CanonicalStarCost => 2;

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead)
                {
                    continue;
                }

                var wall = MpHelpers.CreateCard<ParticleWall>(CombatState, p, upgraded: true);
                await MpHelpers.AddToHand(choiceContext, wall);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
