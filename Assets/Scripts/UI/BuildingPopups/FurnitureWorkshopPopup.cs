using UnityEngine.UI;

/// <summary>
/// A popup for a furniture workshop
/// </summary>
public class FurnitureWorkshopPopup : EmploymentPopup {
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text storageCapacity;
    public Text lumberNum;
    public Text progressNum;

    /// <summary>
    /// Provides information of the working status of the workshop
    /// </summary>
    new void Update()
    {
        employmentUpdate();

        ItemProduction thisFurnitureWorkshop = objectOfPopup.GetComponent<ItemProduction>();
        progressNum.text = "" + thisFurnitureWorkshop.getProgressNum() + "/100";
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        Storage storage = objectOfPopup.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
        lumberNum.text = "" + storage.getLumberCount();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this workshop cannot produce any furniture.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This workshop is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This workshop is producing furniture at peak efficiency.";
        }
    }
}
