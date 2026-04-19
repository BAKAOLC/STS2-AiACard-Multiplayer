using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace STS2_AiACard_Multiplayer.Models.CardPools;

/// <summary>
///     尸体说话衍生诅咒专用池：外观与原版 <see cref="MegaCrit.Sts2.Core.Models.CardPools.CurseCardPool" /> 一致，但不并入通用诅咒随机池。
/// </summary>
public sealed class MpCorpseCurseCardPool : CardPoolModel
{
    public override string Title => "curse";

    public override string EnergyColorName => "colorless";

    public override string CardFrameMaterialPath => "card_frame_curse";

    public override Color DeckEntryCardColor => new Color("585B61FF");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards() => [];
}
