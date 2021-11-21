using UnityEngine.UI;

/// <summary>
/// A popup for a hospital
/// </summary>
public class HospitalPopup : EmploymentPopup
{
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text patientsNum;

    /// <summary>
    /// Provides information about the hospital
    /// </summary>
    new void Update()
    {
        employmentUpdate();
        
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        Hospital hospitalClass = objectOfPopup.GetComponent<Hospital>();
        patientsNum.text = "" + hospitalClass.getNumPatients() + "/" + hospitalClass.getNumAvailableBeds();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "This hospital needs workers in order to help cure sick orcs.";
        }
        else if (employment.getWorkerCap() > employment.getNumHealthyWorkers())
        {
            status.text = "This hospital has less available beds due to a lack of healthy employees.";
        }
        else
        {
            status.text = "This hospital is running at peak efficiency.";
        }
    }
}
