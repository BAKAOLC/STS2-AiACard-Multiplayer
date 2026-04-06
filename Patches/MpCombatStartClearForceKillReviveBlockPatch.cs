using MegaCrit.Sts2.Core.Combat;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2_AiACard_Multiplayer.Patches
{
    public sealed class MpCombatStartClearForceKillReviveBlockPatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.combat.start_clear_force_kill_revive_block";

        public static string Description => "新战斗开始时清除强制击杀复活封锁";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CombatManager), nameof(CombatManager.StartCombatInternal))];
        }

        public static void Prefix()
        {
            MpForceKillReviveBlock.ClearForNewCombat();
        }
    }
}
