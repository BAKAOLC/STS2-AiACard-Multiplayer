using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    public sealed class MpDoubleAttackDamageThisTurnPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpDoubleDamageTakenPower, Const.Paths.PowerIcons.MpDoubleDamageTakenPower);

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
            Creature? dealer, CardModel? cardSource)
        {
            if (dealer == null || dealer != Owner && !Owner.Pets.Contains<Creature>(dealer))
                return 1m;

            if (!props.IsPoweredAttack() || cardSource == null)
                return 1m;

            return 2m;
        }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (participants.Contains(Owner))
                await PowerCmd.Remove(this);
        }
    }
}
