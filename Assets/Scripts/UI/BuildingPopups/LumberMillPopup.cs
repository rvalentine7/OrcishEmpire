using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup for a lumber mill
/// </summary>
public class LumberMillPopup : EmploymentPopup {
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text progressNum;

    /// <summary>
    /// Updates the lumber mill popup
    /// </summary>
    new void Update()
    {
        employmentUpdate();

        Production lumberMillProduction = objectOfPopup.GetComponent<Production>();
        progressNum.text = "" + lumberMillProduction.getProgressNum() + "/100";
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this mill cannot produce any lumber.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This mill is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This mill is harvesting lumber at peak efficiency.";
        }
    }
}
