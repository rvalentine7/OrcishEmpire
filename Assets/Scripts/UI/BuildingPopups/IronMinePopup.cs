using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup for an iron mine
/// </summary>
public class IronMinePopup : EmploymentPopup {
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text progressNum;
	
	/// <summary>
    /// Updates the iron mine popup
    /// </summary>
	new void Update () {
        employmentUpdate();

        Production thisIronMine = objectOfPopup.GetComponent<Production>();
        progressNum.text = "" + thisIronMine.getProgressNum() + "/100";
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this mine cannot produce any iron.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This mine is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This mine is mining iron at peak efficiency.";
        }
    }
}
