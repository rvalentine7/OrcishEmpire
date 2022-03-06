using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public SpeedAdjustment playSpeed;
    private float currentTimeScale = 1.0f;

    private void OnEnable()
    {
        currentTimeScale = Time.timeScale;
        playSpeed.setTimeScale(0.0f);
    }

    private void OnDisable()
    {
        playSpeed.setTimeScale(currentTimeScale);
    }

    public void closePauseMenu()
    {
        gameObject.SetActive(false);
    }
}
