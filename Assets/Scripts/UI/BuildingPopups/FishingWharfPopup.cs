using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup for a fishing wharf
/// </summary>
public class FishingWharfPopup : EmploymentPopup
{
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text progressNum;
    private FishingWharf fishingWharf;

    /// <summary>
    /// Updates the fishing wharf popup
    /// </summary>
    new void Update()
    {
        employmentUpdate();

        FishingWharf thisFishingWharf = objectOfPopup.GetComponent<FishingWharf>();
        progressNum.text = "" + thisFishingWharf.getProgressNum() + "/100";
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this fishing wharf cannot collect fish.";
        }
        else if (!fishingWharf.getHasBoat())
        {
            status.text = "This fishing wharf is waiting to receive a boat in order to fish.";
        }
        else if (fishingWharf.getStandardBoat())
        {
            status.text = "This fishing wharf is waiting on a boat to arrive in order to start fishing.";
        }
        else if (fishingWharf.getFishingBoatWaiting())
        {
            status.text = "The fishing boat is waiting on a delivery orc to take the fish from the boat.";
        }
        else if (fishingWharf.getFishingBoatOutFishing())
        {
            status.text = "The fishing boat is out fishing.";
        }
        else if (fishingWharf.getProgressNum() == 100 && !fishingWharf.getFishingBoatOutFishing())
        {
            status.text = "This fishing wharf is waiting on a delivery orc to deliver fish before the fishing boat" +
                " can go fishing.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This fishing wharf is fishing slowly due to a lack of workers.";
        }
        else
        {
            status.text = "This fishing wharf is fishing at peak efficiency.";
        }
    }

    /// <summary>
    /// Sets the WorldObject this popup is for and also sets the FishingWharf from that object
    /// </summary>
    /// <param name="objectOfPopup">The WorldObject for this popup</param>
    public override void setGameObject(GameObject objectOfPopup)
    {
        this.objectOfPopup = objectOfPopup;
        this.fishingWharf = this.objectOfPopup.GetComponent<FishingWharf>();
    }
}
