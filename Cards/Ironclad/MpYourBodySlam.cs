using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Ironclad
{
    /// <summary>你来撞击：将一张全身撞击放入目标玩家手牌。</summary>
    public sealed class MpYourBodySlam() : ModCardTemplate(0, CardType.Skill, CardRarity.Common, TargetType.AnyPlayer)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var slam = CombatState.CreateCard<BodySlam>(target);
            if (IsUpgraded)
            {
                CardCmd.Upgrade(slam);
            }

            await MpHelpers.AddToHand(choiceContext, slam);
        }
    }
}
