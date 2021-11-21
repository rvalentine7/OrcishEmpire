using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A popup for an arena
/// </summary>
public class ArenaPopup : EmploymentPopup
{
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text gladiatorsNum;
    public Text progressNum;
    public Text fightDurationNum;

    /// <summary>
    /// Updates the arena popup
    /// </summary>
    new void Update()
    {
        employmentUpdate();
        
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "There are no fights going on at the arena right now.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "With few employees, fights will rarely occur.";
        }
        else
        {
            status.text = "This arena has fights occuring on a regular basis.";
        }
        Arena arenaScript = objectOfPopup.GetComponent<Arena>();
        gladiatorsNum.text = arenaScript.getNumGladiators() + "/" + arenaScript.getNumGladiatorsRequired();
        string setupProgress = "" + arenaScript.getSetupProgress() + "/100";
        if (arenaScript.getSetupProgress() >= 100
            && arenaScript.getNumGladiators() < arenaScript.getNumGladiatorsRequired())
        {
            setupProgress = "Need gladiators";
            progressNum.resizeTextForBestFit = true;
        }
        else if (arenaScript.getSetupProgress() >= 100
            && arenaScript.getNumGladiators() == arenaScript.getNumGladiatorsRequired())
        {
            setupProgress = "Fight in progress";
            progressNum.resizeTextForBestFit = true;
        }
        else
        {
            progressNum.resizeTextForBestFit = false;
        }
        progressNum.text = setupProgress;
        string fightDurationTime = "Waiting on next fight";
        if (arenaScript.getRemainingFightTime() > 0)
        {
            fightDurationTime = "" + Mathf.RoundToInt(arenaScript.getRemainingFightTime()) + "s";
        }
        fightDurationNum.text = fightDurationTime;
    }
}
