using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Necrobinder
{
    /// <summary>尸体说话：对敌人造成伤害，并对其他玩家造成生命损失。</summary>
    public sealed class MpCorpseSpeaks() : ModCardTemplate(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            ArgumentNullException.ThrowIfNull(CombatState);
            var dmg = IsUpgraded ? 16m : 12m;
            await DamageCmd.Attack(dmg)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            var loss = IsUpgraded ? 5m : 3m;
            foreach (var p in CombatState.Players.Where(x => x != Owner && !x.Creature.IsDead))
            {
                await MpHelpers.DealHpLoss(choiceContext, p.Creature, loss, this);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
