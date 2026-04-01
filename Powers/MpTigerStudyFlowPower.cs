using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>俺要学猛虎下山：能力持有者打出与上一张不同类型牌时抽牌（仅统计能力持有者）。</summary>
    public sealed class MpTigerStudyFlowPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        protected override object InitInternalData()
        {
            return new Data();
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner.Player) return;

            if (cardPlay.Card.Type is not (CardType.Attack or CardType.Skill or CardType.Power)) return;

            var d = GetInternalData<Data>();
            if (d.LastType.HasValue && d.LastType.Value != cardPlay.Card.Type)
                await CardPileCmd.Draw(context, Amount, Owner.Player);

            d.LastType = cardPlay.Card.Type;
        }

        private sealed class Data
        {
            public CardType? LastType;
        }
    }
}
