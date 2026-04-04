using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>
    ///     对参与能力结算的攻击伤害（<see cref="ValueProp.Move" /> 且非 <see cref="ValueProp.Unpowered" />）按 2^Amount 倍增；敌方回合结束时移除。
    /// </summary>
    public sealed class MpDoubleDamageTakenPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpDoubleDamageTakenPower, Const.Paths.PowerIcons.MpDoubleDamageTakenPower);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<MpTigerStudyFlowPower>()];

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
            Creature? dealer, CardModel? cardSource)
        {
            if (target != Owner) return 1m;

            if (!props.IsPoweredAttack()) return 1m;

            return (decimal)Math.Pow(2.0, Amount);
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == CombatSide.Enemy) await PowerCmd.Remove(this);
        }
    }
}
