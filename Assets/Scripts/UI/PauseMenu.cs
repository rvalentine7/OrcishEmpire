using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Menu providing users with options for settings and quitting the game
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public SpeedAdjustment playSpeed;
    private float currentTimeScale = 1.0f;

    /// <summary>
    /// Pause the game on enable
    /// </summary>
    private void OnEnable()
    {
        currentTimeScale = Time.timeScale;
        playSpeed.setTimeScale(0.0f);
    }

    /// <summary>
    /// Return to the previous speed setting on disable
    /// </summary>
    private void OnDisable()
    {
        playSpeed.setTimeScale(currentTimeScale);
    }

    /// <summary>
    /// Close the pause menu
    /// </summary>
    public void closePauseMenu()
    {
        gameObject.SetActive(false);
    }
}
