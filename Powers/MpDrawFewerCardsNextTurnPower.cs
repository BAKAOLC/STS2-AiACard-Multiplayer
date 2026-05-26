using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    public sealed class MpDrawFewerCardsNextTurnPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpCheckDpsPower, Const.Paths.PowerIcons.MpCheckDpsPower);

        public override decimal ModifyHandDraw(Player player, decimal count)
        {
            if (player != Owner.Player || AmountOnTurnStart == 0)
                return count;

            return Math.Max(0m, count - Amount);
        }

        public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
        {
            if (side == Owner.Side && AmountOnTurnStart != 0)
                await PowerCmd.Remove(this);
        }
    }
}
