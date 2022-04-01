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

    /// <summary>
    /// Provides information on the tax collector
    /// </summary>
    new void Update()
    {
        employmentUpdate();

        employeeNum.text = "" + employment.getNumWorkers();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "The trading post is unable to assist traders with no workers";
        }
        else
        {
            status.text = "The trading post is assisting traders that visit the city";
        }
    }
}
