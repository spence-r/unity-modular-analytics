using System;

/// <summary>
/// Attribute used to define methods which are capable of firing an AnalyticsEvent.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AnalyticsEvent : Attribute
{
    /// <summary>
    /// Backing field for the Description property
    /// </summary>
    private string description;

    /// <summary>
    /// Gets or sets a description of the method (displayed within the AnalyticsComponent)
    /// </summary>
    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    /// <summary>
    /// Initializes a new instance of the AnalyticsEvent class.
    /// </summary>
    /// <param name="desc">a short description of the method displayed within an AnalyticsComponent</param>
    public AnalyticsEvent(string desc)
    {
        Description = desc;
    }
}