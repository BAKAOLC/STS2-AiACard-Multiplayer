using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>
    ///     赏金标记：<see cref="IsInstanced" /> 为 true，每次施加为独立实例（与轨道、独白等一致）；消灭时按该实例的
    ///     GoldCap 与当时的生命上限结算一次金币。
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

        public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
            bool wasRemovalPrevented, float deathAnimLength)
        {
            if (creature != Owner || !creature.IsMonster) return;

            var d = GetInternalData<Data>();
            if (d.GoldCap <= 0) return;

            var pool = Math.Min(creature.MaxHp, d.GoldCap);
            if (pool <= 0) return;

            var history = CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>()
                .LastOrDefault(e => e.Receiver == creature && e.Result.WasTargetKilled);
            var killer = history?.Dealer?.Player;
            if (killer == null || killer.Creature.IsDead) return;

            await PlayerCmd.GainGold(pool, killer);
        }

        private sealed class Data
        {
            public int GoldCap;
        }
    }
}
