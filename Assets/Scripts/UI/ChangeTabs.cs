using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTabs : MonoBehaviour
{
    public GameObject panelToActivate;
    public GameObject panelToDeactivate;
    public GameObject newLargeButton;
    public GameObject newSmallButton;
    public GameObject oldLargeButton;
    public GameObject oldSmallButton;

    /**
     * Change the buttons and panels being displayed on the popup
     */
    public void changePanels()
    {
        panelToActivate.SetActive(true);
        panelToDeactivate.SetActive(false);
        newLargeButton.SetActive(true);
        newSmallButton.SetActive(true);
        oldLargeButton.SetActive(false);
        oldSmallButton.SetActive(false);
    }
}
