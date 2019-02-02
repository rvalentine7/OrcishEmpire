using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WarPaintWorkshopPopup : MonoBehaviour {
    public GameObject warPaintWorkshop;
    public Text status;
    public Text employeeNum;
    public Text storageCapacity;
    public Text ochreNum;
    public Text progressNum;
    public Button activateButton;
    public Sprite activateSprite;
    public Sprite deactivateSprite;
    private bool initialClick;

    // Use this for initialization
    void Start()
    {
        GameObject panel = GameObject.FindWithTag(World.BUILD_PANEL);
        if (panel != null)
        {
            panel.SetActive(false);
        }
        initialClick = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (initialClick && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)))
        {
            initialClick = false;
        }
        if (Input.GetKey(KeyCode.Escape) || (!initialClick
            && !EventSystem.current.IsPointerOverGameObject()
            && (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))))
        {
            Destroy(gameObject);
        }
        ItemProduction warPaintProduction = warPaintWorkshop.GetComponent<ItemProduction>();
        progressNum.text = "" + warPaintProduction.getProgressNum() + "/100";
        Employment employment = warPaintWorkshop.GetComponent<Employment>();
        bool activated = employment.getActivated();
        if (!activated)
        {
            activateButton.image.sprite = activateSprite;
        }
        else
        {
            activateButton.image.sprite = deactivateSprite;
        }
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        Storage storage = warPaintWorkshop.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
        ochreNum.text = "" + storage.getOchreCount();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this workshop cannot produce any war paint.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This workshop is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This workshop is producing war paint at peak efficiency.";
        }
    }

    /**
     * Toggles the building on/off
     */
    public void toggleActivate()
    {
        bool activated = warPaintWorkshop.GetComponent<Employment>().toggleActivated();
        if (!activated)
        {
            activateButton.image.sprite = activateSprite;
        }
        else
        {
            activateButton.image.sprite = deactivateSprite;
        }
    }

    /**
     * Removes the game object from the game.
     */
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    /**
     * Sets the war paint workshop object this popup is displaying information on.
     */
    public void setWarpaintWorkshop(GameObject warPaintWorkshop)
    {
        this.warPaintWorkshop = warPaintWorkshop;
    }
}
