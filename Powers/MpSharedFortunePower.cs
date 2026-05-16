using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>「有福同享」牌对应的能力。层数表示该玩家本场战斗结算中可领取的额外金币数额；金币在战斗奖励阶段领取，不由本能力在战斗结束时直接发放。持有者死亡时不在死亡清算中移除此能力。</summary>
    public sealed class MpSharedFortunePower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        /// <summary>多次打出时各次施加保留为独立实例，层数不在同一图标上合并。</summary>
        public override bool IsInstanced => true;

        public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpSharedFortunePower, Const.Paths.PowerIcons.MpSharedFortunePower);
    }
}
