using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Toggles the active state of a game object.
 */
public class TogglePanel : MonoBehaviour {
    public GameObject panelToSetActive;
    //The other panels for making sure they are turned off when the new panel becomes active
    public GameObject offPanelA;
    public GameObject offPanelB;

    /**
     * Toggle the passed-in game object on or off
     */
    public void togglePanel()
    {
        //Panel to toggle
        if (panelToSetActive.activeSelf == false)
        {
            panelToSetActive.SetActive(true);
        }
        else
        {
            panelToSetActive.SetActive(false);
        }
        //Make sure other panels are off
        offPanelA.SetActive(false);
        offPanelB.SetActive(false);
    }
}
