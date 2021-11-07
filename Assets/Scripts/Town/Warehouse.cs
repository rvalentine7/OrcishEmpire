using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Stores goods
/// </summary>
public class Warehouse : Storage {
    public GameObject warehousePopupObject;

    public Sprite emptyWarehouse;
    public Sprite firstRowWarehouse;
    public Sprite secondRowWarehouse;
    public Sprite thirdRowWarehouse;
    public Sprite fourthRowWarehouse;
    public Sprite fifthRowWarehouse;
    public Sprite sixthRowWarehouse;
    public Sprite seventhRowWarehouse;
    public Sprite eighthRowWarehouse;

    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Initialization
    /// </summary>
    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Adds a resource to the warehouse
    /// </summary>
    /// <param name="name">The name of the resource to add</param>
    /// <param name="num">The amount of resource to add</param>
    public override void addResource(string name, int num)
    {
        base.addResource(name, num);
        updateSprite();
    }

    /// <summary>
    /// Removes a resource from the warehouse
    /// </summary>
    /// <param name="name">The name of the resource to remove</param>
    /// <param name="num">The amount of resource to remove</param>
    public override void removeResource(string name, int num)
    {
        base.removeResource(name, num);
        updateSprite();
    }

    /// <summary>
    /// Updates the warehouse sprite based on the amount of resources remaining
    /// </summary>
    private void updateSprite()
    {
        double percentFull = 100.0 * ((double)base.getCurrentAmountStored() / (double)base.getStorageMax());
        if (percentFull == 0)
        {
            spriteRenderer.sprite = emptyWarehouse;
        }
        else if (percentFull <= 15)
        {
            spriteRenderer.sprite = firstRowWarehouse;
        }
        else if (percentFull <= 30)
        {
            spriteRenderer.sprite = secondRowWarehouse;
        }
        else if (percentFull <= 45)
        {
            spriteRenderer.sprite = thirdRowWarehouse;
        }
        else if (percentFull <= 60)
        {
            spriteRenderer.sprite = fourthRowWarehouse;
        }
        else if (percentFull <= 75)
        {
            spriteRenderer.sprite = fifthRowWarehouse;
        }
        else if (percentFull <= 80)
        {
            spriteRenderer.sprite = sixthRowWarehouse;
        }
        else if (percentFull <= 95)
        {
            spriteRenderer.sprite = seventhRowWarehouse;
        }
        else if (percentFull <= 100)
        {
            spriteRenderer.sprite = eighthRowWarehouse;
        }
    }

    /// <summary>
    /// Returns information on what is stored at the warehouse.
    /// </summary>
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameObject.FindWithTag(World.BUILD_OBJECT) == null)
        {
            GameObject popupObject = GameObject.FindWithTag(World.POPUP);
            if (popupObject != null)
            {
                Destroy(popupObject);
            }
            GameObject popup = Instantiate(warehousePopupObject) as GameObject;
            WarehousePopup warehousePopup = popup.GetComponent<WarehousePopup>();
            warehousePopup.setWarehouse(gameObject);
        }
    }
}
