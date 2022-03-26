using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls in game settings
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    public Slider musicSlider;
    public InputField musicInputField;
    public AudioSource musicAudioSource;
    public Slider clickSlider;
    public InputField clickInputField;
    public AudioSource clickAudioSource;
    public Dropdown graphicsQualityDropdown;
    private int currentGraphicsQuality;

    /// <summary>
    /// Initialization
    /// </summary>
    private void Start()
    {
        currentGraphicsQuality = QualitySettings.GetQualityLevel();
        graphicsQualityDropdown.value = currentGraphicsQuality;
    }

    /// <summary>
    /// Updates music volume based on the slider
    /// </summary>
    public void updateMusicSlider()
    {
        float volume = musicSlider.value;
        musicAudioSource.volume = volume;
        musicInputField.text = Mathf.RoundToInt(volume * 100) + "";
    }

    /// <summary>
    /// Updates music volume based on the input field
    /// </summary>
    public void updateMusicInputField()
    {
        float inputValue = float.Parse(musicInputField.text);
        if (inputValue < 0)
        {
            inputValue = 0;
            musicInputField.text = inputValue + "";
        }
        else if (inputValue > 100)
        {
            inputValue = 100;
            musicInputField.text = inputValue + "";
        }
        inputValue /= 100;
        musicAudioSource.volume = inputValue;
        musicSlider.value = inputValue;
    }

    /// <summary>
    /// Gets the current click volume
    /// </summary>
    /// <returns>The current click volume</returns>
    public float getClickVolume()
    {
        return clickSlider.value;
    }

    /// <summary>
    /// Updates click volume based on the slider
    /// </summary>
    public void updateClickSlider()
    {
        float volume = clickSlider.value;
        clickAudioSource.volume = volume;
        clickInputField.text = Mathf.RoundToInt(volume * 100) + "";
    }

    /// <summary>
    /// Updates click volume based on the input field
    /// </summary>
    public void updateClickInputField()
    {
        float inputValue = float.Parse(clickInputField.text);
        if (inputValue < 0)
        {
            inputValue = 0;
            clickInputField.text = inputValue + "";
        }
        else if (inputValue > 100)
        {
            inputValue = 100;
            clickInputField.text = inputValue + "";
        }
        inputValue /= 100;
        clickAudioSource.volume = inputValue;
        clickSlider.value = inputValue;
    }

    /// <summary>
    /// Updates the graphics quality level
    /// </summary>
    public void updateQualitySettings()
    {
        QualitySettings.SetQualityLevel(currentGraphicsQuality);
    }
}
