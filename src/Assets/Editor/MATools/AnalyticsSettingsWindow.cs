using UnityEditor;
using UnityEngine;

/// <summary>
/// EditorWindow for changing global AnalyticsSettings
/// </summary>
[InitializeOnLoad]
public class AnalyticsSettingsWindow : EditorWindow
{
    /// <summary>
    /// The scrollbar position
    /// </summary>
    private Vector2 scrollbar; 

    /// <summary>
    /// Initializes static members of the <see cref="AnalyticsSettingsWindow"/> class.
    /// </summary>
    static AnalyticsSettingsWindow()
    {
        if (AnalyticsSettings.Settings.NeverLoaded)
        {   // load the assemblies if they were never loaded previously
            AnalyticsSettings.Settings.LoadAssemblies();
            AnalyticsSettings.Settings.NeverLoaded = false;
        }
    }

    /// <summary>
    /// Initialize and display the window
    /// </summary>
    [MenuItem("Edit/Analytics Settings", false, 10000)]
    private static void Initialize()
    {
        AnalyticsSettingsWindow w = (AnalyticsSettingsWindow)GetWindow(typeof(AnalyticsSettingsWindow), true, "Analytics Settings", true);
        w.Show();
    }

    /// <summary>
    /// Window update behaviour
    /// </summary>
    private void OnGUI()
    {
        if (EditorApplication.isPlaying) // lock the settings if editor is in Play mode
        {
            EditorGUILayout.HelpBox("Exit Play mode before editing any Analytics settings.", MessageType.Warning);
        }
        else
        {
            scrollbar = EditorGUILayout.BeginScrollView(scrollbar, false, false);

            // serialize the Settings object
            SerializedObject o = new SerializedObject(AnalyticsSettings.Settings);

            // run offline?
            SerializedProperty runOffline = o.FindProperty("offline");
            EditorGUILayout.PropertyField(runOffline, true);

            GUILayout.Label("Assemblies", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Specify assemblies to examine when searching for methods with an [AnalyticsEvent] attribute.", MessageType.Info);

            // list of Assemblies
            SerializedProperty assemblyList = o.FindProperty("targetAssemblies");
            EditorGUILayout.PropertyField(assemblyList, true);

            // list of loaded Assemblies (non-editable)
            GUI.enabled = false;
            SerializedProperty loadedAssemblies = o.FindProperty("assemblies");
            EditorGUILayout.PropertyField(loadedAssemblies, true);
            GUI.enabled = true;

            GUILayout.Space(8);

            // "Load" button
            if (GUILayout.Button("Load Assemblies"))
            {
                AnalyticsSettings.Settings.LoadAssemblies();
            }

            // "Clean" button
            if (GUILayout.Button("Clean Assemblies"))
            {
                AnalyticsSettings.Settings.CleanAssemblies();
            }

            // "Reset" button
            if (GUILayout.Button("Reset to Default Assemblies"))
            {
                AnalyticsSettings.Settings.Assemblies.Clear();
                AnalyticsSettings.Settings.ResetDefaultAssemblies();
                AnalyticsSettings.Settings.LoadAssemblies();
            }

            /* // NOTE: the AnalyticsComponent needs to be modified if this is re-enabled
               //   the Component needs to know when to refresh/update its own list of memberinfos and compare them against the global ignore list
               //       this implementation here also needs a ReorderableList with add/remove controls - the Array controls are too cumbersome
            GUILayout.Space(8);

            GUILayout.Label("MemberInfo Ignore List", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Specify fields or properties which will be ignored by the AnalyticsComponent dropdowns. Format is \"Namespace.Type memberName\".", MessageType.Info);

            SerializedProperty ignoresList = o.FindProperty("memberInfoIgnores");
            EditorGUILayout.PropertyField(ignoresList, true);
            */

            o.ApplyModifiedProperties(); // send back any updated values to the SerializedObject

            GUILayout.Space(16);

            if (GUILayout.Button("Save Settings"))
            {
                EditorUtility.SetDirty(AnalyticsSettings.Settings);
            }

            /* // NOTE: mostly useful for debugging 
               //   to be particularly functional, this should display in the format Namespace.Class.MethodName 
               // (otherwise it can be confusing, especially if methods in different classes/namespaces share a name - e.g., Test.Foo, Bar.Foo...)
            GUILayout.Label("Analytics-enabled Methods", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("These methods were identified as having an [AnalyticsEvent] attribute.", MessageType.Info);

            // List of loaded methodinfo strings (readonly list)
            GUI.enabled = false;
            SerializedProperty loadedMethods = o.FindProperty("methodKeys");
            EditorGUILayout.PropertyField(loadedMethods, true);
            GUI.enabled = true;
            */

            EditorGUILayout.EndScrollView();
        }
    }

    /// <summary>
    /// Window close behaviour
    /// </summary>
    private void OnDestroy()
    {
        // do nothing
    }
}