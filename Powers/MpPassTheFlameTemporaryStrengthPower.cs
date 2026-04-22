using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_AiACard_Multiplayer.Cards.Ironclad;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>「传火」给予的临时力量（回合结束移除并返还等量力量）。</summary>
    public sealed class MpPassTheFlameTemporaryStrengthPower : TemporaryStrengthPower
    {
        public override AbstractModel OriginModel => ModelDb.Card<MpPassTheFlame>();
    }
}

