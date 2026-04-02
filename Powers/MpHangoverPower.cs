using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>全靠我了后续：下回合开始时能量归零、不可抽牌，并获得原版 <see cref="RingingPower" />（昏眩）。</summary>
    public sealed class MpHangoverPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Const.PlaceholderPowerIcon;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<NoDrawPower>(),
            HoverTipFactory.FromPower<RingingPower>(),
        ];

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            await PlayerCmd.SetEnergy(0, player);
            await PowerCmd.Apply<NoDrawPower>(Owner, 1, Owner, null);
            await PowerCmd.Apply<RingingPower>(Owner, 1, Owner, null);
            await PowerCmd.Remove(this);
        }
    }
}
