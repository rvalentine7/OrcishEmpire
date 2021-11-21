using UnityEngine.UI;

/// <summary>
/// Popup for a warehouse
/// </summary>
public class WarehousePopup : EmploymentPopup {
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
    public Text ironNum;
    public Text lumberNum;
    public Text hopsNum;
    public Text beerNum;
    public Text ochreNum;
    public Text warPaintNum;
    public Text treasureNum;

    /// <summary>
    /// Updates the status and number of goods for the warehouse.
    /// </summary>
    new void Update () {
        employmentUpdate();

        Storage storage = objectOfPopup.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this warehouse cannot distribute goods.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This warehouse is distributing goods slowly due to a lack of workers.";
        }
        else
        {
            status.text = "This warehouse is efficiently distributing goods.";
        }
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        meatNum.text = "" + storage.getMeatCount();
        wheatNum.text = "" + storage.getWheatCount();
        eggsNum.text = "" + storage.getEggCount();
        fishNum.text = "" + storage.getFishCount();
        ironNum.text = "" + storage.getIronCount();
        lumberNum.text = "" + storage.getLumberCount();
        weaponsNum.text = "" + storage.getWeaponCount();
        furnitureNum.text = "" + storage.getFurnitureCount();
        hopsNum.text = "" + storage.getHopsCount();
        beerNum.text = "" + storage.getBeerCount();
        ochreNum.text = "" + storage.getOchreCount();
        warPaintNum.text = "" + storage.getWarPaintCount();
        treasureNum.text = "0";
    }
}
