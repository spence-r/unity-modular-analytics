using System;
using System.Reflection;
using UnityEngine;

/// <summary>
///  A serializable container for MemberInfo data for the Modular Analytics tools
/// </summary>
[Serializable]
public class AnalyticsMemberInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyticsMemberInfo"/> class. 
    /// </summary>
    /// <param name="infoReflectedType">the ReflectedType from MethodInfo</param>
    /// <param name="infoType">the Type to which the member belongs</param>
    /// <param name="infoName">the member's name</param>
    /// <param name="infoMemberType">the type of the member (MemberType.Property or MemberType.Field)</param>
    /// <param name="infoQualifiedName">the assembly-qualified name</param>
    public AnalyticsMemberInfo(string infoReflectedType, string infoType, string infoName, MemberTypes infoMemberType, string infoQualifiedName)
    {
        ReflectedType = infoReflectedType;
        Type = infoType;
        Name = infoName;
        MemberType = infoMemberType;
        QualifiedName = infoQualifiedName;
    }

    /// <summary>
    /// Backing field for ReflectedType property
    /// </summary>
    [SerializeField]
    private string reflectedType;

    /// <summary>
    /// Gets or sets the reflected type of the member info
    /// </summary>
    public string ReflectedType
    {
        get { return reflectedType; }
        set { reflectedType = value; }
    }

    /// <summary>
    /// Backing field for Type property
    /// </summary>
    [SerializeField]
    private string type;

    /// <summary>
    /// Gets or sets the member info type
    /// </summary>
    public string Type
    {
        get { return type; }
        set { type = value; }
    }

    /// <summary>
    /// Backing field for Name property
    /// </summary>
    [SerializeField]
    private string name;

    /// <summary>
    /// Gets or sets the name of the member
    /// </summary>
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    /// <summary>
    /// Backing field for MemberType property
    /// </summary>
    [SerializeField]
    private MemberTypes memberType;

    /// <summary>
    /// Gets or sets the MemberType of the member
    /// </summary>
    public MemberTypes MemberType
    {
        get { return memberType; }
        set { memberType = value; }
    }

    /// <summary>
    /// Backing field for QualifiedName property
    /// </summary>
    [SerializeField]
    private string qualifiedName;

    /// <summary>
    /// Gets or sets the assembly-qualified name of the member
    /// </summary>
    public string QualifiedName
    {
        get { return qualifiedName; }
        set { qualifiedName = value; }
    }
}