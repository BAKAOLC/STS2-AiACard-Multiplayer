using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>认知偏差：其他玩家打出牌时，按消耗能量为能力持有者生成随机球位（每回合至多 Amount 次）。</summary>
    public sealed class MpBiasedEchoPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Const.PlaceholderPowerIcon;

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (Owner.Player == null)
            {
                return;
            }

            if (cardPlay.Card.Owner == Owner.Player)
            {
                return;
            }

            var spent = cardPlay.Resources.EnergySpent;
            if (spent <= 0)
            {
                return;
            }

            var cap = (int)Math.Min(Amount, spent);
            for (var i = 0; i < cap; i++)
            {
                var orb = OrbModel.GetRandomOrb(Owner.Player.RunState.Rng.CombatOrbGeneration).ToMutable();
                await OrbCmd.Channel(context, orb, Owner.Player);
            }
        }
    }
}
