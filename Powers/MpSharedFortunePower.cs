using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>有福同享：战斗结束时持有者获得金币。</summary>
    public sealed class MpSharedFortunePower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Const.PlaceholderPowerIcon;

        public override Task AfterCombatEnd(CombatRoom room)
        {
            return Owner.Player == null
                ? Task.CompletedTask
                : PlayerCmd.GainGold(Amount, Owner.Player);
        }
    }
}
