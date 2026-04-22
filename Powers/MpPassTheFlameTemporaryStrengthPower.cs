using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_AiACard_Multiplayer.Cards.Ironclad;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.Content.Patches;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>「传火」给予的临时力量（回合结束移除并返还等量力量）。</summary>
    public sealed class MpPassTheFlameTemporaryStrengthPower : TemporaryStrengthPower, IModPowerAssetOverrides
    {
        public override AbstractModel OriginModel => ModelDb.Card<MpPassTheFlame>();

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
            [HoverTipFactory.FromPower<StrengthPower>()];

        public PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpPassTheFlameTemporaryStrengthPower,
                Const.Paths.PowerIcons.MpPassTheFlameTemporaryStrengthPower);

        public string? CustomIconPath => AssetProfile.IconPath;
        public string? CustomBigIconPath => AssetProfile.BigIconPath;
    }
}
