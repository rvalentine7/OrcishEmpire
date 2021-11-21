using UnityEngine.UI;

/// <summary>
/// Popup for a gladiator pit
/// </summary>
public class GladiatorPitPopup : EmploymentPopup {
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text gladiatorsNumText;
    public Text trainingProgressNumText;

    /// <summary>
    /// Updates the gladiator pit popup
    /// </summary>
    new void Update()
    {
        employmentUpdate();

        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this pit cannot train any gladiators.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This pit is training gladiators slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This pit is training gladiators at peak efficiency.";
        }
        GladiatorPit gladiatorPitScript = objectOfPopup.GetComponent<GladiatorPit>();
        gladiatorsNumText.text = "" + gladiatorPitScript.getNumReadyGladiators();
        trainingProgressNumText.text = "" + gladiatorPitScript.getTrainingProgress() + "/100";
    }
}
