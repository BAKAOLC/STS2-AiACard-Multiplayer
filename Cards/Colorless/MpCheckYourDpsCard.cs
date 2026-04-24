using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2_AiACard_Multiplayer.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>查你DPS：本阶段目标若未打出攻击则受罚，你获得下回合资源。</summary>
    public sealed class MpCheckYourDpsCard()
        : MpOnlyModCardTemplate(0, CardType.Skill, CardRarity.Common, TargetType.AnyAlly)
    {
        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpCheckYourDpsCard, Const.Paths.CardPortraits.MpCheckYourDpsCard);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips => ModelDb.Power<MpCheckDpsPower>().HoverTips;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var t = MpHelpers.RequireTargetPlayer(cardPlay);
            await PowerCmd.Apply<MpCheckDpsPower>(choiceContext, t.Creature, 1, Owner.Creature, this);
        }
    }
}
