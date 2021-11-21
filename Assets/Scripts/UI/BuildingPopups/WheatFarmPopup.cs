using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A popup for a wheat farm
/// </summary>
public class WheatFarmPopup : EmploymentPopup
{
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text progressNum;

    /// <summary>
    /// Updates the wheat farm popup
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
            status.text = "Without any employees, this wheat farm cannot produce any wheat.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This wheat farm is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This wheat farm is producing wheat at peak efficiency.";
        }
    }
}
