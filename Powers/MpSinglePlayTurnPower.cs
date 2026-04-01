using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Powers
{
    /// <summary>晕眩限制：本回合仅能再打出一张牌。</summary>
    public sealed class MpSinglePlayTurnPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Const.PlaceholderPowerIcon;

        public override bool IsInstanced => true;

        protected override object InitInternalData()
        {
            return new Data();
        }

        public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
        {
            if (Owner.Player == null || card.Owner != Owner.Player) return true;

            return GetInternalData<Data>().PlaysThisTurn < 1;
        }

        public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == Owner.Player) GetInternalData<Data>().PlaysThisTurn++;

            return Task.CompletedTask;
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == CombatSide.Player) await PowerCmd.Remove(this);
        }

        private sealed class Data
        {
            public int PlaysThisTurn;
        }
    }
}
