using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Ironclad
{
    /// <summary>放干他人血：对一名玩家造成生命损失，其获得能量，你回复等量生命。</summary>
    public sealed class MpDrainOthersDry() : ModCardTemplate(2, CardType.Skill, CardRarity.Rare, TargetType.AnyPlayer)
    {
        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var loss = IsUpgraded ? 10m : 8m;
            var energy = IsUpgraded ? 3 : 2;
            await MpHelpers.DealHpLoss(choiceContext, target.Creature, loss, this);
            await PlayerCmd.GainEnergy(energy, target);
            await CreatureCmd.Heal(Owner.Creature, loss);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
