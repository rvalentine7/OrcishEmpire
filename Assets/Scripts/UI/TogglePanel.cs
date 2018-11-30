using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Toggles the active state of a game object.
 */
public class TogglePanel : MonoBehaviour {
    public GameObject panelToSetActive;
    //The other panels for making sure they are turned off when the new panel becomes active
    public GameObject offPanelA;//1
    public GameObject offPanelB;//2
    public GameObject offPanelC;//3
    public GameObject offPanelD;//4
    public GameObject offPanelE;//5
    public GameObject offPanelF;//6
    public GameObject offPanelG;//7
    public GameObject offPanelH;//8
    public GameObject offPanelI;//9
    public GameObject offPanelJ;//10

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
        if (offPanelI != null)
        {
            offPanelI.SetActive(false);
        }
        if (offPanelJ != null)
        {
            offPanelJ.SetActive(false);
        }
    }
}
