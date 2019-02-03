# Modular Analytics Tools for Unity
This Unity package provides a component-based interface for the Unity analytics service.

It was created as part of my final project in the Creative Technologies BSc program at Bangor University. It was submitted in partial satisfaction of the degree requirements in May, 2016.

The reporting component works by first examining the project's assemblies (or target assemblies), and allowing the target object to log the state of member variables from any attached component. 

In the end, a completely modular approach to analytics reporting seems unnecessary, so I think this package is limited in usefulness and therefore it's provided here for free. 

## Readme
This guide will walk you the process of setting up a Unity project for analytics collection, and importing and setting up the Modular Analytics package. It is recommended that users have a basic understanding of the Unity editor, and the Unity analytics service. Basic functionality of the Unity editor, and limitations and specifications of the Unity analytics service will not be covered within this guide. 

## Integration 
- --
##### Unity Setup and Import
To begin, ensure that you have enabled the analytics service for your Unity project, by following the instructions provided [here](http://docs.unity3d.com/Manual/UnityAnalyticsOverview.html). This will associate your project with an identifier for Unity services, and also load the required `UnityEngine.Analytics` classes.

Next, the assets must be imported. Select the "Assets > Import Package > Custom Package" menu option.

![Unity Editor - Import Custom Package](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_custom_pkg.jpg)

In the dialog which opens, navigate to and select the *ModularAnalytics.asset* file. Note that it is not necessary to import the *ModularAnalytics_TestTools.asset* file. An import dialog will open: 

![Unity Editor - Import ModularAnalytics Package](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_pkg_import.jpg)

Ensure that all of the files are selected (by pressing "All", if necessary). Press "Import". In the project pane, the script files will now be visible:

![Unity Editor - Post-Import ModularAnalytics Package](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_project_postimport.jpg)

The assets have now been imported. You may have also seen some output in the console, similar to what is depicted below: 

![AnalyticsSettings Messages](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_settings_import.jpg)

This confirms that the settings package loaded correctly. If you received ouput similar to what is displayed below, however, then verify that the Unity analytics service was enabled (as discussed in the first paragraph of this section). If this step was omitted, then the tools will not function correctly. 

![Unity Can't Find Analytics Namespace](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_console_noanalyticsnamespace.jpg)

- --
##### Verifying Settings

Once the asset package has been imported and the analytics service enabled, a new menu option should be available. Its position may vary, depending on whether you have installed additional editor scripts. 

![Analytics Settings Entry](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_analyticssettings.jpg)

Select this option, and a new dialog box will open in the Unity editor.

![Analytics Settings Dialog](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_analyticssettingsbox.jpg)

Expand the "Target Assemblies" and "Assemblies" by clicking the triangle next to the name. If your dialog looks similar to what is depicted above, great! Press the "Save Settings" button, and close the dialog. The assemblies will be remembered for 

If the "Target Assemblies" and "Assemblies" lists are empty, however, press the "Reset to Default Assemblies" button. The lists should become populated. Then, press the "Save Settings" button and close the dialog. 

- --
##### Integration Into Your Project's Code

The functionality of this system relies on "triggers" for sending analytics data to the Unity service. These triggers can be specified by _any_ method of your choice. However, there is no point in specifying methods which will not be called during a game's runtime, as the trigger will never be fired!

Specifying a method which can act as a trigger point is simple, but requires two steps which work in tandem. These are illustrated below:

```cs
    [AnalyticsEvent("A short description of the trigger point.")]
    public void YourMethod()
    {
        // your method might have some code here
        
        AnalyticsManager.Notify("YourMethod", "YourClassname");
        
        // your method might have other code here
    }
```

- A method which can act as an analytics trigger must be flagged with an _attribute_ called `AnalyticsEvent`. This attribute takes a single parameter, which provides a user-readable description of the trigger method. You can use this space to specify the circumstances under which the event might execute, or provide a description of the method's behaviour. 
- Any method with an `AnalyticsEvent` attribute must also have, within its body, a call to `AnalyticsManager.Notify` at the point where you want data to be sent. This call can be placed anywhere in the method body, as long as that point **will** be reached during the method's execution. 
- It is **imperative** that the two parameters in the `Notify` method match the method's name, and the containing class name **exactly**. These parameters are used to determine when an event is fired, so any errors might result in the event never activating. This is a known area for improvement. 
- If your class is declared within a **namespace**, then you **must** specify the namespace as a prefix to the class name parameter. In the example below, the "PlayerShooting" class is declared within the "CompleteProject" namespace. In this case, the class parameter is specified as "CompleteProject.PlayerShooting" instead of simply "PlayerShooting". This is a known area for improvement. 
```cs
    namespace CompleteProject
    {
        public class PlayerShooting : MonoBehaviour
        {
            [AnalyticsEvent("Fired when the player shoots.")]
            private void Shoot ()
            {
                AnalyticsManager.Notify("Shoot", "CompleteProject.PlayerShooting");
```


To make the system as flexible as possible, it's best to perform this process for any method which might be executed at key points during a game's execution. However, you can specify as many as you'd like, provided that you specify at least one. 

> NOTE: Failing to specify at least **one** method within your project as an `AnalyticsEvent` will cause errors when attempting to attach an "AnalyticsComponent" to an object. This is a known issue, but can be resolved by removing the AnalyticsComponent from the GameObject. 

> NOTE: Once you specify methods with an `AnalyticsEvent` attribute, you **MUST** open the "Analytics Settings" dialog, described above, and press the "Load Assemblies" button. Then, you must press "Save Settings" and close the dialog. This will re-inspect the methods within the specified assemblies and generate a new listing of ones posessing the `AnalyticsEvent` attribute. This is a known issue and in the future, this behaviour will not be required; the settings will automatically load and re-save when script files are modified. 

- --
##### Component Setup

![AnalyticsComponent Filter](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_component_search.jpg)

Select a GameObject in the Unity editor, then press "Add Component" in the inspector pane, as you would when attaching any other script or Component object. Begin to enter "analytics" in the search filter, and the "Analytics Component" script appears. Select it. 

![AnalyticsComponent](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_component_go.jpg)

> NOTE: If the console displays errors after attaching the AnalyticsComponent, ensure that at least one method in the project's code was defined as a trigger method, as described in the section above. This is a known issue. 

The AnalyticsComponent is now attached to the GameObject. A trigger method can be selected, which displays the descriptive information provided with the attribute parameter (described above). The "Analytics Event" listing is empty, by default. 

Specify an "Event Name" in the field, which will describe the event data sent to the Unity analytics service. 

Next, press the "+" symbol at the bottom-right of the list box. A new list entry will appear. 

![New List Item](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_event_item.jpg)

Specify a name for the list item. This will be treated as a "key" of a key-value pair within a Dictionary. The dropdown box is populated with members belonging to any other Components attached to the GameObject. Each entry in the dropdown is displayed in the format:
> Component: memberName (memberType)

In the picture above, the component is "Transform", the member's name is "position", and it is of `UnityEngine.Vector3` type. 

Depending on your needs, you can add up to 10 list items for every analytics event. You may also transmit an event with only an "event name", and an empty list. This is useful for cases such as completion of a game, where you simply want to know when a player reached a certain point, and do not require any extra information which can be provided by members. 

- --
##### Component Testing

Once you have specified an event and set up the AnalyticsComponent (described above), you can perform local testing to verify your events are gathering the appropriate data. This is a useful first step, since test events won't be transmitted to the Unity service. 

First, open the "Analytics Settings" dialog. Ensure that the "Offline" box, at the top of the dialog, is ticked. 

![Analytics Settings Offline](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_offline.jpg)

When this box is checked, the Modular Analytics system will operate in an offline mode. No events will be transmitted to the Unity analytics service, and all data will be logged to the editor console (or, to the [player log file](http://docs.unity3d.com/Manual/LogFiles.html), if you are running your game as a built executable). 

![Analytics Settings Offline Results](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_resultsoffline.jpg)
![Analytics Component Offline Results](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_resultscomponent.jpg)

You can use offline mode to verify that your event triggers are functioning correctly, and that the data specified in your components is being logged correctly, by cross-referencing your specified member names with the log output. In the example above, the "EnemyDied" event was transmitted when the "Death" method was called within the "EnemyHealth" script - this behaviour was verified by playing the game. The console log indicates that an event with the name "EnemyDied" was transmitted, with two event values specified - "playerPosition" and "playerCurrentHealth". These match the members we specified in the list within the component, so everything looks okay!

- --
##### Online Logging

After you have (optionally) verified your components and data targets by using the offline mode, you are ready to begin capturing data with the Modular Analytics tools. First, you must untick the "Offline" box in the analytics settings dialog, and click "Save Settings" before closing the dialog. 

![Untick Offline](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_offlineun.jpg)

Then, you can verify by testing your game as before. When the event is fired, you will receive log information, denoting the name of the object transmitting data, and a _response code_ sent from the Unity service. These response codes are detailed [here](http://docs.unity3d.com/ScriptReference/Analytics.AnalyticsResult.html). Typically, you should expect a response of "Ok", although you may encounter other responses if you transmit large volumes of data (or somehow transmit malformed data).

To verify that the server received appropriate data, you can visit the analytics dashboard page [here](https://analytics.cloud.unity3d.com/) (you must be logged in to your Unity services account). Select your project, click the "Integration" tab, then choose your engine version from the dropdown list. Then, select the "Advanced Integration" link near the top. 

![Advanced Integration Dashboard Link](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_advlink.jpg)

Scroll to the bottom of the "Advanced Integration" page. A table will be listed, under the heading "Validate". Here, you should see received data under the "Event" column, similar to what is shown below:

![Advanced Integration Data](https://dl.dropboxusercontent.com/u/406357/unityanalytics/fig_advancedintegration.jpg)

If this data appears here, you have successfully integrated the Modular Analytics tools into your project, and can use the analytics component to specify data collection from any object within your game. 
- --
## Known Issues and AFIs (Areas For Improvement)
- Parameters in the `AnalyticsManager.Notify` method must match the class and method names exactly, which can be easily overlooked when refactoring or renaming methods or classes. An automated way of extracting this information is being investigated. 
- If a class is explicitly declared within a namespace, the namespace must be specified in the `AnalyticsManager.Notify` class name parameter, which is not necessarily intuitive or obvious. 
- Unity editor can throw an exception when no `AnalyticsEvents` are specified in code, and a user adds a `AnalyticsComponent` to a GameObject. The exception will be resolved by removing the component, or specifying an AnalyticsEvent, and reloading/saving assemblies in the "Analytics Settings" dialog.
- After code is modified to include `AnalyticsEvent`s, assemblies **must** be reloaded through the "Analytics Settings" dialog.
- Rarely, an exception is thrown when manipulating the "Analytics Settings" dialog. However, in production, this was only encountered when modifying the `AnalyticsSettings.cs` file while the editor was running. 

- --

##### Additional Information

It is expected that these tools will be updated over time to correct bugs or to improve functionality.

If you encounter issues using the tools and require guidance, or if you'd like to report bugs or suggest features, get in touch: 

> s p e n c e @ r a b i d m n k y . c o m

(make sure to remove all of the spaces) 
