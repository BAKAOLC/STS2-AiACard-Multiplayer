using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>赌怪悬赏标记：该非玩家单位被消灭时，最后一击的玩家获得等同于其生命上限的金币（不超过 Amount）。</summary>
    public sealed class MpBountyMarkPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
            bool wasRemovalPrevented, float deathAnimLength)
        {
            if (creature != Owner || !creature.IsMonster) return;

            var pool = Math.Min(creature.MaxHp, Amount);
            if (pool <= 0) return;

            var history = CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>()
                .LastOrDefault(e => e.Receiver == creature && e.Result.WasTargetKilled);
            var killer = history?.Dealer?.Player;
            if (killer == null || killer.Creature.IsDead) return;

            await PlayerCmd.GainGold(pool, killer);
        }
    }
}
