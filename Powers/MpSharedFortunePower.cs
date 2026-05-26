using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    public sealed class MpSharedFortunePower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override bool IsInstanced => true;

        public override int DisplayAmount => GetInternalData<Data>().TotalGold;

        public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpSharedFortunePower, Const.Paths.PowerIcons.MpSharedFortunePower);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("SharedFortuneTotalGold", 0m)];

        protected override object? InitInternalData()
        {
            return new Data();
        }

        public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
        {
            GetInternalData<Data>().TotalGold = MpSharedWealthFortuneApplyContext.SharedFortuneTotalGold ?? 0;
            return Task.CompletedTask;
        }

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            SyncDisplayVars();
            return Task.CompletedTask;
        }

        public override Task AfterCombatEnd(CombatRoom room)
        {
            var player = Owner.Player;
            if (player == null || Amount <= 0)
                return Task.CompletedTask;

            room.AddExtraReward(player, new GoldReward(Amount, player));

            return Task.CompletedTask;
        }

        private void SyncDisplayVars()
        {
            DynamicVars["SharedFortuneTotalGold"].BaseValue = GetInternalData<Data>().TotalGold;
            InvokeDisplayAmountChanged();
        }

        private sealed class Data
        {
            public int TotalGold;
        }
    }
}
