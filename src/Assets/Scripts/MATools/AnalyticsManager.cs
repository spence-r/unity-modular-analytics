using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

/// <summary>
/// Handles transmission of Analytics messages to and from the Unity Analytics service
/// </summary>
public static class AnalyticsManager
{
    /// <summary>
    /// Transmit an Analytics Event to the Unity Analytics service
    /// </summary>
    /// <param name="eventName">the name of the Event to transmit (will be used for display and indexing within the Unity service)</param>
    /// <param name="eventParams">the parameters to associate with the Event (keyed by name)</param>
    /// <param name="sender">the GameObject which called TransmitEvent()</param>
    public static void TransmitEvent(string eventName, Dictionary<string, object> eventParams, GameObject sender)
    {
        // IMPORTANT NOTE: Use DebugLogResult for offline testing (to avoid eating up client events + tracking points within the Analytics service) 
        if (!AnalyticsSettings.Settings.Offline)
        {
            LogResult(Analytics.CustomEvent(eventName, eventParams), eventName, sender);
        }
        else // always DebugLogResult unless explicitly set to !offline
        {
            DebugLogResult(eventName, eventParams, sender);
        }
    }

    /// <summary>
    /// Notify the Modular Analytics system that an event is being sent from className.methodName()
    /// </summary>
    /// <param name="methodName">the name of the method which is firing an event</param>
    /// <param name="className">the name of the class to which the method belongs</param>
    public static void Notify(string methodName, string className)
    {
        AnalyticsComponent[] components = Object.FindObjectsOfType<AnalyticsComponent>(); // find all AnalyticsComponent as Component

        foreach (AnalyticsComponent c in components)
        {
            // the AnalyticsComponent target matches this method + classname - call the Component's Send() method
            if (c.TriggerMethod.MethodValue == className && c.TriggerMethod.MethodKey == methodName)
            {
                Debug.Log("MAManager: Notifying object " + c.name.ToString() + " to fire its Send() event");
                c.Send();
            }
        }
    }

    /// <summary>
    /// Log the result of an Analytics Event transmission
    /// </summary>
    /// <param name="r">the AnalyticsResult returned from the Unity service</param>
    /// <param name="eventName">the name of the Event which was transmitted</param>
    /// <param name="sender">the GameObject which transmitted an event</param>
    private static void LogResult(AnalyticsResult r, string eventName, GameObject sender)
    {
        Debug.Log("MALogResult: " + sender.name.ToString() + " sending event " + eventName + " returned code " + r.ToString());
    }

    /// <summary>
    /// Locally log the result of an Analytics Event transmission for offline debugging
    /// </summary>
    /// <param name="eventName">the name of the Event which was transmitted</param>
    /// <param name="eventDict">the Dictionary(string, object) containing event information</param>
    /// <param name="sender">the GameObject which transmitted an event</param>
    private static void DebugLogResult(string eventName, Dictionary<string, object> eventDict, GameObject sender)
    {
        Debug.Log("MADebugLogResult: " + sender.name.ToString() + " sending event with name \"" + eventName + "\" , dumping the event Dictionary data: ");

        if (eventDict.Count == 0)   // if Unity decides to disallow null dictionaries or empty dictionaries, then 
        {
            Debug.LogWarning("MADebugLogResult: " + sender.name.ToString() + " sent an empty dictionary. Was this your intended behaviour?");
            ////throw new System.ArgumentException("The event dictionary was empty. Pass a dictionary with at least one value to DebugLogResult, or provide a null reference.");
        }
        else if (eventDict == null)
        {
            Debug.LogWarning("MADebugLogResult: " + sender.name.ToString() + " sent a null dictionary reference. Was this your intended behaviour?");
            ////throw new System.NullReferenceException("The event dictionary reference was null. Pass a non-null reference to a Dictionary<string, object> to DebugLogResult.");
        }
        
        // only iterate kvps if there are kvps to iterate...
        if (eventDict != null && eventDict.Count != 0)
        {
            int i = 1;
            foreach (KeyValuePair<string, object> kvp in eventDict)
            {
                Debug.Log("MADebugLogResult: " + sender.name + " AnalyticsComponent entry " + i + ": " + kvp.Key + ", " + kvp.Value.ToString());
                ++i;
            }
        }
    }
}