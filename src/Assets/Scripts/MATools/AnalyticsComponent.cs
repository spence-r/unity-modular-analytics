using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Handles per-component behaviour for Modular Analytics tools.
/// </summary>
public class AnalyticsComponent : MonoBehaviour
{
    /// <summary>
    /// Backing field for MemberInfos property
    /// </summary>
    [SerializeField]
    [HideInInspector]
    private List<AnalyticsMemberInfo> memberInfos = new List<AnalyticsMemberInfo>();

    /// <summary>
    /// Gets or sets list of information about properties/fields belonging to all Components attached to the parent GameObject
    /// </summary>
    public List<AnalyticsMemberInfo> MemberInfos
    {
        get { return memberInfos; }
        set { memberInfos = value; }
    }

    /// <summary>
    /// Backing field for StringMemberInfos property
    /// </summary>
    [SerializeField]
    [HideInInspector]
    private List<string> stringMemberInfos = new List<string>();

    /// <summary>
    /// Gets or sets list of MemberInfos as string values - used for displaying popup information about MemberInfos in the CustomEditor for AnalyticsComponent.
    /// </summary>
    public List<string> StringMemberInfos
    {
        get { return stringMemberInfos; }
        set { stringMemberInfos = value; }
    }

    /// <summary>
    /// Backing field for TrackingEnabled property
    /// </summary>
    [SerializeField]
    [Tooltip("If enabled, this component will transmit its event data when the trigger method is called.")]
    private bool trackingEnabled = true;

    /// <summary>
    /// Gets or sets a value indicating whether this AnalyticsComponent should transmit its event data when told to Send()
    /// </summary>
    public bool TrackingEnabled
    {
        get { return trackingEnabled; }
        set { trackingEnabled = value; }
    }

    /// <summary>
    /// Backing field for EventName property
    /// </summary>
    [SerializeField]
    [Tooltip("The name which will be associated with the event transmission - usually related to the triggering method or game behaviour (e.g., OnPlayerSpawn)")]
    private string eventName;

    /// <summary>
    /// Gets or sets name of the AnalyticsEvent which will be sent to the Unity Analytics service.
    /// </summary>
    public string EventName
    {
        get { return eventName; }
        set { eventName = value; }
    }

    /// <summary>
    /// Backing field for EventData property
    /// </summary>
    [SerializeField]
    [HideInInspector]
    private List<AnalyticsData> eventData = new List<AnalyticsData>();

    /// <summary>
    /// Gets or sets list containing user-specified data about an Analytics Event.
    /// </summary>
    public List<AnalyticsData> EventData
    {
        get { return eventData; }
        set { eventData = value; }
    }

    /// <summary>
    /// Backing field for TriggerIdx property
    /// </summary>
    [SerializeField]
    private int triggerIdx = 0;

    /// <summary>
    /// Gets or sets index value for the TargetMethod editor dropdown.
    /// </summary>
    public int TriggerIdx
    {
        get { return triggerIdx; }
        set { triggerIdx = value; }
    }

    /// <summary>
    /// Backing field for TriggerMethodData property
    /// </summary>
    [SerializeField]
    private AnalyticsTrigger triggerMethod;

    /// <summary>
    /// Gets or sets the name of the method which will cause this AnalyticsComponent to fire its Send() event.
    /// </summary>
    public AnalyticsTrigger TriggerMethod
    {
        get { return triggerMethod; }
        set { triggerMethod = value; }
    }

    /// <summary>
    /// Transmit an Event to the Unity Analytics service.
    /// </summary>
    public void Send()
    {
        if (trackingEnabled)
        {
            Dictionary<string, object> d = BuildEventDictionary(); 
            AnalyticsManager.TransmitEvent(eventName, d, gameObject); // pass the event information to the Manager
        }
        else
        {   // I'm !trackingEnabled....don't send my information, but let the user know why
            Debug.LogWarning("MAComponent: Analytics Component attached to " + name + " was instructed to Send(), but has tracking disabled.");
        }
    }

    /// <summary>
    /// Construct a Dictionary(string, object) containing event metric information, for transmission to Unity Analytics service.
    /// </summary>
    /// <returns>the Dictionary(string, object) populated from the EventData information</returns>
    public Dictionary<string, object> BuildEventDictionary()
    {
        Dictionary<string, object> eventDataDict = new Dictionary<string, object>(); 

        // NOTE: A populated dictionary is not required by Unity analytics service. If this behaviour changes, uncomment the Exceptions below.
        if (EventData.Count == 0) // the user didn't specify any event data
        {
            ////throw new ArgumentException("The data list passed to BuildEventDictionary was empty.");
        }
        else if (EventData == null) // the event data was null
        {        
            ////throw new NullReferenceException("The data list passed to BuildEventDictionary was null.");
        }   
        else
        {
            // NOTE TODO: there is no validation of event data (e.g., checks on bogus indices, malformed names...) - 
            //  some sanitization of names occurs elsewhere, but both parameters should be verified and 100% ok before dictionary packing + transmission
            foreach (AnalyticsData d in EventData) 
            {
                eventDataDict.Add(d.EventName, GetReflectedData(d.EventInfo));
            }
        }

        return eventDataDict;
    }

    /// <summary>
    /// Get reflected data from fields/properties specified in the AnalyticsComponent event parameters.
    /// </summary>
    /// <param name="m">the AnalyticsMemberInfo which holds information about the target member</param>
    /// <returns>the data which is to be added to the Dictionary for an event transmission(string, object)</returns>
    public object GetReflectedData(AnalyticsMemberInfo m)
    {
        if (m == null) 
        {
            throw new NullReferenceException("MAComponent: An AnalyticsMemberInfo passed to GetReflectedData was null.");
        }

        // NOTE: see http://stackoverflow.com/questions/11107536/convert-string-to-type-in-c-sharp
        //      The qualified type string is also stored in the AnalyticsMemberInfo and used to GetType
        //      It may be necessary to also specify the Assembly in GetType(), depending on where the Send() events are placed (e.g., in Assemblies external to Assembly-CSharp)
        //          At present, use of external assemblies is NOT evaluated or supported by these tools....
        Type t = Type.GetType(m.QualifiedName);

        Component c = gameObject.GetComponent(t); // NOTE: Will only get the first Component found. TODO: More work needed to handle duplicate components - this is currently infrequent, and therefore unsupported
                                                  
        if (m.MemberType == MemberTypes.Field) // the member is a field
        {
            FieldInfo fi = c.GetType().GetField(m.Name); // construct a FieldInfo from the Component Type and field Name
            if (fi.GetValue(c) == null)
            {   // warning: the reflected field value was null.
                // this may be expected or normal behaviour (e.g., the property/field is an object reference)
                Debug.LogWarning("MAComponent: GetReflectedData FieldInfo was null - returning \"null\"");
                return "null";
            }
            else
            {   // Unity Analytics can sum, average, etc on numeric types 
                // if the field is a numeric type, cast to it before returning 
                if (fi.FieldType == typeof(sbyte))
                {
                    return (sbyte)fi.GetValue(c);
                }
                else if (fi.FieldType == typeof(byte))
                {
                    return (byte)fi.GetValue(c);
                }
                else if (fi.FieldType == typeof(char))
                {
                    return (char)fi.GetValue(c);
                }
                else if (fi.FieldType == typeof(short))
                {
                    return (short)fi.GetValue(c);
                }
                else if (fi.FieldType == typeof(ushort))
                {
                    return (ushort)fi.GetValue(c);
                }
                else if (fi.FieldType == typeof(int))
                {
                    return (int)fi.GetValue(c);
                }
                else if (fi.FieldType == typeof(uint))
                {
                    return (uint)fi.GetValue(c);
                }
                else if (fi.FieldType == typeof(long))
                {
                    return (long)fi.GetValue(c);
                }
                else if (fi.FieldType == typeof(ulong))
                {
                    return (ulong)fi.GetValue(c);
                }
                else if (fi.FieldType == typeof(float))
                {
                    return (float)fi.GetValue(c);
                }
                else if (fi.FieldType == typeof(double))
                {
                    return (double)fi.GetValue(c);
                }
                else if (fi.FieldType == typeof(decimal))
                {
                    return (decimal)fi.GetValue(c);
                }
                else if (fi.FieldType == typeof(bool))
                {
                    return (bool)fi.GetValue(c);
                }
                else // the field is not a numeric, floating-point, decimal, or boolean type 
                {    // so, return its string representation
                    return fi.GetValue(c).ToString();
                }
            }
        }
        else if (m.MemberType == MemberTypes.Property) // the member is a property
        {
            PropertyInfo pi = c.GetType().GetProperty(m.Name); // construct a PropertyInfo from the Component Type and property Name
            if (pi.GetValue(c, null) == null)
            {   // warning: the reflected field value was null.
                // this may be expected or normal behaviour (e.g., the property/field is an object reference)
                Debug.LogWarning("MAComponent: GetReflectedData PropertyInfo was null - returning \"null\"");
                return "null";
            }
            else
            {   // Unity Analytics can sum, average, etc on numeric types 
                //      if the property is a numeric type, cast to it before returning 
                if (pi.PropertyType == typeof(sbyte))
                {
                    return (sbyte)pi.GetValue(c, null);
                }
                else if (pi.PropertyType == typeof(byte))
                {
                    return (byte)pi.GetValue(c, null);
                }
                else if (pi.PropertyType == typeof(char))
                {
                    return (char)pi.GetValue(c, null);
                }
                else if (pi.PropertyType == typeof(short))
                {
                    return (short)pi.GetValue(c, null);
                }
                else if (pi.PropertyType == typeof(ushort))
                {
                    return (ushort)pi.GetValue(c, null);
                }
                else if (pi.PropertyType == typeof(int))
                {
                    return (int)pi.GetValue(c, null);
                }
                else if (pi.PropertyType == typeof(uint))
                {
                    return (uint)pi.GetValue(c, null);
                }
                else if (pi.PropertyType == typeof(long))
                {
                    return (long)pi.GetValue(c, null);
                }
                else if (pi.PropertyType == typeof(ulong))
                {
                    return (ulong)pi.GetValue(c, null);
                }
                else if (pi.PropertyType == typeof(float))
                {
                    return (float)pi.GetValue(c, null);
                }
                else if (pi.PropertyType == typeof(double))
                {
                    return (double)pi.GetValue(c, null);
                }
                else if (pi.PropertyType == typeof(decimal))
                {
                    return (decimal)pi.GetValue(c, null);
                }
                else if (pi.PropertyType == typeof(bool))
                {
                    return (bool)pi.GetValue(c, null);
                }
                else // the property is not a numeric, floating-point, decimal, or boolean type 
                {    // so, return its string representation
                    return pi.GetValue(c, null).ToString();
                }
            }
        }
        else    // the AnalyticsMemberInfo should always have its type as MemberTypes.Field or MemberTypes.Property - if we reached this point, then something went wrong..
        {
            throw new NotSupportedException("MAComponent: GetReflectedData can only operate with MemberTypes.Field or MemberTypes.Property types.");
        }
    }

    /// <summary>
    /// Retrieve AnalyticsMemberInfo information about the components attached to this object.
    /// </summary>
    public void RetrieveMemberInfo()
    {
        // clear out component lists (to avoid old component data being present)
        memberInfos.Clear(); 
        stringMemberInfos.Clear();

        // check each Component attached to the gameObject container
        foreach (Component c in gameObject.GetComponents(typeof(Component)))
        {
            Type t = c.GetType(); // get the component's Type

            if (t != typeof(AnalyticsComponent)) // NEVER check the AnalyticsComponent itself - its values will never need to be logged..
            {                                    // TODO: support user-customizable Type ignores? (e.g., ignore all properties of 'transform' type Components)

                PropertyInfo[] props = t.GetProperties(); // get all public properties
                FieldInfo[] fields = t.GetFields();       // get all public fields

                foreach (PropertyInfo p in props)   // check each property
                {
                    if (!MemberInfoIgnored(p.ToString())) // check property string against global ignores
                    {
                        memberInfos.Add(new AnalyticsMemberInfo(
                            p.ReflectedType.ToString(), p.PropertyType.ToString(), p.Name.ToString(), p.MemberType, p.ReflectedType.AssemblyQualifiedName));  
                    }
                }

                foreach (FieldInfo f in fields) // check each field
                {
                    if (!MemberInfoIgnored(f.ToString())) // check field string against global ignores
                    {
                        memberInfos.Add(new AnalyticsMemberInfo(
                            f.ReflectedType.ToString(), f.FieldType.ToString(), f.Name.ToString(), f.MemberType, f.ReflectedType.AssemblyQualifiedName));
                    }
                }
            }
        }

        FormatMemberInfoStrings(); // build the MemberInfo strings for the CustomEditor Popup
    }

    /// <summary>
    /// Format entries in the memberInfos list for display in the CustomEditor popup
    /// </summary>
    public void FormatMemberInfoStrings()
    {
        ////int i = 0; 
        foreach (AnalyticsMemberInfo m in memberInfos)
        {
            string[] reflectedTypeStr = AnalyticsFunctions.StripNamespace(m.ReflectedType);

            // NOTE: to append the idx - add "[" + i + "] " + to fString and uncomment int i/i++
            string formattedString = reflectedTypeStr[reflectedTypeStr.GetUpperBound(0)] + ": " + m.Name + " (" + m.Type + ")";
            stringMemberInfos.Add(formattedString);
            ////i++;
        }
    }

    /// <summary>
    /// Check whether the global ignores list contains a MemberInfo string.
    /// </summary>
    /// <param name="s">the MemberInfo string which will be evaluated against the ignore list</param>
    /// <returns>true if the list contains the string</returns>
    public bool MemberInfoIgnored(string s)
    {
        if (AnalyticsSettings.Settings.MemberInfoIgnores.Contains(s))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}