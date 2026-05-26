using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2_AiACard_Multiplayer.Patches
{
    public sealed class MpRequiredFirstPlayCardDescriptionPatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.required_first_play.description";

        public static string Description => "必须优先打出：被标记手牌追加执迷式说明文本";

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(CardModel), nameof(CardModel.GetDescriptionForPile),
                    [typeof(PileType), typeof(Creature)]),
                new(typeof(CardModel), nameof(CardModel.GetDescriptionForUpgradePreview), Type.EmptyTypes),
            ];
        }

        public static void Postfix(CardModel __instance, ref string __result)
        {
            if (!MpRequiredFirstPlayCards.IsMarked(__instance))
                return;

            var text = ModelDb.Card<Enthralled>().Description.GetFormattedText();
            if (__result.Contains(text, StringComparison.Ordinal))
                return;

            __result = string.IsNullOrWhiteSpace(__result) ? text : $"{__result}\n{text}";
        }
    }
}
