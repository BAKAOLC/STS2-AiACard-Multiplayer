using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Ironclad
{
    /// <summary>放干他人血：目标失去生命、获得能量，你回复等量生命。</summary>
    public sealed class MpDrainOthersDry()
        : MpOnlyModCardTemplate(0, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(6m, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move),
            new HealVar(6m),
        ];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            await MpHelpers.DealHpLoss(choiceContext, target.Creature, DynamicVars.Damage.BaseValue, this);
            await PlayerCmd.GainEnergy(2, target);
            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1m);
            DynamicVars.Heal.UpgradeValueBy(1m);
        }
    }
}
