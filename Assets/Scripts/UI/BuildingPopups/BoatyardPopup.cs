using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays information about a boatyard
/// </summary>
public class BoatyardPopup : EmploymentPopup
{
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text storageCapacity;
    public Text lumberNum;
    public Text progressNum;

    /// <summary>
    /// Updates the boatyard popup
    /// </summary>
    new void Update()
    {
        employmentUpdate();
        
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        Boatyard thisBoatyard = objectOfPopup.GetComponent<Boatyard>();
        progressNum.text = "" + thisBoatyard.getProgressNum() + "/100";
        Storage storage = objectOfPopup.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
        lumberNum.text = "" + storage.getLumberCount();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this boatyard cannot produce any boats.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This boatyard is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This boatyard is producing boats at peak efficiency.";
        }
    }
}
