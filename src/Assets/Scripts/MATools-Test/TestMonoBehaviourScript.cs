using UnityEngine;

/// <summary>
/// Test script, providing an AnalyticsEvent method, and an Update call which fires the AnalyticsEvent method
/// </summary>
public class TestMonoBehaviourScript : MonoBehaviour
{
    /// <summary>
    /// flag to stop update behvaiour
    /// </summary>
    private bool run = false;

    /// <summary>
    /// Test method providing a method call for sending analytics data
    /// </summary>
    [AnalyticsEvent("An event which is fired immediately after the game begins")]
    public void OnTestGameEvent()
    {
        //// call to either AnalyticsManager.Notify or Analytics.CustomEvent
        //// (or, the corresponding wrappers in TimingFunctions.cs)
        AnalyticsManager.Notify("OnTestGameEvent", "TestMonoBehaviourScript");
    }

    /// <summary>
    /// game update behaviour
    /// </summary>
    private void Update()
    {
        if (!run) // only run the test event once
        {
            OnTestGameEvent();
            run = !run;
        }
    }
}