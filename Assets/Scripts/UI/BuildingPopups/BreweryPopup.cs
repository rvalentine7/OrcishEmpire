using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// A popup for a brewery
/// </summary>
public class BreweryPopup : EmploymentPopup {
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text storageCapacity;
    public Text hopsNum;
    public Text progressNum;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    new void Update () {
        employmentUpdate();

        ItemProduction thisBrewery = objectOfPopup.GetComponent<ItemProduction>();
        progressNum.text = "" + thisBrewery.getProgressNum() + "/100";
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        Storage storage = objectOfPopup.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
        hopsNum.text = "" + storage.getHopsCount();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this brewery cannot produce any beer.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This brewery is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This brewery is producing beer at peak efficiency.";
        }
    }
}
