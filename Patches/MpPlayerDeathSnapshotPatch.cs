using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2_AiACard_Multiplayer.Patches
{
    /// <summary>玩家死亡处理开始时快照其战斗牌堆，供复活时按引用还原。</summary>
    public sealed class MpPlayerDeathSnapshotPatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.player.death_card_snapshot";

        public static string Description => "死亡时快照战斗牌堆";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CombatManager), nameof(CombatManager.HandlePlayerDeath), [typeof(Player)])];
        }

        public static void Prefix(Player player)
        {
            if (!CombatManager.Instance.IsInProgress) return;
            var pcs = player.PlayerCombatState;
            if (pcs == null) return;

            if (MpForceKillReviveBlock.IsBlocked(player.NetId)) return;

            MpPlayerDeathCardStash.StoreSnapshot(player.NetId, pcs);
        }
    }
}
