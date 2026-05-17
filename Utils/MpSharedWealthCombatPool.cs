using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Combat;

namespace STS2_AiACard_Multiplayer.Utils
{
    internal sealed class MpSharedWealthCombatPool
    {
        public int PendingTotalGold;
        public bool Distributed;

        private static readonly ConditionalWeakTable<CombatState, MpSharedWealthCombatPool> Pools = new();

        public static MpSharedWealthCombatPool GetOrCreate(CombatState combatState)
        {
            if (!Pools.TryGetValue(combatState, out var pool))
            {
                pool = new MpSharedWealthCombatPool();
                Pools.Add(combatState, pool);
            }

            return pool;
        }
    }
}
