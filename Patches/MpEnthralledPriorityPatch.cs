using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2_AiACard_Multiplayer.Patches
{
    public sealed class MpEnthralledPriorityPatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.required_first_play.enthralled";
        public static string Description => "执迷与必须优先打出的卡牌共享优先级";

        public static ModPatchTarget[] GetTargets() =>
            [new(typeof(Enthralled), nameof(Enthralled.ShouldPlay), [typeof(CardModel), typeof(AutoPlayType)])];

        public static void Postfix(CardModel card, ref bool __result)
        {
            if (MpRequiredFirstPlayCards.IsMarked(card))
                __result = true;
        }
    }
}
