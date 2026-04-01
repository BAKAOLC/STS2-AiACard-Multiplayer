using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>是兄弟就来砍它：其他玩家打出君王之剑时，能力持有者将一张君王之剑置入手牌。</summary>
    public sealed class MpBrothersBladePower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card is not SovereignBlade) return;

            if (cardPlay.Card.Owner == Owner.Player) return;

            ArgumentNullException.ThrowIfNull(CombatState);
            var ownerPlayer = Owner.Player ??
                              throw new InvalidOperationException("MpBrothersBladePower owner must be a player.");
            var blade = MpHelpers.CreateCard<SovereignBlade>(CombatState, ownerPlayer, false);
            await MpHelpers.AddToHand(context, blade);
        }
    }
}
