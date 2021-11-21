using UnityEngine.UI;

/// <summary>
/// Popup for a market
/// </summary>
public class MarketPopup : EmploymentPopup {
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text storageCapacity;
    public Text meatNum;
    public Text wheatNum;
    public Text eggsNum;
    public Text fishNum;
    public Text furnitureNum;
    public Text weaponsNum;

    /// <summary>
    /// Updates the status and number of goods for the market.
    /// </summary>
    new void Update ()
    {
        employmentUpdate();

        Storage storage = objectOfPopup.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this marketplace cannot distribute goods.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This marketplace is distributing goods slowly due to a lack of workers.";
        }
        else
        {
            status.text = "This marketplace is efficiently distributing goods.";
        }
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        meatNum.text = "" + storage.getMeatCount();
        wheatNum.text = "" + storage.getWheatCount();
        eggsNum.text = "" + storage.getEggCount();
        fishNum.text = "" + storage.getFishCount();
    }
}
