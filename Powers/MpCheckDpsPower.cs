using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    public sealed class MpCheckDpsPower : ModPowerTemplate
    {
        private const int TargetEnergyLoss = 2;
        private const int DrawFewerNextHand = 1;
        private const int CasterEnergyBase = 2;
        private const int CasterEnergyUpgraded = 3;
        private const int CasterDrawBase = 1;
        private const int CasterDrawUpgraded = 2;

        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override bool IsInstanced => true;

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpCheckDpsPower, Const.Paths.PowerIcons.MpCheckDpsPower);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("DpsTargetEnergyLoss", TargetEnergyLoss),
            new("DpsDrawFewerNextHand", DrawFewerNextHand),
            new("DpsCasterEnergyNextTurn", CasterEnergyBase),
            new("DpsCasterDrawNextTurn", CasterDrawBase),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<MpLoseEnergyNextTurnPower>(),
            HoverTipFactory.FromPower<DrawCardsNextTurnPower>(),
            HoverTipFactory.FromPower<EnergyNextTurnPower>(),
        ];

        protected override object? InitInternalData()
        {
            return new Data();
        }

        public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
        {
            var d = GetInternalData<Data>();
            var upgraded = cardSource?.IsUpgraded == true;
            d.CasterEnergyNextTurn = upgraded ? CasterEnergyUpgraded : CasterEnergyBase;
            d.CasterDrawNextTurn = upgraded ? CasterDrawUpgraded : CasterDrawBase;
            return Task.CompletedTask;
        }

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            SyncLocVarsFromData();
            InvokeDisplayAmountChanged();
            return Task.CompletedTask;
        }

        private void SyncLocVarsFromData()
        {
            var d = GetInternalData<Data>();
            DynamicVars["DpsCasterEnergyNextTurn"].BaseValue = d.CasterEnergyNextTurn;
            DynamicVars["DpsCasterDrawNextTurn"].BaseValue = d.CasterDrawNextTurn;
        }

        public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != CombatSide.Player || Owner.IsMonster) return;

            var attacks = CombatManager.Instance.History.CardPlaysFinished.Count(e =>
                e.HappenedThisTurn(CombatState) &&
                e.CardPlay.Card.Owner == Owner.Player &&
                e.CardPlay.Card.Type == CardType.Attack);

            if (attacks > 0)
            {
                await PowerCmd.Remove(this);
                return;
            }

            if (Applier?.Player == null)
            {
                await PowerCmd.Remove(this);
                return;
            }

            GetInternalData<Data>().PendingAfterTurnEnd = true;
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != CombatSide.Player || Owner.IsMonster) return;

            var d = GetInternalData<Data>();
            if (!d.PendingAfterTurnEnd) return;

            var applierCreature = Applier;
            if (applierCreature?.Player != null)
            {
                await PowerCmd.Apply<MpLoseEnergyNextTurnPower>(Owner, TargetEnergyLoss, applierCreature, null);
                await PowerCmd.Apply<DrawCardsNextTurnPower>(Owner, -DrawFewerNextHand, applierCreature, null);
                await PowerCmd.Apply<EnergyNextTurnPower>(applierCreature, d.CasterEnergyNextTurn, applierCreature,
                    null);
                await PowerCmd.Apply<DrawCardsNextTurnPower>(applierCreature, d.CasterDrawNextTurn, applierCreature,
                    null);
            }

            await PowerCmd.Remove(this);
        }

        private sealed class Data
        {
            public int CasterDrawNextTurn;
            public int CasterEnergyNextTurn;
            public bool PendingAfterTurnEnd;
        }
    }
}
