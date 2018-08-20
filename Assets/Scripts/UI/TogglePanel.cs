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
    public GameObject offPanelC;
    public GameObject offPanelD;
    public GameObject offPanelE;
    public GameObject offPanelF;
    public GameObject offPanelG;
    public GameObject offPanelH;

    /**
     * Toggle the passed-in game object on or off
     */
    public void togglePanel()
    {
        //Panel to toggle
        if (panelToSetActive != null && panelToSetActive.activeSelf == false)
        {
            panelToSetActive.SetActive(true);
        }
        else if (panelToSetActive != null)
        {
            panelToSetActive.SetActive(false);
        }
        //Make sure other panels are off.  TODO: Could add the panels to a list instead and then loop through
        if (offPanelA != null)
        {
            offPanelA.SetActive(false);
        }
        if (offPanelB != null)
        {
            offPanelB.SetActive(false);
        }
        if (offPanelC != null)
        {
            offPanelC.SetActive(false);
        }
        if (offPanelD != null)
        {
            offPanelD.SetActive(false);
        }
        if (offPanelE != null)
        {
            offPanelE.SetActive(false);
        }
        if (offPanelF != null)
        {
            offPanelF.SetActive(false);
        }
        if (offPanelG != null)
        {
            offPanelG.SetActive(false);
        }
        if (offPanelH != null)
        {
            offPanelH.SetActive(false);
        }
    }
}
