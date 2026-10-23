using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    public sealed class MpPassExhaustedCardsPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
        public override PowerAssetProfile AssetProfile =>
            new(Const.Paths.PowerIcons.MpPassTheFlameTemporaryStrengthPower,
                Const.Paths.PowerIcons.MpPassTheFlameTemporaryStrengthPower);

        public Player? Recipient { get; set; }

        public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
            bool causedByEthereal)
        {
            var recipient = Recipient;
            if (card.Owner != Owner.Player || recipient == null || recipient.Creature.IsDead || CombatState == null)
                return;

            var copy = CombatState.CloneCard(card);
            copy.Owner = null!;
            copy.Owner = recipient;
            copy.DeckVersion = null;
            await MpHelpers.AddToHand(choiceContext, copy);
        }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (participants.Contains(Owner))
                await PowerCmd.Remove(this);
        }
    }
}
