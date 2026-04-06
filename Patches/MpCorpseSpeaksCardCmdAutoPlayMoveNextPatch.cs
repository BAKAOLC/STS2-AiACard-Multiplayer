using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using STS2_AiACard_Multiplayer.Cards.Necrobinder;

namespace STS2_AiACard_Multiplayer.Patches
{
    /// <summary>尸体说话：在 CardCmd.AutoPlay 状态机里把 AnyAlly 自动选目标的盟友池改为可含死亡队友。</summary>
    public static class MpCorpseSpeaksCardCmdAutoPlayMoveNextPatch
    {
        /// <summary>定位 CardCmd.AutoPlay 生成的 IAsyncStateMachine.MoveNext。</summary>
        public static MethodBase? TryResolveAutoPlayMoveNext()
        {
            MethodBase? fallback = null;
            foreach (var t in typeof(CardCmd).GetNestedTypes(BindingFlags.NonPublic))
            {
                if (!typeof(IAsyncStateMachine).IsAssignableFrom(t)) continue;
                if (!t.Name.Contains("AutoPlay", StringComparison.Ordinal)) continue;

                var moveNext = t.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.NonPublic);
                if (moveNext == null) continue;

                if (t.Name.StartsWith("<AutoPlay>", StringComparison.Ordinal))
                    return moveNext;

                fallback ??= moveNext;
            }

            return fallback;
        }

        /// <summary>AutoPlay 的 AnyAlly 候选生物序列（尸体说话含同侧死亡玩家）。</summary>
        public static IEnumerable<Creature> BuildAutoPlayAnyAllyPool(CombatState combatState, CardModel card)
        {
            if (card is MpCorpseSpeaks)
            {
                var owner = card.Owner?.Creature;
                if (owner == null) return [];
                return combatState.PlayerCreatures.Where(c =>
                    c.IsPlayer && c != owner && c.Side == owner.Side);
            }

            return combatState.Allies.Where(c =>
                c != null && c.IsAlive && c.IsPlayer && card.Owner?.Creature != null && c != card.Owner.Creature);
        }

        public static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator _,
            MethodBase original)
        {
            var list = instructions.ToList();
            var getAllies = AccessTools.PropertyGetter(typeof(CombatState), nameof(CombatState.Allies));
            if (getAllies == null) return list;

            var idx = list.FindIndex(ci => ci.Calls(getAllies));
            if (idx < 0) return list;

            var whereOpen = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == nameof(Enumerable.Where) && m.IsGenericMethodDefinition &&
                                     m.GetParameters().Length == 2
                                     && m.GetParameters()[1].ParameterType.IsGenericType
                                     && m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() ==
                                     typeof(Func<,>));
            if (whereOpen == null) return list;

            var whereIdx = -1;
            for (var k = idx + 1; k < list.Count; k++)
            {
                if (list[k].opcode != OpCodes.Call && list[k].opcode != OpCodes.Callvirt) continue;
                if (list[k].operand is not MethodInfo mi || !mi.IsGenericMethod) continue;
                if (mi.GetGenericMethodDefinition() != whereOpen) continue;
                var ga = mi.GetGenericArguments();
                if (ga.Length >= 1 && ga[0] == typeof(Creature))
                {
                    whereIdx = k;
                    break;
                }
            }

            if (whereIdx < 0) return list;

            var smType = original.DeclaringType ??
                         throw new InvalidOperationException("MoveNext has no declaring type.");
            var cardField = ResolveCardModelField(smType);
            if (cardField == null) return list;

            var helper = AccessTools.Method(typeof(MpCorpseSpeaksCardCmdAutoPlayMoveNextPatch),
                nameof(BuildAutoPlayAnyAllyPool));
            if (helper == null) return list;

            var removeCount = whereIdx - idx + 1;
            list.RemoveRange(idx, removeCount);
            list.InsertRange(idx,
            [
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, cardField),
                new(OpCodes.Call, helper),
            ]);
            return list;
        }

        private static FieldInfo? ResolveCardModelField(Type smType)
        {
            var fields = smType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(f => f.FieldType == typeof(CardModel))
                .ToList();
            if (fields.Count == 0) return null;
            return fields.FirstOrDefault(f => f.Name.Contains("card", StringComparison.OrdinalIgnoreCase)) ?? fields[0];
        }
    }
}
