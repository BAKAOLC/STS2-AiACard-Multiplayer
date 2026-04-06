using MegaCrit.Sts2.Core.Entities.Creatures;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2_AiACard_Multiplayer.Patches
{
    /// <summary>打出「有福同享」时临时允许已死亡玩家仍满足 CanReceivePowers，以便完成施加；该能力在死亡清算中默认不会被剥掉。</summary>
    public sealed class MpSharedWealthFortuneCanReceivePowersPatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.creature.can_receive_powers.shared_fortune";

        public static string Description => "有福同享：打出时死亡玩家仍可挂上能力";

        public static ModPatchTarget[] GetTargets() =>
            [new(typeof(Creature), "get_CanReceivePowers")];

        public static void Postfix(Creature __instance, ref bool __result)
        {
            if (__result)
                return;
            if (!MpSharedWealthFortuneApplyContext.AllowFortunePowerOnDeadPlayers)
                return;
            if (__instance is not { IsPlayer: true, CombatState: not null })
                return;
            __result = true;
        }
    }
}
