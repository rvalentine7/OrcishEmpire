using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup for a fishing boat
/// </summary>
public class FishingBoatPopup : Popup
{
    public Text status;
    private FishingBoat fishingBoat;

    /// <summary>
    /// Updates status information about the fishing boat
    /// </summary>
    new void Update()
    {
        base.Update();

        if (fishingBoat == null)
        {
            fishingBoat = objectOfPopup.GetComponent<FishingBoat>();
        }
        else
        {
            if (fishingBoat.getFinishedFishing())
            {
                status.text = "Returning to the fishing wharf.";
            }
            else if (fishingBoat.getStartedFishing())
            {
                status.text = "Fishing.";
            }
        }
    }
}
