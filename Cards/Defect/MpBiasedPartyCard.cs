using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Defect
{
    /// <summary>你们都有认知偏差：其他玩家获得按耗能自充能球的效果。</summary>
    public sealed class MpBiasedPartyCard() : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<MpPerEnergySelfChannelPower>(1)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<BiasedCognition>(false)
                .Concat(ModelDb.Power<MpPerEnergySelfChannelPower>().HoverTips);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var orbsPerEnergy = DynamicVars["MpPerEnergySelfChannelPower"].BaseValue;
            foreach (var p in CombatState.Players.Where(p => p != Owner && !p.Creature.IsDead))
            {
                var biased = MpHelpers.CreateCard<BiasedCognition>(CombatState, p, false);
                await MpHelpers.AddToHand(choiceContext, biased);

                await PowerCmd.Apply<MpPerEnergySelfChannelPower>(p.Creature, orbsPerEnergy, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["MpPerEnergySelfChannelPower"].UpgradeValueBy(1m);
        }
    }
}
