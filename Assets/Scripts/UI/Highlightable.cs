using UnityEngine;

/// <summary>
/// Performs actions from highlighting an object in the game
/// </summary>
public class Highlightable : MonoBehaviour
{
    public GameObject tooltip;

    private float delayTooltipTime;
    private float setActiveTime;
    private bool displayTooltip;

    /// <summary>
    /// Initialization
    /// </summary>
    private void Start()
    {
        delayTooltipTime = 0.75f;
        setActiveTime = 0.0f;
        displayTooltip = false;
    }

    /// <summary>
    /// Handles actions that involve time
    /// </summary>
    private void Update()
    {
        if (Time.time > setActiveTime && displayTooltip)
        {
            tooltip.SetActive(true);
            displayTooltip = false;
        }
    }

    /// <summary>
    /// Sets variables to display a tooltip
    /// </summary>
    public void showToolTip()
    {
        setActiveTime = Time.time + delayTooltipTime;
        displayTooltip = true;
    }

    /// <summary>
    /// Hides a tooltip or prevents it from being displayed
    /// </summary>
    public void hideTooltip()
    {
        displayTooltip = false;
        tooltip.SetActive(false);
    }
}
