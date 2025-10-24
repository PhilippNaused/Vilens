using System.Diagnostics;
using dnlib.DotNet;
using Vilens.Data;
using Vilens.Helpers;
using Vilens.Logging;

namespace Vilens.Features;

internal sealed class Renaming : FeatureBase
{
    private readonly List<IGrouping<TypeDef?, IMemberData<IMemberDef>>> _groups;
    private readonly List<ITypeData> _unnestedTypes;
    private int _membersRenamed;
    private HashSet<string> _usedTypeNames;

    public Renaming(Scrambler scrambler) : base(scrambler)
    {
        var sw = Stopwatch.StartNew();

        _groups = Database.Members.AsParallel().Where(member => member.Visibility <= Scrambler.Scope && member.HasFeatures(VilensFeature.Renaming)).GroupBy(t => t.Item.DeclaringType).ToList()!;

        // Non-tested types
        _unnestedTypes = _groups.SingleOrDefault(group => group.Key is null)?.Cast<ITypeData>().OrderBy(t => t.Item.Rid).ToList() ?? [];
        _usedTypeNames = [];
        Log.Debug("Filtered data in {time}.", sw.Elapsed);
    }

    public override Logger Log { get; } = new Logger(nameof(Renaming));

    public int MembersRenamed => _membersRenamed;

    public override void Execute()
    {
        var removed = _unnestedTypes.RemoveAll(t => t.WasTrimmed());
        Log.Debug("Removed {count} trimmed types", removed);
        // must be initialized here since previous steps may have added another type
        _usedTypeNames = [.. Module.Types.Select(t => t.FullName)];
        Debug.Assert(_usedTypeNames.Count >= _unnestedTypes.Count);
        // rename unnested types first (in sequence)
        foreach (var type in _unnestedTypes)
        {
            Cancellation.ThrowIfCancellationRequested();
            ProcessUnnestedType(type);
        }

        // rename all other members in parallel groups
        var result = Parallel.ForEach(_groups, group =>
        {
            Cancellation.ThrowIfCancellationRequested();
            if (group.Key is null)
            {
                // unnested types are already done.
                Debug.Assert(group.Where(t => !t.WasTrimmed()).SequenceEqual(_unnestedTypes));
                return;
            }

            foreach (var member in group.OrderBy(m => m.Item.Rid)) // sort them to stay deterministic
            {
                ProcessMember(member);
            }
        });

        Debug.Assert(result.IsCompleted);

        Log.Info("Renamed {count} members", MembersRenamed);
    }

    private void ProcessMember(IMemberData<IMemberDef> member)
    {
        if (member.WasTrimmed())
        {
            Log.Trace("Skipping [{0}] because it was trimmed", member);
            return;
        }
        Log.Trace("Processing {type} [{0}]", member.Kind, member);

        Debug.Assert(member.Item is not TypeDef { IsNested: false });

        if (member.Item is MethodDef method)
        {
            foreach (var parameter in method.Parameters)
            {
                if (parameter.IsNormalMethodParameter)
                {
                    parameter.Name = ",";
                }
            }
        }

        if (member.IsVirtual() || member.IsRuntimeSpecialName())
        {
            // don't rename virtual members or RTSpecialName flags
            return;
        }

        if (Database.UserStrings.Contains(member.Name) || Database.UserStrings.Contains(member.FullName))
        {
            Log.Trace("Skipping [{0}] because it's name was found on the user string heap. It may be referenced via reflection.", member);
            return;
        }

        Rename(member);
    }

    private void Rename(IMemberData<IMemberDef> member)
    {
        NamingHelper.Rename(member.Item, Scrambler.Settings.NamingScheme, _usedTypeNames);

        Log.Trace(@"Renaming {kind} [{old}] to ""{new}""", member.Kind, member, member.Item.Name);

        _ = Interlocked.Increment(ref _membersRenamed);

        if (member is IHasMemberRefData refData)
        {
            foreach (var memberRef in refData.Refs)
            {
                Log.Trace("Renaming member reference {MDToken}", memberRef.MDToken);
                memberRef.Name = member.Item.Name;
                var resolved = memberRef.Resolve();
                if (resolved != member.Item)
                {
                    Log.Error("member ref {typeRef} resolved to [{resolved}] ({token1}) instead of [{member}] ({token2})", memberRef.MDToken, resolved, resolved?.MDToken, member, member.Item.MDToken);
                    throw new InvalidOperationException("member ref resolve failed");
                }
            }
        }

        if (member is ITypeData type)
        {
            foreach (var typeRef in type.Refs)
            {
                Log.Trace("Renaming type reference {MDToken}", typeRef.MDToken);
                typeRef.Name = member.Item.Name;
                var resolved = typeRef.Resolve();
                if (resolved != type.Item)
                {
                    Log.Error("member ref {typeRef} resolved to [{resolved}] ({token1}) instead of [{member}] ({token2})", typeRef.MDToken, resolved, resolved?.MDToken, type, type.Item.MDToken);
                    throw new InvalidOperationException("member ref resolve failed");
                }
            }
        }
    }

    private void ProcessUnnestedType(ITypeData type)
    {
        if (type.WasTrimmed())
        {
            Log.Trace("Skipping [{0}] because it was trimmed", type);
            return;
        }
        Log.Trace("Processing Type [{0}]", type);

        if (Module.GlobalType == type.Item)
        {
            /*
            The global type must not be renamed from <Module>
            That name is part of the definition (See: ECMA-335 II.10.8)
            Renaming it will lead to undefined behavior in the runtime.

            So far I have only observed issue in the mono runtime:
            Renaming it can cause a System.TypeLoadException if you invoke a method via reflection in the mono runtime.

                System.TypeLoadException : Could not load type '' from assembly ''.
                Stack Trace:
                    at (wrapper managed-to-native) System.Reflection.RuntimeMethodInfo.InternalInvoke(System.Reflection.RuntimeMethodInfo,object,object[],System.Exception&)
                    at System.Reflection.RuntimeMethodInfo.Invoke (System.Object obj, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture)
            */
            Log.Trace("Skipping [{0}] because it is the global type", type);
            return;
        }

        Debug.Assert(!type.Item.IsNested);
        var oldFullName = type.Item.FullName;
        type.Item.Namespace = UTF8String.Empty;

        NamingHelper.Rename(type.Item, Scrambler.Settings.NamingScheme, _usedTypeNames);
        string newFullName = type.Item.FullName;
        Log.Trace("Renamed {old} to {new}", type, newFullName);

        var b = _usedTypeNames.Remove(oldFullName);
        Debug.Assert(b);
        b = _usedTypeNames.Add(newFullName);
        Debug.Assert(b, $"Failed for {type.FullName} (renamed to {newFullName})");

        _membersRenamed++;

        foreach (var typeRef in type.Refs)
        {
            Log.Trace("Renaming type reference {MDToken}", typeRef.MDToken);
            typeRef.Name = type.Item.Name;
            var resolved = typeRef.Resolve();
            if (resolved != type.Item)
            {
                Log.Error("type ref resolved to [{resolved}] ({token1}) instead of [{member}] ({token2})", resolved, resolved.MDToken, type.Item, type.Item.MDToken);
                Debug.Fail("type ref resolve failed");
            }
        }

        foreach (var genericParam in type.Item.GenericParameters)
        {
            Log.Trace(@"Renaming generic parameter ""{old}"" to ""{new}""", genericParam.Name, UTF8String.Empty);
            genericParam.Name = UTF8String.Empty;
        }
    }
}
