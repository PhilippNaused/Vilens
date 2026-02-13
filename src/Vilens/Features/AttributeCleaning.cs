using System.Collections.Immutable;
using System.Diagnostics;
using dnlib.DotNet;
using Vilens.Data;
using Vilens.Helpers;
using Vilens.Logging;

namespace Vilens.Features;

internal sealed class AttributeCleaning : FeatureBase
{
    private static readonly HashSet<string> s_AttributeNames = // can always be removed
    [
        "System.Runtime.CompilerServices.CompilerGeneratedAttribute",
        "System.CodeDom.Compiler.GeneratedCodeAttribute",
        "System.Text.RegularExpressions.GeneratedRegexAttribute",
        "System.Diagnostics.DebuggerBrowsableAttribute",
        "System.Diagnostics.DebuggerDisplayAttribute",
        "System.Diagnostics.DebuggerHiddenAttribute",
        "System.Diagnostics.ConditionalAttribute",
        "System.Diagnostics.DebuggerStepperBoundaryAttribute",
        "System.Diagnostics.DebuggerTypeProxyAttribute",
        "System.Diagnostics.DebuggerVisualizerAttribute",
        "System.Diagnostics.DebuggerNonUserCodeAttribute",
        "System.Diagnostics.DebuggerStepThroughAttribute",
        "System.Runtime.CompilerServices.IteratorStateMachineAttribute",
        "System.Runtime.CompilerServices.AsyncIteratorStateMachineAttribute",
        "System.Runtime.CompilerServices.AsyncStateMachineAttribute",
        "System.Runtime.CompilerServices.ModuleInitializerAttribute",
        "System.Runtime.CompilerServices.AsyncMethodBuilderAttribute"
    ];

    private static readonly HashSet<string> s_PublicAttributeNames = // can only be removed if in scope
    [
        "Microsoft.CodeAnalysis.EmbeddedAttribute",
        "System.Runtime.CompilerServices.NullableContextAttribute",
        "System.Runtime.CompilerServices.NullableAttribute",
        "System.Runtime.CompilerServices.RefSafetyRulesAttribute",
        "System.Runtime.CompilerServices.TupleElementNamesAttribute",
        "System.FlagsAttribute",
        "System.Runtime.CompilerServices.IsReadOnlyAttribute",
        "System.Runtime.CompilerServices.InterpolatedStringHandlerAttribute",
        "System.Runtime.CompilerServices.InterpolatedStringHandlerArgumentAttribute",
        "System.Runtime.CompilerServices.CallerArgumentExpressionAttribute",
        "System.Runtime.CompilerServices.CallerFilePathAttribute",
        "System.Runtime.CompilerServices.CallerLineNumberAttribute",
        "System.Runtime.CompilerServices.CallerMemberNameAttribute",
        "System.Runtime.CompilerServices.DiscardableAttribute",
        "System.Runtime.CompilerServices.EnumeratorCancellationAttribute",
        "System.Diagnostics.CodeAnalysis.NotNullWhenAttribute",
        "System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute",
        "System.Diagnostics.CodeAnalysis.NotNullAttribute",
        "System.Diagnostics.CodeAnalysis.MemberNotNullAttribute",
        "System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute",
        "System.Runtime.CompilerServices.ExtensionAttribute",
    ];

    private readonly ImmutableHashSet<ITypeDefOrRef> _publicRemovableAttributes;

    private readonly ImmutableHashSet<ITypeDefOrRef> _removableAttributes;

    public AttributeCleaning(Scrambler scrambler) : base(scrambler)
    {
        var allAttributes = Database.MemberFinder.CustomAttributes.Keys.Select(ca => ca.AttributeType).Distinct().ToList();
        _removableAttributes = allAttributes.Where(def => s_AttributeNames.Contains(def.FullName)).ToImmutableHashSet();
        _publicRemovableAttributes = allAttributes.Where(def => s_PublicAttributeNames.Contains(def.FullName)
                                                               || def.Namespace == "JetBrains.Annotations"
                                                               || def.Namespace == "System.Diagnostics.CodeAnalysis").ToImmutableHashSet();
    }

    public override Logger Log { get; } = new Logger(nameof(AttributeCleaning));

    public int AttributesRemoved { get; set; }

    public override void Execute()
    {
        var types = Module.GetTypes().ToList();
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < types.Count; i++)
        {
            var type = types[i];
            Log.Trace("Processing type [{0}]", type);

            Cancellation.ThrowIfCancellationRequested();
            Process(type);

            if (sw.ElapsedMilliseconds > 1000)
            {
                Log.Debug("{0:p} done", (double)i / types.Count);
                sw.Restart();
            }
        }

        Log.Info("Removed {0} attributes", AttributesRemoved);
    }

    private void ClearCustomAttributes(IHasCustomAttribute obj, ImmutableHashSet<ITypeDefOrRef> attributes)
    {
        CustomAttributeCollection coll = obj.CustomAttributes;
        for (int i = coll.Count - 1; i >= 0; i--)
        {
            if (attributes.Contains(coll[i].AttributeType))
            {
                Log.Trace("Removing [{0}] from [{1}]", coll[i], obj);
                coll.RemoveAt(i);
                AttributesRemoved++;
            }
        }
    }

    private void Process(IMemberDef member)
    {
        if (!member.HasFeatures(VilensFeature.AttributeCleaning, Database.FeatureMap))
            return;

        Log.Trace("Processing [{0}]", member);
        ClearCustomAttributes(member, _removableAttributes);
        if (IsInScope(member))
        {
            ClearCustomAttributes(member, _publicRemovableAttributes);
            if (member is ITypeOrMethodDef def)
            {
                foreach (var gp in def.GenericParameters)
                {
                    ClearCustomAttributes(gp, _publicRemovableAttributes);
                }
            }
            if (member is TypeDef type)
            {
                foreach (var impl in type.Interfaces)
                {
                    ClearCustomAttributes(impl, _publicRemovableAttributes);
                }
            }
            if (member is MethodDef method)
            {
                foreach (var param in method.ParamDefs)
                {
                    ClearCustomAttributes(param, _publicRemovableAttributes);
                }
            }
        }

        if (member is TypeDef type2)
        {
            foreach (IMemberDef member2 in type2.GetMembers())
                Process(member2);
        }
    }
}
