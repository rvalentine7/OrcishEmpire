using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup for a trader
/// </summary>
public class TraderPopup : Popup
{
    public Text status;
    private Trade trade;

    /// <summary>
    /// Updates status information about the trader
    /// </summary>
    new void Update()
    {
        base.Update();

        if (trade == null)
        {
            trade = objectOfPopup.GetComponent<Trade>();
        }
        else
        {
            if (trade.leavingTheCity())
            {
                status.text = "Leaving the city.";
            }
            else if (trade.getWaitingOnTradingPost())
            {
                status.text = "Trading with the trading post.";
            }
            else
            {
                status.text = "Going to a trading post to trade goods.";
            }
        }
    }
}
