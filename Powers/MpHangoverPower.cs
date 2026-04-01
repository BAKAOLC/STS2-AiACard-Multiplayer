using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>全靠我了后续：下回合开始时能量归零且本回合无法抽牌。</summary>
    public sealed class MpHangoverPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Const.PlaceholderPowerIcon;

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner)
            {
                return;
            }

            await PlayerCmd.SetEnergy(0, player);
            await PowerCmd.Apply<NoDrawPower>(Owner, 1, Owner, null);
            await PowerCmd.Apply<MpSinglePlayTurnPower>(Owner, 1, Owner, null);
            await PowerCmd.Remove(this);
        }
    }
}
