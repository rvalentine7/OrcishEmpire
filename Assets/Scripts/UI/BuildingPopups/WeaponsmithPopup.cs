using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup for a weaponsmith
/// </summary>
public class WeaponsmithPopup : EmploymentPopup {
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text storageCapacity;
    public Text ironNum;
    public Text progressNum;

    /// <summary>
    /// Updates the weaponsmith popup
    /// </summary>
    new void Update()
    {
        employmentUpdate();

        ItemProduction thisWeaponsmith = objectOfPopup.GetComponent<ItemProduction>();
        progressNum.text = "" + thisWeaponsmith.getProgressNum() + "/100";
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        Storage storage = objectOfPopup.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
        ironNum.text = "" + storage.getIronCount();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this weaponsmith cannot produce any weapons.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This smithy is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This smithy is producing weapons at peak efficiency.";
        }
    }
}
