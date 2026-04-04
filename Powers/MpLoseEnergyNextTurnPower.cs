using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    public sealed class MpLoseEnergyNextTurnPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpLoseEnergyNextTurnPower, Const.Paths.PowerIcons.MpLoseEnergyNextTurnPower);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.ForEnergy(this)];

        public override async Task AfterEnergyReset(Player player)
        {
            if (player != Owner.Player) return;
            await PlayerCmd.LoseEnergy(Amount, player);
            await PowerCmd.Remove(this);
        }
    }
}
