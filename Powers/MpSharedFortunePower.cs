using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    public sealed class MpSharedFortunePower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

        public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpSharedFortunePower, Const.Paths.PowerIcons.MpSharedFortunePower);

        public override Task AfterCombatEnd(CombatRoom room)
        {
            var player = Owner.Player;
            var combatState = Owner.CombatState;
            if (player == null || combatState == null)
                return Task.CompletedTask;

            var pool = MpSharedWealthCombatPool.GetOrCreate(combatState);
            int totalGold;
            lock (pool)
            {
                if (pool.Distributed || pool.PendingTotalGold <= 0)
                    return Task.CompletedTask;

                pool.Distributed = true;
                totalGold = pool.PendingTotalGold;
                pool.PendingTotalGold = 0;
            }

            var players = combatState.Players.ToList();
            var shares = MpSharedWealthGoldDistribution.Distribute(
                totalGold, players.Count, 1, player.RunState.Rng.Shuffle);

            for (var i = 0; i < players.Count; i++)
            {
                var gold = shares[i];
                if (gold <= 0)
                    continue;

                room.AddExtraReward(players[i], new GoldReward(gold, players[i]));
            }

            return Task.CompletedTask;
        }
    }
}
