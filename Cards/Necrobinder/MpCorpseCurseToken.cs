using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Necrobinder
{
    /// <summary>尸语诅咒：诅咒牌；回合结束时若在手中则受到伤害，随后将此牌消耗。</summary>
    public sealed class MpCorpseCurseToken()
        : MpOnlyModCardTemplate(-1, CardType.Curse, CardRarity.Curse, TargetType.None, false)
    {
        public override bool CanBeGeneratedInCombat => false;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(99m, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable, CardKeyword.Ethereal];

        public override bool HasTurnEndInHandEffect => true;

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpCorpseCurseToken, Const.Paths.CardPortraits.MpCorpseCurseToken);

        public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
        {
            await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.Damage.BaseValue,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, null, this);
            await CardCmd.Exhaust(choiceContext, this);
        }
    }
}
