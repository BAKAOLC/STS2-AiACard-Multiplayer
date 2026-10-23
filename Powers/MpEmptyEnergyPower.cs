using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    public sealed class MpEmptyEnergyPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpLoseEnergyNextTurnPower, Const.Paths.PowerIcons.MpLoseEnergyNextTurnPower);

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            if (Owner.Player != null)
                await PlayerCmd.SetEnergy(0, Owner.Player);
            await PowerCmd.Remove(this);
        }
    }
}
