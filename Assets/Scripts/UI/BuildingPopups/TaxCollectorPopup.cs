using UnityEngine.UI;

/// <summary>
/// Popup for a tax collector building
/// </summary>
public class TaxCollectorPopup : EmploymentPopup {
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text taxesCollectedNum;

    /// <summary>
    /// Provides information on the tax collector
    /// </summary>
    new void Update()
    {
        employmentUpdate();

        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        TaxCollector taxCollectorScript = objectOfPopup.GetComponent<TaxCollector>();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "This building is unable to collect taxes with no workers";
        }
        else if (taxCollectorScript.getCollectorStatus() == true)
        {
            status.text = "An orc is currently collecting taxes from nearby houses";
        }
        else
        {
            status.text = "An orc is getting ready to collect taxes from nearby houses";
        }
        taxesCollectedNum.text = "" + taxCollectorScript.getTaxesCollected();
    }
}
