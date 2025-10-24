using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Vilens.Data;
using Vilens.Logging;

namespace Vilens.Features;

internal sealed class PropertyInline : FeatureBase
{
    private readonly ImmutableList<MethodDef> _methods;
    private int _propertiesInlined;

    // Ignore Spelling: Inline
    /// <inheritdoc />
    public PropertyInline(Scrambler scrambler) : base(scrambler)
    {
        _methods = Database.Methods.AsParallel().Where(m => m.Item.HasBody && m.HasFeatures(VilensFeature.PropertyInline)).Select(m => m.Item).ToImmutableList();
        Log.Debug("Filtered {count} methods", _methods.Count);
    }

    public override Logger Log { get; } = new Logger(nameof(PropertyInline));

    /// <inheritdoc />
    public override void Execute()
    {
        Log.Debug("Finding auto-properties");
        var dict = GetReplacements(Database.MemberFinder.PropertyDefs.Keys);
        Log.Debug("Found {count} auto-properties", dict.Count);

        _ = Parallel.ForEach(_methods, method =>
        {
            Cancellation.ThrowIfCancellationRequested();
            var instrs = method.Body.Instructions;
            foreach (var instr in instrs)
            {
                if (!(instr.OpCode == OpCodes.Call || instr.OpCode == OpCodes.Callvirt) || instr.Operand is not MethodDef called)
                {
                    // Not a method call
                    continue;
                }

                if (dict.TryGetValue(called, out var pair))
                {
                    var (code, field) = pair;
                    if (method.DeclaringType != field.DeclaringType)
                    {
                        // Not the same type => cannot call private field
                        continue;
                    }
                    var old = instr.Clone();

                    instr.OpCode = code;
                    instr.Operand = field;

                    Log.Trace("Replaced {old} with {new} in {method}", old, instr, method);
                    _ = Interlocked.Increment(ref _propertiesInlined);
                }
            }
        });

        Log.Info("In-lined {old} calls to auto-properties", _propertiesInlined);
    }

    private Dictionary<MethodDef, (OpCode, FieldDef)> GetReplacements(IReadOnlyCollection<PropertyDef> properties)
    {
        var dict = new Dictionary<MethodDef, (OpCode, FieldDef)>();
        foreach (var property in properties)
        {
            if (TryGetAutoProperty(property, out var backingField))
            {
                Log.Trace("{0} is an auto-property with field {1}", property, backingField);
                Debug.Assert(property.GetMethod.HasReturnType);
                Debug.Assert(property.GetMethod.GetParamCount() == 0);
                dict.Add(property.GetMethod, (property.GetMethod.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, backingField));
                if (property.SetMethod != null)
                {
                    Debug.Assert(!property.SetMethod.HasReturnType);
                    Debug.Assert(property.SetMethod.GetParamCount() == 1);
                    dict.Add(property.SetMethod, (property.SetMethod.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, backingField));
                }
            }
            else
            {
                Log.Trace("{0} is not an auto-property", property);
            }
        }
        return dict;
    }

    private bool TryGetAutoProperty(PropertyDef property, [NotNullWhen(true)] out FieldDef? backingField)
    {
        Log.Trace("Checking if {0} is an auto-property", property);
        backingField = default;
        if (property.GetMethod is null)
        {
            Log.Trace("Property {prop} has no getter", property);
            return false;
        }

        bool isStatic = property.GetMethod.IsStatic;
        bool hasSetter = property.SetMethod is not null;

        if (hasSetter && property.SetMethod!.IsStatic != isStatic)
        {
            Log.Error("Property {prop} has a static getter and a instance setter.", property);
            return false;
        }

        if (!TryParseAccessor(property.GetMethod, out var backingField1, true))
        {
            return false;
        }

        if (hasSetter)
        {
            if (!TryParseAccessor(property.SetMethod!, out var backingField2, false))
            {
                return false;
            }

            if (backingField1 != backingField2)
            {
                return false;
            }
        }

        Debug.Assert(backingField1.IsStatic == property.GetMethod.IsStatic);

        if (!backingField1.IsPrivate)
        {
            return false;
        }

        if (property.DeclaringType != backingField1.DeclaringType)
        {
            return false;
        }

        backingField = backingField1;
        return true;
    }

    private static bool TryParseAccessor(MethodDef method, [NotNullWhen(true)] out FieldDef? backingField, bool isGetter)
    {
        backingField = null;
        if (method.Body is null || method is { IsVirtual: true, IsFinal: false })
        {
            return false;
        }
        method.Body.OptimizeMacros();
        var instr = method.Body.Instructions.Where(i => i.OpCode != OpCodes.Nop).ToList();

        int expectedCount = (isGetter, method.IsStatic) switch
        {
            (true, true) => 2,
            (true, false) => 3,
            (false, true) => 3,
            (false, false) => 4,
        };

        if (instr.Count != expectedCount)
        {
            return false;
        }

        switch (isGetter, method.IsStatic)
        {
            case (true, true): // static getter
                if (instr[0].OpCode == OpCodes.Ldsfld &&
                instr[0].Operand is FieldDef field &&
                instr[1].OpCode == OpCodes.Ret)
                {
                    //IL_0000: ldsfld type Class::'<Property>k__BackingField'
                    //IL_0005: ret
                    backingField = field;
                    return true;
                }
                break;

            case (true, false): // instance getter
                if (instr[0].OpCode == OpCodes.Ldarg_0 &&
                instr[1].OpCode == OpCodes.Ldfld &&
                instr[1].Operand is FieldDef field2 &&
                instr[2].OpCode == OpCodes.Ret)
                {
                    //IL_0000: ldarg.0
                    //IL_0001: ldfld type Class::'<Property>k__BackingField'
                    //IL_0006: ret
                    backingField = field2;
                    return true;
                }
                break;

            case (false, true): // static setter
                if (instr[0].OpCode == OpCodes.Ldarg_0 &&
                instr[1].OpCode == OpCodes.Stsfld &&
                instr[1].Operand is FieldDef field3 &&
                instr[2].OpCode == OpCodes.Ret)
                {
                    //IL_0000: ldarg.0
                    //IL_0001: stsfld type Class::'<Property>k__BackingField'
                    //IL_0006: ret
                    backingField = field3;
                    return true;
                }
                break;

            case (false, false): // instance setter
                if (instr[0].OpCode == OpCodes.Ldarg_0 &&
                instr[1].OpCode == OpCodes.Ldarg_1 &&
                instr[2].OpCode == OpCodes.Stfld &&
                instr[2].Operand is FieldDef field4 &&
                instr[3].OpCode == OpCodes.Ret)
                {
                    //IL_0000: ldarg.0
                    //IL_0001: ldarg.1
                    //IL_0002: stfld type Class::'<Property>k__BackingField'
                    //IL_0007: ret
                    backingField = field4;
                    return true;
                }
                break;
            default:
                // unreachable
        }
        return false;
    }
}
