using dnlib.DotNet;

namespace Vilens.Data;

/// <summary>
/// All possible types of <see cref="IMemberDef"/>
/// </summary>
internal enum MemberKind
{
    /// <summary>
    /// <see cref="TypeDef"/>
    /// </summary>
    Type,
    /// <summary>
    /// <see cref="MethodDef"/>
    /// </summary>
    Method,
    /// <summary>
    /// <see cref="FieldDef"/>
    /// </summary>
    Field,
    /// <summary>
    /// <see cref="PropertyDef"/>
    /// </summary>
    Property,
    /// <summary>
    /// <see cref="EventDef"/>
    /// </summary>
    Event,
    /// <summary>
    /// <see cref="GenericParam"/>
    /// </summary>
    GenericParameter,
    /// <summary>
    /// ???
    /// </summary>
    Unknown
}
