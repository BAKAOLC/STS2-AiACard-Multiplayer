using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Necrobinder
{
    /// <summary>尸语诅咒：回合结束时若在手中则受到 99 点伤害，随后将此牌消耗。</summary>
    public sealed class MpCorpseCurseToken()
        : MpOnlyModCardTemplate(-1, CardType.Status, CardRarity.Status, TargetType.None, false)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];

        public override bool HasTurnEndInHandEffect => true;

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
        {
            await CreatureCmd.Damage(choiceContext, Owner.Creature, 99m,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, null, this);
            await CardCmd.Exhaust(choiceContext, this);
        }
    }
}
