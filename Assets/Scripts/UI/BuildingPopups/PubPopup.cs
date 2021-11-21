using UnityEngine.UI;

/// <summary>
/// Popup for a pub
/// </summary>
public class PubPopup : EmploymentPopup {
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text storageCapacity;
    public Text beerNum;
    public Text timeLeftNum;
	
	/// <summary>
    /// Provides information on things needed to provide entertainment to the area surrounding the pub
    /// </summary>
	new void Update () {
        employmentUpdate();

        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        Storage storage = objectOfPopup.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "There is no drinking going on at the pub right now.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "With few employees, entertainment will not be provided as often.";
        }
        else if (storage.getBeerCount() > 0)
        {
            status.text = "This pub is providing entertainment to nearby houses.";
        }
        else
        {
            status.text = "This pub is waiting on a supply of beer to provide entertainment.";
        }
        beerNum.text = "" + storage.getBeerCount();
        Pub pubScript = objectOfPopup.GetComponent<Pub>();
        int timeLeft = pubScript.getTimeLeftOnCurrentDrinks();
        if (timeLeft <= 0)
        {
            timeLeftNum.text = "0s";
        }
        else
        {
            timeLeftNum.text = "" + timeLeft + "s";
        }
    }
}
