using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup variables and methods used by all employment buildings
/// </summary>
public abstract class EmploymentPopup : Popup
{
    public Button activateButton;
    public Sprite activateSprite;
    public Sprite deactivateSprite;
    protected Employment employment;

    /// <summary>
    /// Updates general employment variables
    /// </summary>
    protected void employmentUpdate()
    {
        base.Update();

        if (!employment)
        {
            employment = objectOfPopup.GetComponent<Employment>();
        }

        bool activated = employment.getActivated();
        if (!activated)
        {
            activateButton.image.sprite = activateSprite;
        }
        else
        {
            activateButton.image.sprite = deactivateSprite;
        }
    }

    /// <summary>
    /// Toggles the building on/off
    /// </summary>
    public void toggleActivate()
    {
        bool activated = true;
        if (!employment)
        {
            employment = objectOfPopup.GetComponent<Employment>();
            activated = employment.toggleActivated();
        }
        else
        {
            activated = employment.toggleActivated();
        }

        if (!activated)
        {
            activateButton.image.sprite = activateSprite;
        }
        else
        {
            activateButton.image.sprite = deactivateSprite;
        }
    }
}
