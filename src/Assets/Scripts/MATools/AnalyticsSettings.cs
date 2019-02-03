using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Global settings for Modular Analytics tools
/// </summary>
[Serializable]
public class AnalyticsSettings : ScriptableObject
{
    /// <summary>
    /// Prevents a default instance of the <see cref="AnalyticsSettings"/> class from being created.
    /// </summary>
    private AnalyticsSettings()
    { }

    /// <summary>
    /// Gets or sets the global AnalyticsSettings object
    /// </summary>
    public static AnalyticsSettings Settings
    {
        get // look for a settings file. if there isn't one, create it and assign it to settings    
        {   // also, populate the targetassemblies List from the list of defaults
            if (AssetDatabase.LoadAssetAtPath(AssetPath, typeof(AnalyticsSettings)) == null)
            {
                // this is a ScriptableObject, so use CreateInstance<T> instead of the ctor
                settings = CreateInstance<AnalyticsSettings>();

                // NOTE: Exceptions thrown here can cause a hang, because we need to read from the .asset file and the ScriptableObject
                //     In testing, this has only occurred when the script is modified + reloaded while the editor is running  
                try
                {
                    // create the settings folder if it doesn't exist in the project environment
                    if (!AssetDatabase.IsValidFolder("Assets/Editor/MATools/Settings"))
                    {
                        Debug.Log("MASettings: Creating settings folder at Assets/Editor/MATools/Settings/");
                        AssetDatabase.CreateFolder("Assets/Editor/MATools", "Settings");
                    }

                    // build the settings .asset file if it didn't exist already
                    AssetDatabase.CreateAsset(settings, AssetPath); 
                }
                catch (Exception e)
                {
                    if (e is NullReferenceException)
                    {
                        Debug.LogError("MASettings: Loading the Analytics Settings threw a NullReferenceException. Close and restart the Unity Editor.");
                    }
                }

                settings.ResetDefaultAssemblies();
            }
            else
            {   // return the settings from the loaded .asset file
                settings = (AnalyticsSettings)AssetDatabase.LoadAssetAtPath(
                    AssetPath, typeof(AnalyticsSettings));
            }

            return settings;
        }

        set
        {
            settings = value;
        }
    }

    /// <summary>
    /// Location of stored settings for the AnalyticsSettings object
    /// </summary>
    private const string AssetPath = "Assets/Editor/MATools/Settings/AnalyticsSettings.asset";

    /// <summary>
    /// List of default/standard assemblies to load
    /// </summary>
    private List<string> defaultAssemblies = new List<string>
    {
        "Assembly-CSharp", "Assembly-CSharp-Editor"
    };

    /// <summary>
    /// Backing field for MemberInfoIgnores property
    /// </summary>
    [SerializeField]
    private List<string> memberInfoIgnores = new List<string>()
    {
        ////"System.String name",
        ////"System.String tag",
        "UnityEngine.GameObject gameObject",
        "UnityEngine.Component rigidbody",
        "UnityEngine.Component rigidbody2D",
        "UnityEngine.Component camera",
        "UnityEngine.Component light",
        "UnityEngine.Component animation",
        "UnityEngine.Component constantForce",
        "UnityEngine.Component renderer",
        "UnityEngine.Component audio",
        "UnityEngine.Component guiText",
        "UnityEngine.Component networkView",
        "UnityEngine.Component guiElement",
        "UnityEngine.Component guiTexture",
        "UnityEngine.Component collider",
        "UnityEngine.Component collider2D",
        "UnityEngine.Component hingeJoint",
        "UnityEngine.Component particleEmitter",
        "UnityEngine.Component particleSystem",
        "UnityEngine.HideFlags hideFlags",
        "UnityEngine.HideFlags None",
        "UnityEngine.HideFlags HideInHierarchy",
        "UnityEngine.HideFlags HideInInspector",
        "UnityEngine.HideFlags DontSaveInEditor",
        "UnityEngine.HideFlags NotEditable",
        "UnityEngine.HideFlags DontSaveInBuild",
        "UnityEngine.HideFlags DontUnloadUnusedAsset",
        "UnityEngine.HideFlags DontSave",
        "UnityEngine.HideFlags HideAndDontSave"
    };

    /// <summary>
    /// Gets or sets the list of MemberInfo strings which will be ignored by the AnalyticsComponent
    /// </summary>
    public List<string> MemberInfoIgnores
    {
        get { return memberInfoIgnores; }
        set { memberInfoIgnores = value; }
    }

    /// <summary>
    /// Backing field for NeverLoaded property
    /// </summary>
    [SerializeField]
    private bool neverLoaded = true;

    /// <summary>
    /// Gets or sets a value indicating whether the AnalyticsSettings had their first-time setup or not
    /// </summary>
    public bool NeverLoaded
    {
        get { return neverLoaded; }
        set { neverLoaded = value; }
    }

    /// <summary>
    /// Backing field for Offline property
    /// </summary>
    [SerializeField]
    private bool offline = true;

    /// <summary>
    /// Gets or sets a value indicating whether the Modular Analytics tools should run in 'offline mode' (no transmission of data to Unity Analytics service)
    /// </summary>
    public bool Offline
    {
        get { return offline; }
        set { offline = value; }
    }

    /// <summary>
    /// Backing field for the global AnalyticsSettings
    /// </summary>
    private static AnalyticsSettings settings;

    /// <summary>
    /// Backing field for TargetAssemblies property
    /// </summary>
    [SerializeField]
    private List<string> targetAssemblies = new List<string>();

    /// <summary>
    /// Gets a List of user-defined assemblies to search when looking for Analytics-enabled methods
    /// </summary>
    public List<string> TargetAssemblies
    {
        get { return targetAssemblies; }
        private set { }
    }

    /// <summary>
    /// Backing field for Assemblies property
    /// </summary>
    [SerializeField]
    private List<string> assemblies = new List<string>();

    /// <summary>
    /// Gets a List of Assemblies loaded from the user-specified strings, or the List of defaults
    /// </summary>
    public List<string> Assemblies
    {
        get { return assemblies; }
        private set { }
    }

    /// <summary>
    /// Serializable backing field for MethodKeys property
    /// </summary>
    [SerializeField]
    private List<string> methodKeys = new List<string>();

    /// <summary>
    /// Gets or sets a List of 'keys' for a dictionary of methods with Analytics-enabled attributes
    /// </summary>
    public List<string> MethodKeys
    {
        get { return methodKeys; }
        set { methodKeys = value; }
    }

    /// <summary>
    /// Serializable backing field for MethodValues property
    /// </summary>
    [SerializeField]
    [HideInInspector]
    private List<string> methodValues = new List<string>();

    /// <summary>
    /// Gets or sets a List of 'values' for a dictionary of methods with Analytics-enabled attributes
    /// </summary>
    [HideInInspector]
    public List<string> MethodValues
    {
        get { return methodValues; }
        set { methodValues = value; }
    }

    /// <summary>
    /// Backing field for MethodDescs property 
    /// </summary>
    [SerializeField]
    private List<string> methodDescs = new List<string>();

    /// <summary>
    /// Gets or sets the list of method descriptors
    /// </summary>
    [HideInInspector]
    public List<string> MethodDescs
    {
        get { return methodDescs; }
        set { methodDescs = value; }
    }
    
    /// <summary>
    /// Read user specified Assembly names and load the corresponding Assembly into Assemblies property
    /// </summary>
    public void LoadAssemblies()
    {
        // Using a set of target assemblies is faster, but users must specify each assembly
        // (if they load in custom/third-party assemblies containing methods with [AnalyticsEvent] attributes)
        // The previous approach used System.AppDomain.CurrentDomain.GetAssemblies() - this was VERY slow as 
        // it had to iterate all assemblies, including some like System which would never have an [AnalyticsEvent] method.
        foreach (string s in Settings.targetAssemblies)
        {
            try
            {   // test loading the Assembly
                Assembly a = Assembly.Load(s);

                // store the loaded assembly only if it wasn't loaded already
                if (!assemblies.Contains(a.ToString()))
                {
                    assemblies.Add(a.ToString());
                    Debug.Log("MASettings: Loading assembly " + a.ToString());
                }
                else
                {   // we won't load this because the string was a duplicate
                    Debug.LogWarning("MASettings: Skipping load of assembly \"" + AnalyticsFunctions.GetAssemblyShortName(a.FullName.ToString())
                        + "\" - the assembly was already loaded.");
                }
            }
            catch (Exception e) // the assembly was bad - don't load it and warn the user
            {
                if (e is FileNotFoundException ||
                    e is IOException || e is ArgumentNullException)
                {
                    Debug.LogError("MASettings: The specified assembly string \""
                        + s + "\" is null, or could not be loaded.");
                }
                else if (e is BadImageFormatException)
                {
                    Debug.LogError("MASettings: The specified assembly string \""
                        + s + "\" is not a valid assembly.");
                }
            }
        }

        LoadMethods();
    }

    /// <summary>
    /// Removes assemblies which are in the List of 'loaded' assemblies, but not the List of targets
    /// </summary>
    public void CleanAssemblies()
    {
        List<string> removes = new List<string>(); // List of Assemblies to remove from the currently loaded List

        foreach (string s in settings.assemblies) // check all Assembly strings in assemblies
        {
            if (!settings.targetAssemblies.Contains(AnalyticsFunctions.GetAssemblyShortName(s)))
            {   // remove s from loaded assemblies if not in the List of target assemblies
                removes.Add(s);
                Debug.Log("MASettings: Removing loaded assembly: \"" + s + "\"");
            }
        }

        foreach (string s in removes)  
        {
            settings.assemblies.RemoveAll(rem => rem == s); // remove all strings in assemblies which == the entry in 'removes' List
        }

        LoadMethods(); // update the List of loaded methods with [AnalyticsEvent]
    }

    /// <summary>
    /// Load method descriptors from their attributes
    /// </summary>
    public void LoadMethodDescs()
    {
        methodDescs.Clear();

        int i = 0;
        foreach (string s in MethodKeys) // iterate through MethodKeys
        {
            Type t = Type.GetType(MethodValues[i]);
            AnalyticsEvent e = (AnalyticsEvent)t.GetMethod(s, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).GetCustomAttributes(typeof(AnalyticsEvent), true)[0];
            MethodDescs.Add(e.Description);
            i++;
        }
    }

    /// <summary>
    /// Load MethodInfo for each method which is Analytics-enabled, 
    /// and store the reflected information to MethodKeys/MethodValues properties
    /// </summary>
    public void LoadMethods()
    {
        methodKeys.Clear(); // clear existing methodinfo k/vs
        methodValues.Clear();
        foreach (string a in Settings.Assemblies)
        {
            Assembly ay = Assembly.Load(a);

            foreach (Type t in ay.GetTypes()) // check each Type in the assembly
            {
                MethodInfo[] methodInfos = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static); // get all Methods from the current Type

                foreach (MethodInfo m in methodInfos) // check each Method for an AnalyticsEvent attribute
                {
                    if (m.GetCustomAttributes(typeof(AnalyticsEvent), true).Length > 0)
                    {
                        ////Debug.Log("Storing method with AnalyticsEvent attribute: " + m.ToString() + ", has reflected type " + m.ReflectedType.ToString());

                        string mK = AnalyticsFunctions.StripReturnType(m.ToString())[1];
                        mK = AnalyticsFunctions.StripMethodParameters(mK)[0];

                        methodKeys.Add(mK); // stores the method which had attribute [AnalyticsEvent]
                        methodValues.Add(m.ReflectedType.ToString()); // stores the Type containing the method above
                    }
                }
            }
        }

        LoadMethodDescs();
    }

    /// <summary>
    /// Reset the Assemblies property to a default/clean state (using Assembly-CSharp and Assembly-Editor-CSharp only)
    /// </summary>
    public void ResetDefaultAssemblies()
    {
        TargetAssemblies.Clear(); // clear the user-specified Assemblies

        foreach (string a in defaultAssemblies) // now load+add each default from the List
        {
            TargetAssemblies.Add(a);
        }
    }
}