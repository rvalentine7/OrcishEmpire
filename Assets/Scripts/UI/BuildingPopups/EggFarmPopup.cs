using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup for an egg farm
/// </summary>
public class EggFarmPopup : EmploymentPopup
{
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text progressNum;

    /// <summary>
    /// Updates the egg farm popup
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
            status.text = "Without any employees, this egg farm cannot produce any eggs.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This egg farm is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This egg farm is producing eggs at peak efficiency.";
        }
    }
}
