using UnityEngine.UI;

/// <summary>
/// Popup for an ochre pit
/// </summary>
public class OchrePitPopup : EmploymentPopup {
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text progressNum;

    /// <summary>
    /// Updates the ochre pit popup
    /// </summary>
    new void Update()
    {
        employmentUpdate();

        Production ochrePitProduction = objectOfPopup.GetComponent<Production>();
        progressNum.text = "" + ochrePitProduction.getProgressNum() + "/100";
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this pit cannot mine any ochre.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This pit is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This pit is mining ochre at peak efficiency.";
        }
    }
}
