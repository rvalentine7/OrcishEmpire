using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// A popup for a pig farm
/// </summary>
public class PigFarmPopup : EmploymentPopup
{
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text progressNum;

    /// <summary>
    /// Updates the pig farm popup
    /// </summary>
    new void Update()
    {
        employmentUpdate();

        Production thisFarm = objectOfPopup.GetComponent<Production>();
        progressNum.text = "" + thisFarm.getProgressNum() + "/100";
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this pig farm cannot produce any meat.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This pig farm is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This pig farm is producing meat at peak efficiency.";
        }
    }
}
