using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button to allow users to click on world items
/// </summary>
public class WorldPopupButton : MonoBehaviour
{
    public string worldItemName;
    public GameObject panelToActivate;
    public List<GameObject> panelsToDeactivate;
    public GameObject worldItemIndicator;
    public Sprite worldItemNameTemplate;

    /// <summary>
    /// Toggle the passed-in game object on or off
    /// </summary>
    public void togglePanel()
    {
        //Panel to toggle
        if (panelToActivate != null)
        {
            WorldPopupPanel worldPopupPanel = panelToActivate.GetComponent<WorldPopupPanel>();
            if (!panelToActivate.activeSelf)
            {
                panelToActivate.SetActive(true);
            }
            //We actually want to close the popup if it is active and we already have information on it from this world item
            else if (worldPopupPanel.getWorldItemName().Equals(worldItemName))
            {
                panelToActivate.SetActive(false);
            }
            worldPopupPanel.receiveWorldItemInfo(worldItemName, worldItemNameTemplate, worldItemIndicator);

        }
        //Make sure other panels are off.
        foreach (GameObject panelToDeactivate in panelsToDeactivate)
        {
            if (panelToDeactivate.activeSelf)
            {
                panelToDeactivate.SetActive(false);
            }
        }
    }
}
