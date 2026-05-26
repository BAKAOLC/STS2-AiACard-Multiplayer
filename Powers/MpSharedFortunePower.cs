using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    public sealed class MpSharedFortunePower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override bool IsInstanced => true;

        public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpSharedFortunePower, Const.Paths.PowerIcons.MpSharedFortunePower);

        public override Task AfterCombatEnd(CombatRoom room)
        {
            var player = Owner.Player;
            if (player == null || Amount <= 0)
                return Task.CompletedTask;

            room.AddExtraReward(player, new GoldReward(Amount, player));

            return Task.CompletedTask;
        }
    }
}
