using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>查你DPS：本玩家阶段内若未打出攻击牌，目标失去能量并少抽牌，施法者下回合多抽并获得下回合能量。</summary>
    public sealed class MpCheckDpsPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Single;

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

            var applierCreature = Applier;
            if (applierCreature?.Player == null)
            {
                await PowerCmd.Remove(this);
                return;
            }

            var targetPlayer = Owner.Player ??
                               throw new InvalidOperationException("MpCheckDpsPower owner must be a player.");
            await PlayerCmd.LoseEnergy(2, targetPlayer);
            await PowerCmd.Apply<MpDrawPenaltyOncePower>(Owner, 1, applierCreature, null);

            var energyBonus = Amount >= 1 ? 3 : 2;
            var drawBonus = Amount >= 1 ? 2 : 1;
            await PowerCmd.Apply<EnergyNextTurnPower>(applierCreature, energyBonus, applierCreature, null);
            await PowerCmd.Apply<DrawCardsNextTurnPower>(applierCreature, drawBonus, applierCreature, null);

            await PowerCmd.Remove(this);
        }
    }
}
