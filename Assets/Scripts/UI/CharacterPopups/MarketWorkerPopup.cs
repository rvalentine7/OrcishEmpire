using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup for a market worker
/// </summary>
public class MarketWorkerPopup : Popup
{
    public Text status;
    private Distribute distribute;

    /// <summary>
    /// Updates status information about the market worker
    /// </summary>
    new void Update()
    {
        base.Update();

        if (distribute == null)
        {
            distribute = objectOfPopup.GetComponent<Distribute>();
        }
        else
        {
            if (distribute.getHeadingHome())
            {
                status.text = "Returning to the market.";
            }
        }
    }
}
