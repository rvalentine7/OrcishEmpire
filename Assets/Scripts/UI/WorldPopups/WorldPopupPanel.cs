using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A panel for any popup that is created from the world map
/// </summary>
public class WorldPopupPanel : MonoBehaviour
{
    public Image worldItemNameTemplate;
    protected string worldItemName;
    protected GameObject worldItemIndicator;

    /// <summary>
    /// Receives information on the item on the world map
    /// </summary>
    /// <param name="worldItemName">The name of the item on the world map</param>
    /// <param name="worldItemNameTemplate">The sprite containing the name of the world item</param>
    /// <param name="worldItemIndicator">Used to indicate which item on the world map is displaying information</param>
    public virtual void receiveWorldItemInfo(string worldItemName, Sprite worldItemNameTemplate, GameObject worldItemIndicator)
    {
        toggleWorldItemGUI(worldItemIndicator);

        this.worldItemName = worldItemName;
        this.worldItemNameTemplate.sprite = worldItemNameTemplate;
    }

    /// <summary>
    /// Gets the name of the world item
    /// </summary>
    /// <returns>The name of the world item</returns>
    public string getWorldItemName()
    {
        return worldItemName;
    }

    /// <summary>
    /// Toggles GUI items specific to the world item
    /// </summary>
    /// <param name="worldItemIndicator">Used to indicate which item on the world map is displaying information</param>
    protected void toggleWorldItemGUI(GameObject worldItemIndicator)
    {
        if (gameObject.activeSelf)
        {
            if (this.worldItemIndicator)
            {
                this.worldItemIndicator.SetActive(false);
                
                this.worldItemIndicator = worldItemIndicator;
                this.worldItemIndicator.SetActive(true);
            }
            else
            {
                worldItemIndicator.SetActive(true);
                this.worldItemIndicator = worldItemIndicator;
            }
        }
    }

    /// <summary>
    /// Disables GUI elements
    /// </summary>
    protected virtual void OnDisable()
    {
        if (this.worldItemIndicator && this.worldItemIndicator.activeSelf)
        {
            this.worldItemIndicator.SetActive(false);
            this.worldItemIndicator = null;
        }
    }
}
