using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2_AiACard_Multiplayer.Cards.Necrobinder;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Patching.Models;

namespace STS2_AiACard_Multiplayer.Patches
{
    internal static class MpCorpseSpeaksPatchLogic
    {
        internal static bool HasOtherPlayerAllyOnSameSide(CardModel card)
        {
            var cs = card.CombatState ?? card.Owner?.Creature.CombatState;
            if (cs == null || card.Owner?.Creature == null) return false;

            var self = card.Owner.Creature;
            foreach (var c in cs.PlayerCreatures)
            {
                if (!c.IsPlayer || c == self) continue;
                if (c.Side == self.Side) return true;
            }

            return false;
        }
    }

    /// <summary>解除「仅一名存活玩家」时对尸体说话的封锁。</summary>
    public sealed class MpCorpseSpeaksCardModelCanPlayPatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.corpse_speaks.can_play";

        public static string Description => "尸体说话：允许在仅有死亡队友时仍可打出";

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(CardModel), nameof(CardModel.CanPlay),
                    [typeof(UnplayableReason).MakeByRefType(), typeof(AbstractModel).MakeByRefType()]),
            ];
        }

        public static void Postfix(CardModel __instance, ref UnplayableReason reason, ref AbstractModel? preventer,
            ref bool __result)
        {
            if (__instance is not MpCorpseSpeaks) return;

            if (reason.HasFlag(UnplayableReason.NoLivingAllies) &&
                MpCorpseSpeaksPatchLogic.HasOtherPlayerAllyOnSameSide(__instance))
                reason &= ~UnplayableReason.NoLivingAllies;

            __result = reason == UnplayableReason.None;
        }
    }

    /// <summary>允许以死亡玩家为合法目标。</summary>
    public sealed class MpCorpseSpeaksCardModelIsValidTargetPatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.corpse_speaks.is_valid_target";

        public static string Description => "尸体说话：IsValidTarget 接受死亡队友";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CardModel), nameof(CardModel.IsValidTarget), [typeof(Creature)])];
        }

        public static void Postfix(CardModel __instance, Creature? target, ref bool __result)
        {
            if (__instance is not MpCorpseSpeaks || target == null) return;

            if (MpForceKillReviveBlock.IsBlockedCorpse(target))
            {
                __result = false;
                return;
            }

            if (__result) return;
            if (target.IsAlive) return;
            if (!target.IsPlayer || __instance.Owner?.Creature == null) return;
            if (target == __instance.Owner.Creature) return;
            if (target.Side != __instance.Owner.Creature.Side) return;

            __result = true;
        }
    }

    /// <summary>鼠标/手柄瞄准时允许指向死亡队友 UI。</summary>
    public sealed class MpCorpseSpeaksNTargetManagerAllowedToTargetCreaturePatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.corpse_speaks.allowed_to_target";

        public static string Description => "尸体说话：NTargetManager 允许死亡队友节点";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NTargetManager), "AllowedToTargetCreature", [typeof(Creature)])];
        }

        public static void Postfix(NTargetManager __instance, Creature creature, ref bool __result)
        {
            if (MpCorpseSpeaksTargetingState.ActiveCard is MpCorpseSpeaks &&
                MpForceKillReviveBlock.IsBlockedCorpse(creature))
            {
                __result = false;
                return;
            }

            if (__result) return;
            if (MpCorpseSpeaksTargetingState.ActiveCard is not MpCorpseSpeaks card) return;

            var vt = Traverse.Create(__instance).Field<TargetType>("_validTargetsType").Value;
            if (vt != TargetType.AnyAlly) return;
            if (!creature.IsPlayer || !creature.IsDead) return;
            if (LocalContext.IsMe(creature.Player)) return;
            var ownerCreature = card.Owner?.Creature;
            if (ownerCreature == null) return;
            if (creature.Side != ownerCreature.Side || creature == ownerCreature) return;

            __result = true;
        }
    }

    public sealed class MpCorpseSpeaksNTargetManagerStartTargetingVector2Patch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.corpse_speaks.start_targeting_v2";

        public static string Description => "尸体说话：StartTargeting(Vector2) 时清除瞄准上下文";

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(NTargetManager), nameof(NTargetManager.StartTargeting),
                [
                    typeof(TargetType), typeof(Vector2), typeof(TargetMode), typeof(Func<bool>),
                    typeof(Func<Node, bool>),
                ]),
            ];
        }

        public static void Prefix(
            TargetType validTargetsType,
            Vector2 startPosition,
            TargetMode startingMode,
            Func<bool>? exitEarlyCondition,
            Func<Node, bool>? nodeFilter)
        {
            MpCorpseSpeaksTargetingState.ActiveCard = null;
        }
    }

    public sealed class MpCorpseSpeaksNTargetManagerStartTargetingControlPatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.corpse_speaks.start_targeting_control";

        public static string Description => "尸体说话：StartTargeting(Control) 时记录是否为尸体说话";

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(NTargetManager), nameof(NTargetManager.StartTargeting),
                [
                    typeof(TargetType), typeof(Control), typeof(TargetMode), typeof(Func<bool>),
                    typeof(Func<Node, bool>),
                ]),
            ];
        }

        public static void Prefix(
            TargetType validTargetsType,
            Control control,
            TargetMode startingMode,
            Func<bool>? exitEarlyCondition,
            Func<Node, bool>? nodeFilter)
        {
            MpCorpseSpeaksTargetingState.ActiveCard = null;
            if (control is NCard { Model: MpCorpseSpeaks m }) MpCorpseSpeaksTargetingState.ActiveCard = m;
        }
    }

    public sealed class MpCorpseSpeaksNTargetManagerFinishTargetingPatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.corpse_speaks.finish_targeting";

        public static string Description => "尸体说话：结束瞄准时清除上下文";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NTargetManager), "FinishTargeting", [typeof(bool)])];
        }

        public static void Postfix(bool cancel)
        {
            MpCorpseSpeaksTargetingState.ActiveCard = null;
        }
    }

    /// <summary>瞄准尸体说话时，死亡队友在手柄循环里视为可命中。</summary>
    public sealed class MpCorpseSpeaksCreatureIsHittablePatch : IPatchMethod
    {
        public static string PatchId => $"{Const.ModId}.corpse_speaks.is_hittable";

        public static string Description => "尸体说话：瞄准期间死亡队友视为可命中以进入列表";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(Creature), "get_IsHittable", Type.EmptyTypes)];
        }

        public static void Postfix(Creature __instance, ref bool __result)
        {
            if (MpCorpseSpeaksTargetingState.ActiveCard is MpCorpseSpeaks &&
                MpForceKillReviveBlock.IsBlockedCorpse(__instance))
            {
                __result = false;
                return;
            }

            if (__result) return;
            if (MpCorpseSpeaksTargetingState.ActiveCard is not MpCorpseSpeaks card) return;
            if (!__instance.IsPlayer || !__instance.IsDead) return;
            var ownerCreature = card.Owner?.Creature;
            if (ownerCreature == null) return;
            if (__instance.Side != ownerCreature.Side || __instance == ownerCreature) return;

            __result = true;
        }
    }
}
