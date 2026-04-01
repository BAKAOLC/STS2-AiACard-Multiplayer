using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>偏差认知：本玩家阶段内，每消耗 1 点能量为自己生成 Amount 个随机充能球。</summary>
    public sealed class MpPerEnergySelfChannelPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Const.PlaceholderPowerIcon;

        public override async Task AfterEnergySpent(CardModel card, int amount)
        {
            if (Owner.Player == null || card.Owner != Owner.Player || amount <= 0) return;

            var orbsPerEnergy = Amount;
            if (orbsPerEnergy <= 0) orbsPerEnergy = 1;

            var ctx = new ThrowingPlayerChoiceContext();
            var n = amount * orbsPerEnergy;
            var rng = Owner.Player.RunState.Rng.CombatOrbGeneration;
            for (var i = 0; i < n; i++)
            {
                var orb = OrbModel.GetRandomOrb(rng).ToMutable();
                await OrbCmd.Channel(ctx, orb, Owner.Player);
            }
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == CombatSide.Player) await PowerCmd.Remove(this);
        }
    }
}
