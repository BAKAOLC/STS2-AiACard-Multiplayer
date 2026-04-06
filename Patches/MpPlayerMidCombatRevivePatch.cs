using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2_AiACard_Multiplayer.Patches
{
    /// <summary>战斗中玩家从死亡恢复生命时：启用 hooks、解除结束回合锁定、有死亡快照则还原牌堆否则从卡组重填。</summary>
    public sealed class MpPlayerMidCombatRevivePatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.player.mid_combat_revive_restock";

        public static string Description => "战斗中复活：hooks、结束回合、快照牌堆或卡组重填";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(Creature), nameof(Creature.SetCurrentHpInternal), [typeof(decimal)])];
        }

        public static void Prefix(Creature __instance, out bool __state)
        {
            __state = __instance.IsDead;
        }

        public static void Postfix(Creature __instance, bool __state)
        {
            if (!__state || __instance.IsDead || !__instance.IsPlayer) return;

            var player = __instance.Player;
            if (player == null) return;

            player.ActivateHooks();

            if (!CombatManager.Instance.IsInProgress) return;

            var cm = CombatManager.Instance;
            if (cm.IsPlayerReadyToEndTurn(player)) cm.UndoReadyToEndTurn(player);

            var pcs = player.PlayerCombatState;
            if (pcs == null) return;

            var cs = __instance.CombatState;
            var restored = cs != null && MpPlayerDeathCardStash.TryConsumeAndRestore(player, pcs);

            if (!restored && !pcs.AllCards.Any() && cs != null)
                MpHelpers.RepopulateCombatPilesFromDeckIfEmpty(player, cs);
        }
    }
}
