using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adjusts the speed of the game
/// </summary>
public class SpeedAdjustment : MonoBehaviour
{
    public Button speedAdjustmentButton;
    public Sprite realTime;
    public Sprite fasterTime;
    public Sprite fastestTime;
    public Sprite pausedTime;
    public GameObject pauseMenu;

    /// <summary>
    /// Updates the speed of the game.  This goes in the order of real time -> 2x -> 3x -> paused -> back to real time
    /// </summary>
    public void updateSpeed()
    {
        if (!pauseMenu.activeInHierarchy)
        {
            if (Time.timeScale == 1.0f)
            {
                Time.timeScale = 1.5f;
                speedAdjustmentButton.image.sprite = fasterTime;
            }
            else if (Time.timeScale == 1.5f)
            {
                Time.timeScale = 2.0f;
                speedAdjustmentButton.image.sprite = fastestTime;
            }
            else if (Time.timeScale == 2.0f)
            {
                Time.timeScale = 0.0f;
                speedAdjustmentButton.image.sprite = pausedTime;
            }
            else
            {
                Time.timeScale = 1.0f;
                speedAdjustmentButton.image.sprite = realTime;
            }
        }
    }

    /// <summary>
    /// Sets the time scale for the game
    /// </summary>
    /// <param name="timeScale">The time scale the game should be using</param>
    public void setTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        if (Time.timeScale == 1.0f)
        {
            speedAdjustmentButton.image.sprite = realTime;
        }
        else if (Time.timeScale == 1.5f)
        {
            speedAdjustmentButton.image.sprite = fasterTime;
        }
        else if (Time.timeScale == 2.0f)
        {
            speedAdjustmentButton.image.sprite = fastestTime;
        }
        else
        {
            speedAdjustmentButton.image.sprite = pausedTime;
        }
    }
}
