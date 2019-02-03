using System;
using UnityEngine;

/// <summary>
/// Data which comprises an element within a Dictionary used in an Analytics Event
/// </summary>
[Serializable]
public class AnalyticsData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyticsData"/> class.
    /// </summary>
    public AnalyticsData()
    { }

    /// <summary>
    /// Backing field for EventName property
    /// </summary>
    [SerializeField]
    private string eventName;

    /// <summary>
    /// Gets or sets the name representing this event metric
    /// </summary>
    public string EventName
    {
        get { return eventName; }
        set { eventName = value; }
    }

    /// <summary>
    /// Backing field for EventInfo property
    /// </summary>
    [SerializeField]
    private AnalyticsMemberInfo eventInfo;

    /// <summary>
    /// Gets or sets the associated AnalyticsMemberInfo (information used to collect the event value with Reflection during runtime)
    /// </summary>
    public AnalyticsMemberInfo EventInfo
    {
        get { return eventInfo; }
        set { eventInfo = value; }
    }

    /// <summary>
    /// Backing field for Index property
    /// </summary>
    [SerializeField]
    private int index = 0;

    /// <summary>
    /// Gets or sets the selection index for the eventInfo editor popup
    /// </summary>
    public int Index
    {
        get { return index; }
        set { index = value; }
    }
}