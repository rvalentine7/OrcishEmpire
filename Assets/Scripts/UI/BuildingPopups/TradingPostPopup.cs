using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup for a trading post building
/// </summary>
public class TradingPostPopup : EmploymentPopup
{
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    private TradingPost tradingPost;

    /// <summary>
    /// Provides information on the tax collector
    /// </summary>
    new void Update()
    {
        employmentUpdate();

        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Unable to assist traders with no workers.";
        }
        else if (employment.getNumHealthyWorkers() == 0)
        {
            status.text = "Unable to assist traders with no healthy workers.";
        }
        else
        {
            status.text = "Waiting to assist traders.";
        }

        if (tradingPost == null)
        {
            tradingPost = objectOfPopup.GetComponent<TradingPost>();
        }
        else
        {
            if (tradingPost.getCollectingTradeGoods())
            {
                status.text = "Currently assisting a trader.";
            }
        }
    }
}
