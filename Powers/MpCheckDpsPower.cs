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
        private const int TargetEnergyLossBase = 2;
        private const int TargetEnergyLossUpgraded = 2;
        private const int DrawFewerNextHand = 1;
        private const int CasterEnergyGain = 2;
        private const int CasterDrawBase = 1;
        private const int CasterDrawUpgraded = 3;

        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override bool IsInstanced => true;

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpCheckDpsPower, Const.Paths.PowerIcons.MpCheckDpsPower);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new EnergyVar("DpsTargetEnergyLoss", TargetEnergyLossBase),
            new("DpsDrawFewerNextHand", DrawFewerNextHand),
            new EnergyVar("DpsCasterEnergyNextTurn", CasterEnergyGain),
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
            d.TargetEnergyLoss = upgraded ? TargetEnergyLossUpgraded : TargetEnergyLossBase;
            d.CasterEnergyNextTurn = CasterEnergyGain;
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
            DynamicVars["DpsTargetEnergyLoss"].BaseValue = d.TargetEnergyLoss;
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
                await PowerCmd.Apply<MpLoseEnergyNextTurnPower>(choiceContext, Owner, d.TargetEnergyLoss,
                    applierCreature, null);
                await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner, -DrawFewerNextHand, applierCreature,
                    null);
                foreach (var player in CombatState.Players.Where(p => p != Owner.Player && p.Creature.IsAlive))
                {
                    await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, player.Creature, d.CasterEnergyNextTurn,
                        applierCreature, null);
                    await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, player.Creature, d.CasterDrawNextTurn,
                        applierCreature, null);
                }
            }

            await PowerCmd.Remove(this);
        }

        private sealed class Data
        {
            public int CasterDrawNextTurn;
            public int CasterEnergyNextTurn;
            public bool PendingAfterTurnEnd;
            public int TargetEnergyLoss;
        }
    }
}
