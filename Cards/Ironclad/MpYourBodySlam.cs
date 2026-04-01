using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Ironclad
{
    /// <summary>你来撞击：对一名玩家造成等于其当前格挡的伤害。</summary>
    public sealed class MpYourBodySlam() : ModCardTemplate(0, CardType.Attack, CardRarity.Common, TargetType.AnyPlayer)
    {
        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var dmg = target.Creature.Block + (IsUpgraded ? 5m : 0m);
            await DamageCmd.Attack(dmg)
                .FromCard(this)
                .Targeting(target.Creature)
                .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
                .Execute(choiceContext);
        }
    }
}
