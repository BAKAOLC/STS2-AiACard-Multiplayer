using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2_AiACard_Multiplayer.Patches
{
    public sealed class MpRequiredFirstPlayCardCanPlayPatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.required_first_play.can_play";

        public static string Description => "必须优先打出：存在被标记手牌时，其他手牌不可打出";

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(CardModel), nameof(CardModel.CanPlay),
                    [typeof(UnplayableReason).MakeByRefType(), typeof(AbstractModel).MakeByRefType()]),
            ];
        }

        public static void Postfix(CardModel __instance, ref UnplayableReason reason, ref AbstractModel? preventer,
            ref bool __result)
        {
            if (__instance.Owner?.PlayerCombatState == null)
                return;
            if (MpRequiredFirstPlayCards.IsMarked(__instance))
                return;
            if (!MpRequiredFirstPlayCards.HasMarkedCardInHand(__instance.Owner))
                return;

            reason |= UnplayableReason.BlockedByCardLogic;
            __result = false;
        }
    }
}
