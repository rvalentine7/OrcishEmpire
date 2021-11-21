using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup for a mud bath
/// </summary>
public class MudBathPopup : EmploymentPopup
{
    public GameObject imageOnCanvas;
    public Sprite dryImage;
    public Sprite wetImage;
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text timeToRefillNum;
    public Text timeUntilDryNum;

    /// <summary>
    /// Provides updated information on employees and status of the mud bath
    /// </summary>
    new void Update()
    {
        employmentUpdate();

        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();

        //The currentlyWet variable will always match up with whether the bath is dry or wet
        MudBath mudBathClass = objectOfPopup.GetComponent<MudBath>();
        float timeToRefill = mudBathClass.getTimeLeftToRefill();
        timeToRefillNum.text = "" + (timeToRefill < 0 ? "--" : ("" + timeToRefill + "s"));
        float timeUntilDry = mudBathClass.getTimeUntilDry();
        timeUntilDryNum.text = "" + (timeUntilDry == 0 ? "--" : ("" + timeUntilDry + "s"));
        if (employment.getNumWorkers() == 0)
        {
            status.text = "This mud bath needs workers and water access in order to be open for orcs to bathe in.";
        }
        else if (employment.getWorkerCap() > employment.getNumHealthyWorkers())
        {
            status.text = "Due to a lack of employees, this mud bath will dry up on occasion.";
        }
        else
        {
            status.text = "This mud bath is always open for orcs to bathe in.";
        }
    }
}
