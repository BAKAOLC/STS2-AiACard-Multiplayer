using MegaCrit.Sts2.Core.Models;

namespace STS2_AiACard_Multiplayer.Patches
{
    /// <summary>瞄准尸体说话等需可选死亡队友时，当前关联的打出卡牌。</summary>
    internal static class MpCorpseSpeaksTargetingState
    {
        internal static CardModel? ActiveCard;
    }
}
