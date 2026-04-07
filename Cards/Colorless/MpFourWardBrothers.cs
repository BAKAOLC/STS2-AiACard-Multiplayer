using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>四区兄弟：消耗手牌，生成等量无色牌（升级后为升级版），再以感染补满手牌。</summary>
    public sealed class MpFourWardBrothers()
        : MpOnlyModCardTemplate(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpFourWardBrothers, Const.Paths.CardPortraits.MpFourWardBrothers);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<Infection>();

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var perPlayer = new Dictionary<Player, int>();
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                var n = MpHelpers.SnapshotHand(p).Count;
                perPlayer[p] = n;
                foreach (var c in MpHelpers.SnapshotHand(p).ToList()) await CardCmd.Exhaust(choiceContext, c);
            }

            var pool = ModelDb.CardPool<ColorlessCardPool>();
            var canon = pool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => c is not Infection && c.Type != CardType.Status && c.Rarity != CardRarity.Basic);
            canon = CardFactory.FilterForCombat(canon);
            var list = canon.ToList();
            var rng = Owner.RunState.Rng.CombatCardGeneration;
            foreach (var kv in perPlayer)
            {
                var p = kv.Key;
                for (var i = 0; i < kv.Value && list.Count > 0; i++)
                {
                    var pick = list.TakeRandom(1, rng).First();
                    var card = MpHelpers.CreateCard(CombatState, p, pick, IsUpgraded);
                    await MpHelpers.AddToHand(choiceContext, card);
                }

                var pcs = p.PlayerCombatState;
                if (pcs == null) continue;

                while (pcs.Hand.Cards.Count < Const.CombatHandMax)
                {
                    var before = pcs.Hand.Cards.Count;
                    var inf = MpHelpers.CreateCard<Infection>(CombatState, p, false);
                    await MpHelpers.AddToHand(choiceContext, inf);
                    if (pcs.Hand.Cards.Count == before)
                        break;
                }
            }
        }
    }
}
