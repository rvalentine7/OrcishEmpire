using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup for a hops farm
/// </summary>
public class HopsFarmPopup : EmploymentPopup
{
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text progressNum;

    /// <summary>
    /// Updates the hops farm popup
    /// </summary>
    new void Update()
    {
        employmentUpdate();

        Production thisFarm = objectOfPopup.GetComponent<Production>();
        progressNum.text = "" + thisFarm.getProgressNum() + "/100";
        Employment employment = objectOfPopup.GetComponent<Employment>();
        bool activated = employment.getActivated();
        if (!activated)
        {
            activateButton.image.sprite = activateSprite;
        }
        else
        {
            activateButton.image.sprite = deactivateSprite;
        }
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this farm cannot produce any hops.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This hops farm is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This hops farm is producing hops at peak efficiency.";
        }
    }
}
