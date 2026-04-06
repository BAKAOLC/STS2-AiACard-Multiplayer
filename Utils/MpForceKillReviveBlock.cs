using System.Collections.Concurrent;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace STS2_AiACard_Multiplayer.Utils
{
    /// <summary>强制击杀（如无厌沙虫吞噬）的玩家本战内不可作为尸体说话目标；不影响战斗结束等自然回血。</summary>
    internal static class MpForceKillReviveBlock
    {
        private static readonly ConcurrentDictionary<ulong, byte> BlockedNetIds = new();

        internal static void Mark(ulong netId)
        {
            BlockedNetIds[netId] = 0;
        }

        internal static bool IsBlocked(ulong netId)
        {
            return BlockedNetIds.ContainsKey(netId);
        }

        /// <summary>尸体说话等：不可选为目标的强制击杀玩家。</summary>
        internal static bool IsBlockedCorpse(Creature creature)
        {
            return creature is { IsPlayer: true, IsDead: true, Player: { } p } && IsBlocked(p.NetId);
        }

        internal static void ClearForNewCombat()
        {
            BlockedNetIds.Clear();
        }
    }
}
