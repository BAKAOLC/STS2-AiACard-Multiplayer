using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    public sealed class MpDizzyPower : ModPowerTemplate
    {
        private bool _playedCard;
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpHangoverPower, Const.Paths.PowerIcons.MpHangoverPower);

        public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType) =>
            card.Owner != Owner.Player || !_playedCard;

        public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == Owner.Player)
                _playedCard = true;
            return Task.CompletedTask;
        }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (participants.Contains(Owner))
                await PowerCmd.Remove(this);
        }
    }
}
