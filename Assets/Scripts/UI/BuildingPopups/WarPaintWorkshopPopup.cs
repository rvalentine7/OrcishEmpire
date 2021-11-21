using UnityEngine.UI;

/// <summary>
/// Popup for a war paint workshop
/// </summary>
public class WarPaintWorkshopPopup : EmploymentPopup {
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text storageCapacity;
    public Text ochreNum;
    public Text progressNum;

    /// <summary>
    /// Updates the war paint workshop
    /// </summary>
    new void Update()
    {
        employmentUpdate();

        ItemProduction warPaintProduction = objectOfPopup.GetComponent<ItemProduction>();
        progressNum.text = "" + warPaintProduction.getProgressNum() + "/100";
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        Storage storage = objectOfPopup.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
        ochreNum.text = "" + storage.getOchreCount();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this workshop cannot produce any war paint.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This workshop is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This workshop is producing war paint at peak efficiency.";
        }
    }
}
