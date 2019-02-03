using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

/// <summary>
/// Contains wrappers enabling timing of execution of Analytics functions
/// </summary>
public static class TimingFunctions
{
    /// <summary>
    /// Fixed path for TimedNotify log file
    /// </summary>
    private static readonly string timedNotifyPath = "C:\\timedNotify.csv";

    /// <summary>
    /// Fixed path for TimedCE log file
    /// </summary>
    private static readonly string timedCEPath = "C:\\timedCE.csv";

    /// <summary>
    /// StreamWriter for TimedNotify
    /// </summary>
    private static StreamWriter notifySW = new StreamWriter(timedNotifyPath, true); // true == append

    /// <summary>
    /// StreamWriter for TimedCustomEvent
    /// </summary>
    private static StreamWriter ceSW = new StreamWriter(timedCEPath, true); // true == append

    /// <summary>
    /// Stopwatch to time execution
    /// </summary>
    private static System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();

    /// <summary>
    /// Stopwatch wrapper for AnalyticsManager.Notify method
    /// </summary>
    /// <param name="name">name param for Notify method</param>
    /// <param name="type">type param for Notify method</param>
    public static void TimedNotify(string name, string type)
    {
        w.Start();
        AnalyticsManager.Notify(name, type);
        w.Stop();

        //// log to console + csv file, flush StreamWriter, and reset
        Debug.Log("TimingFunctions: TimedNotify call took " + w.ElapsedMilliseconds + " ms");
        notifySW.WriteLine("timednotify\t" + w.ElapsedMilliseconds);
        notifySW.Flush();
        w.Reset();
    }

    /// <summary>
    /// Stopwatch wrapper for Analytics.CustomEvent method
    /// </summary>
    /// <param name="o">the GameObject which is sending the event</param>
    public static void TimedCustomEvent(GameObject o)
    {
        //// get reference to the attached AnalyticsComponent
        AnalyticsComponent c = (AnalyticsComponent)o.GetComponent(typeof(AnalyticsComponent));

        w.Start();

        /* code to populate dictionary from GameObject goes here - must be modified for every test procedure */

        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("transformForward", o.transform.forward.ToString());
        dict.Add("playerPosition", o.transform.position.ToString());
        dict.Add("eulerAngles", o.transform.eulerAngles.ToString());
        dict.Add("localEuler", o.transform.localEulerAngles.ToString());
        dict.Add("transformName", o.transform.name);
        dict.Add("transformTag", o.transform.tag);

        //// return the AnalyticsResult code
        Debug.Log("TimingFunctions: TimedCustomEvent returned code " + Analytics.CustomEvent(c.EventName, dict));

        w.Stop();

        //// log to console + csv file, flush StreamWriter, and reset
        Debug.Log("TimingFunctions: TimedCustomEvent call took " + w.ElapsedMilliseconds + " ms");
        ceSW.WriteLine("timedcustomevent\t" + w.ElapsedMilliseconds);
        ceSW.Flush();
        w.Reset();
    }
}