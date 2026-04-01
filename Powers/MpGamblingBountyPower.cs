using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>赌怪：敌人死亡时，所有玩家按上限分得金币（简化：不计最后一击归属）。</summary>
    public sealed class MpGamblingBountyPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
            bool wasRemovalPrevented, float deathAnimLength)
        {
            if (!creature.IsMonster || creature.CombatState == null) return;

            var cap = Amount;
            var pool = Math.Min(creature.MaxHp, cap);
            if (pool <= 0) return;

            var players = creature.CombatState.Players.Where(p => !p.Creature.IsDead).ToList();
            if (players.Count == 0) return;

            var each = pool / players.Count;
            if (each <= 0) each = 1;

            foreach (var p in players) await PlayerCmd.GainGold(each, p);
        }
    }
}
