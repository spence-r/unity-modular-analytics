using System;
using UnityEngine;

/// <summary>
/// Data item which contains paired AnalyticsEvent methods with their reflected Type
/// </summary>
[Serializable]
public class AnalyticsTrigger
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyticsTrigger"/> class.
    /// </summary>
    public AnalyticsTrigger()
    { }

    /// <summary>
    /// The MethodInfo name, sanitized to remove parameters, i.e.; ThisIsAMethod(string s) => ThisIsAMethod
    /// </summary>
    [SerializeField]
    private string methodKey;

    /// <summary>
    /// Gets or sets the MethodInfo name (without parameters)
    /// </summary>
    public string MethodKey
    {
        get { return methodKey; }
        set { methodKey = value; }
    }

    /// <summary>
    /// The reflected Type which we associate methodKey as being a member of.
    /// </summary>
    [SerializeField]
    private string methodValue;

    /// <summary>
    /// Gets or sets the reflected Type which we associate an <see cref="AnalyticsTrigger"/>.MethodKey as being a member of. 
    /// </summary>
    public string MethodValue
    {
        get { return methodValue; }
        set { methodValue = value; }
    }
}