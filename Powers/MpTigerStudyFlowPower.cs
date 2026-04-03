using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    public sealed class MpTigerStudyFlowPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override bool IsInstanced => true;

        public override PowerAssetProfile AssetProfile => Const.PlaceholderPowerIcon;

        public override int DisplayAmount
        {
            get
            {
                var d = GetInternalData<Data>();
                return d.LastEligibleType.HasValue ? 1 : 2;
            }
        }

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("TigerFlowStepsUntilDraw", 2),
            new("TigerFlowLastType", (int)CardType.None),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<MpDoubleDamageTakenPower>()];

        protected override object? InitInternalData()
        {
            return new Data();
        }

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            SyncProgressToDynamicVars();
            InvokeDisplayAmountChanged();
            return Task.CompletedTask;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner.Player) return;
            if (cardPlay.Card.Type is not (CardType.Attack or CardType.Skill or CardType.Power)) return;

            var d = GetInternalData<Data>();
            if (!d.LastEligibleType.HasValue)
            {
                d.LastEligibleType = cardPlay.Card.Type;
                SyncProgressToDynamicVars();
                InvokeDisplayAmountChanged();
                return;
            }

            if (d.LastEligibleType.Value == cardPlay.Card.Type)
            {
                SyncProgressToDynamicVars();
                InvokeDisplayAmountChanged();
                return;
            }

            Flash();
            await CardPileCmd.Draw(context, Amount, Owner.Player);
            d.LastEligibleType = null;
            SyncProgressToDynamicVars();
            InvokeDisplayAmountChanged();
        }

        private void SyncProgressToDynamicVars()
        {
            var d = GetInternalData<Data>();
            DynamicVars["TigerFlowStepsUntilDraw"].BaseValue = d.LastEligibleType.HasValue ? 1 : 2;
            DynamicVars["TigerFlowLastType"].BaseValue = d.LastEligibleType.HasValue
                ? (int)d.LastEligibleType.Value
                : (int)CardType.None;
        }

        private sealed class Data
        {
            public CardType? LastEligibleType;
        }
    }
}
