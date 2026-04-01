using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer
{
    /// <summary>本包内卡牌均为多人玩法设计，单人模式下不应进入卡池与奖励。</summary>
    public abstract class MpOnlyModCardTemplate(
        int baseCost,
        CardType type,
        CardRarity rarity,
        TargetType target,
        bool showInCardLibrary = true)
        : ModCardTemplate(baseCost, type, rarity, target, showInCardLibrary)
    {
        public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    }
}
