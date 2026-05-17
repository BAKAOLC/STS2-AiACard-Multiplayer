using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>偏差认知：1 回合内，每消耗 1 点能量为自己生成 1 个随机充能球；层数表示剩余回合。</summary>
    public sealed class MpPerEnergySelfChannelPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpPerEnergySelfChannelPower, Const.Paths.PowerIcons.MpPerEnergySelfChannelPower);

        public override async Task AfterEnergySpent(CardModel card, int amount)
        {
            if (Owner.Player == null || card.Owner != Owner.Player || amount <= 0 || Amount <= 0)
                return;

            var ctx = new ThrowingPlayerChoiceContext();
            var rng = Owner.Player.RunState.Rng.CombatOrbGeneration;
            for (var i = 0; i < amount; i++)
            {
                var orb = OrbModel.GetRandomOrb(rng).ToMutable();
                await OrbCmd.Channel(ctx, orb, Owner.Player);
            }
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != CombatSide.Player || Owner.IsMonster || Amount <= 0)
                return;

            await PowerCmd.ModifyAmount(this, -1m, null, null);
            if (Amount <= 0)
                await PowerCmd.Remove(this);
        }
    }
}
