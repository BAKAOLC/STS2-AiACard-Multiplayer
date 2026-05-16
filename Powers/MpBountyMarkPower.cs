using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>
    ///     赏金标记：<see cref="IsInstanced" /> 为 true，每次施加为独立实例（与轨道、独白等一致）；消灭时按该实例的
    ///     GoldCap 与当时的生命上限结算一次金币。击杀赏金主要在 <see cref="AfterDamageGiven" /> 结算（致命伤会跳过
    ///     <see cref="AfterDamageReceived" />，且部分环境下 <see cref="AfterDeath" /> 不可靠）；无伤害来源时在
    ///     <see cref="AfterDeath" /> 用战史兜底。
    /// </summary>
    public sealed class MpBountyMarkPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override bool IsInstanced => true;

        public override int DisplayAmount => GetInternalData<Data>().GoldCap;

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpBountyMarkPower, Const.Paths.PowerIcons.MpBountyMarkPower);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("BountyGoldCap", 0m)];

        protected override object? InitInternalData()
        {
            return new Data();
        }

        public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
        {
            var d = GetInternalData<Data>();
            d.GoldCap = ReadCapFromCard(cardSource);
            return Task.CompletedTask;
        }

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            SyncCapToDynamicVars();
            return Task.CompletedTask;
        }

        public override Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result,
            ValueProp props, Creature target, CardModel? cardSource)
        {
            if (target != Owner || !target.IsMonster || !result.WasTargetKilled)
                return Task.CompletedTask;

            var killer = dealer?.Player;
            if (killer == null)
                return Task.CompletedTask;

            TryGrantBounty(killer);
            return Task.CompletedTask;
        }

        public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
            bool wasRemovalPrevented, float deathAnimLength)
        {
            if (creature != Owner || !creature.IsMonster || wasRemovalPrevented)
                return Task.CompletedTask;

            var d = GetInternalData<Data>();
            if (d.PaidOut)
                return Task.CompletedTask;

            var history = CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>()
                .LastOrDefault(e => e.Receiver == creature && e.Result.WasTargetKilled);
            var killer = history?.Dealer?.Player;
            if (killer == null)
                return Task.CompletedTask;

            TryGrantBounty(killer);
            return Task.CompletedTask;
        }

        private void TryGrantBounty(Player killer)
        {
            var d = GetInternalData<Data>();
            if (d.PaidOut)
                return;

            var pool = Math.Min(Owner.MaxHp, d.GoldCap);
            if (pool <= 0)
                return;

            var runState = Owner.CombatState?.RunState;
            if (runState?.CurrentRoom is not CombatRoom combatRoom)
                return;

            d.PaidOut = true;
            combatRoom.AddExtraReward(killer, new GoldReward(pool, killer));
        }

        private static int ReadCapFromCard(CardModel? cardSource)
        {
            if (cardSource == null) return 0;
            if (!cardSource.DynamicVars.TryGetValue("BountyGoldCap", out var v)) return 0;

            return (int)v.BaseValue;
        }

        private void SyncCapToDynamicVars()
        {
            var d = GetInternalData<Data>();
            DynamicVars["BountyGoldCap"].BaseValue = d.GoldCap;
            InvokeDisplayAmountChanged();
        }

        private sealed class Data
        {
            public int GoldCap;
            public bool PaidOut;
        }
    }
}
