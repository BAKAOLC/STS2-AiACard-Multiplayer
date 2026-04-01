using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>下次手牌分配时少抽若干张，随后移除。</summary>
    public sealed class MpDrawPenaltyOncePower : ModPowerTemplate
    {
        private bool _scheduledRemove;

        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override decimal ModifyHandDraw(Player player, decimal count)
        {
            if (player != Owner.Player) return count;

            _scheduledRemove = true;
            return Math.Max(0m, count - Amount);
        }

        public override async Task AfterModifyingHandDraw()
        {
            if (_scheduledRemove) await PowerCmd.Remove(this);
        }
    }
}
