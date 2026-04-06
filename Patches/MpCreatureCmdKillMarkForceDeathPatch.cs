using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2_AiACard_Multiplayer.Patches
{
    /// <summary>与沙虫吞噬等一致：强制击杀玩家时标记，禁止本战内复活与死亡牌堆快照。</summary>
    public sealed class MpCreatureCmdKillMarkForceDeathPatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.creature_cmd.kill_mark_force_death";

        public static string Description => "强制击杀玩家时标记不可复活";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CreatureCmd), nameof(CreatureCmd.Kill), [typeof(Creature), typeof(bool)])];
        }

        public static void Prefix(Creature creature, bool force)
        {
            if (!force || creature is not { IsPlayer: true, Player: { } p }) return;
            MpForceKillReviveBlock.Mark(p.NetId);
        }
    }
}
