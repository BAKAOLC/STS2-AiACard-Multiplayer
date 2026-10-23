using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2_AiACard_Multiplayer.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Ironclad
{
    /// <summary>传火：本回合将你消耗的牌的复制品交给目标，目标下回合获得能量。</summary>
    public sealed class MpPassTheFlame()
        : MpOnlyModCardTemplate(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpPassTheFlame, Const.Paths.CardPortraits.MpPassTheFlame);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<MpPassExhaustedCardsPower>(), HoverTipFactory.FromPower<EnergyNextTurnPower>()];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var power = await PowerCmd.Apply<MpPassExhaustedCardsPower>(choiceContext, Owner.Creature, 1,
                Owner.Creature, this);
            if (power != null)
                power.Recipient = target;
            await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, target.Creature, IsUpgraded ? 2 : 1,
                Owner.Creature, this);
        }
    }
}
