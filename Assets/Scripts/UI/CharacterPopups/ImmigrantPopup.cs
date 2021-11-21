using UnityEngine.UI;

/// <summary>
/// Popup for an immigrant
/// </summary>
public class ImmigrantPopup : Popup
{
    public Text status;

    /// <summary>
    /// Updates status information on an immigrant
    /// </summary>
    new void Update()
    {
        base.Update();

        Immigrate immigrate = objectOfPopup.GetComponent<Immigrate>();
        if (immigrate.getExitingMap())
        {
            status.text = "Leaving the city because a home is no longer available.";
        }
    }
}
