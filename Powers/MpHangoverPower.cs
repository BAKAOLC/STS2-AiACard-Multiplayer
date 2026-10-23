using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>全靠我了后续：下回合分别施加三个可被抵挡的负面效果。</summary>
    public sealed class MpHangoverPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpHangoverPower, Const.Paths.PowerIcons.MpHangoverPower);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<NoDrawPower>(),
            HoverTipFactory.FromPower<MpDizzyPower>(),
        ];

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            await PowerCmd.Apply<MpEmptyEnergyPower>(choiceContext, Owner, 1, Owner, null);
            await PowerCmd.Apply<NoDrawPower>(choiceContext, Owner, 1, Owner, null);
            await PowerCmd.Apply<MpDizzyPower>(choiceContext, Owner, 1, Owner, null);
            await PowerCmd.Remove(this);
        }
    }
}
