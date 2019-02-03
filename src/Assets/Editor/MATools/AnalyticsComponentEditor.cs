using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// CustomEditor for AnalyticsComponent
/// </summary>
[CustomEditor(typeof(AnalyticsComponent))]
public class AnalyticsComponentEditor : Editor
{
    // ReorderableList behaviour referenced from 
    //      http://answers.unity3d.com/questions/826062/re-orderable-object-lists-in-inspector.html
    //      and  http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/

    /// <summary>
    /// Limit of parameters which can be passed in a CustomEvent. Unity's default is 10. 
    /// </summary>
    private const int EventLimit = 10;

    /// <summary>
    /// the List of variables to track as part of an Analytics Event
    /// </summary>
    private ReorderableList eventList;

    /// <summary>
    /// reference to the AnalyticsComponent (the 'target' component)
    /// </summary>
    private AnalyticsComponent c;

    /// <summary>
    /// Toggles the 'Analytics Event' foldout
    /// </summary>
    private bool eventFoldout = true;

    /// <summary>
    /// Show a warning about event name lengths (when length == 0 || null)
    /// </summary>
    private bool showEventNameLengthWarning = false;

    /// <summary>
    /// Editor GUI Behaviour
    /// </summary>
    public override void OnInspectorGUI()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            serializedObject.Update();

            GUILayout.Space(4);

            // 'Tracking Enabled?'
            EditorGUILayout.PropertyField(serializedObject.FindProperty("trackingEnabled"), new GUIContent("Enable Tracking?"));

            // 'Event Trigger'
            List<GUIContent> triggerInfo = new List<GUIContent>(); // hold Type::Method listings for the 'Trigger Method' popup
            string[] mKeys = AnalyticsSettings.Settings.MethodKeys.ToArray(); // take all methodnames with [AnalyticsEvent] 
            string[] mValues = AnalyticsSettings.Settings.MethodValues.ToArray(); // pair the above methodnames with their Type

            // populate the triggerInfo list for the popup - pair types with their method names
            for (int i = 0; i < mKeys.Length; i++)
            {
                GUIContent gc = new GUIContent(mValues[i] + "::" + mKeys[i]);
                triggerInfo.Add(gc);
            }

            GUILayout.Space(4);
            serializedObject.FindProperty("triggerIdx").intValue = EditorGUILayout.Popup(
                new GUIContent("Trigger Method", AnalyticsSettings.Settings.MethodDescs[serializedObject.FindProperty("triggerIdx").intValue]), 
                serializedObject.FindProperty("triggerIdx").intValue, 
                triggerInfo.ToArray());
            GUILayout.Space(4);

            EditorGUILayout.HelpBox(
                AnalyticsSettings.Settings.MethodDescs[serializedObject.FindProperty("triggerIdx").intValue], MessageType.Info);

            // assign to TriggerMethodData the selected mKey/mVal
            c.TriggerMethod.MethodKey = mKeys[serializedObject.FindProperty("triggerIdx").intValue];
            c.TriggerMethod.MethodValue = mValues[serializedObject.FindProperty("triggerIdx").intValue];

            // error checking for event list (warn about 0-length names)
            showEventNameLengthWarning = false;

            foreach (AnalyticsData d in c.EventData)
            {
                if (d == null || d.EventName == null || d.EventName.Length == 0)
                {
                    showEventNameLengthWarning = true;
                }
            }

            // 'Analytics Event'
            eventFoldout = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), eventFoldout, "Analytics Event", true);
            if (eventFoldout)
            {
                GUILayout.Space(4);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("eventName"), new GUIContent("Event Name"));
                GUILayout.Space(4);

                try
                {
                    eventList.DoLayoutList();
                }
                catch (ArgumentOutOfRangeException)
                {
                    foreach (AnalyticsData d in c.EventData)
                    {
                        if (d.Index > c.StringMemberInfos.Count - 1)
                        {
                            d.Index = 0;
                        }
                    }
                }

                if (eventList.count >= EventLimit) // too many events
                {
                    EditorGUILayout.HelpBox("Analytics events support a maximum of " + EventLimit + " parameters.", MessageType.Warning);
                }

                if (showEventNameLengthWarning) // eventname length = 0
                {
                    EditorGUILayout.HelpBox("Event parameter names should not be null or empty. Leaving names blank or empty can cause an AnalyticsResult.InvalidData response during runtime.", MessageType.Warning);
                }
            }

            serializedObject.ApplyModifiedProperties(); // send back updated properties to the SerializedObject
        }
        else
        {
            EditorGUILayout.HelpBox("Exit Play mode before changing Analytics Component settings.", MessageType.Info);
        }
    }

    /// <summary>
    /// Initialization behaviour
    /// </summary>
    private void OnEnable()
    {
        // storing the (AnalyticsComponent)target OnEnable prevents casting every time
        c = (AnalyticsComponent)target;

        c.RetrieveMemberInfo();

        eventList = new ReorderableList(c.EventData, typeof(AnalyticsData), true, true, true, true);

        // assign callbacks for drawing header/elem and add/removing elems
        eventList.drawHeaderCallback += HeaderCallback;
        eventList.drawElementCallback += ElementCallback;
        eventList.onAddCallback += Add;
        eventList.onRemoveCallback += Remove;
    }

    /// <summary>
    /// Callback for drawing ReorderableList header
    /// </summary>
    /// <param name="r">Rect 2D</param>
    private void HeaderCallback(Rect r)
    {
        GUI.Label(r, GUIContent.none);
    }

    /// <summary>
    /// Callback for drawing ReorderableList element
    /// </summary>
    /// <param name="r">Rect 2D</param>
    /// <param name="idx">Index of the list element</param>
    /// <param name="active">Is the element active?</param>
    /// <param name="focus">Is the element focused?</param>
    private void ElementCallback(Rect r, int idx, bool active, bool focus)
    {
        AnalyticsData d = c.EventData[idx]; // index d into EventData list

        EditorGUI.BeginChangeCheck(); // watch for list changes

        // populate fields of each AnalyticsData object
        GUI.Label(new Rect(r.x, r.y, 40, r.height), new GUIContent("Name", "Name associated with a piece of event data"));
        d.EventName = EditorGUI.TextArea(new Rect(r.x + 40, r.y, 200, r.height), d.EventName);
        GUI.Label(new Rect(r.x + 250, r.y, 50, r.height), new GUIContent("Value", "The data associated with this component which will be logged"));
        d.Index = EditorGUI.Popup(new Rect(r.x + 300, r.y, r.width / 1.7f, r.height), d.Index, c.StringMemberInfos.ToArray());         

        if (EditorGUI.EndChangeCheck()) // finish watching for list changes, sanitize property strings + setdirty
        {
            // sanitize special chars in the name strings (no tabs, escaped null, line breaks...)
            d.EventName = AnalyticsFunctions.CleanPropertyString(d.EventName);
            d.EventInfo = c.MemberInfos[d.Index];

            // not using SerializedProperty to reference values on each list element - must manually set dirty to save any value changes
            EditorUtility.SetDirty(target); 
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene()); 
        }
    }

    /// <summary>
    /// Callback for adding ReorderableList elements
    /// </summary>
    /// <param name="l">the ReorderableList</param>
    private void Add(ReorderableList l)
    {
        if (l.list.Count < EventLimit)
        {
            c.EventData.Add(new AnalyticsData());
        }
        else
        {
            EditorUtility.DisplayDialog(string.Empty, "Analytics events are limited to " + EventLimit + " parameters.\n\nModify an existing parameter, or remove a parameter from the list before adding a new parameter.", "OK");
        }
    }

    /// <summary>
    /// Callback for removing ReorderableList elements
    /// </summary>
    /// <param name="l">the ReorderableList</param>
    private void Remove(ReorderableList l)
    {
        c.EventData.RemoveAt(l.index);
    }
}