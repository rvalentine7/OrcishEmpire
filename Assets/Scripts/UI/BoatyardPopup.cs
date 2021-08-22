using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Displays information about a boatyard
/// </summary>
public class BoatyardPopup : MonoBehaviour
{
    public GameObject boatyard;
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text storageCapacity;
    public Text lumberNum;
    public Text progressNum;
    public Button activateButton;
    public Sprite activateSprite;
    public Sprite deactivateSprite;
    private bool initialClick;

    // Start is called before the first frame update
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
        Employment employment = boatyard.GetComponent<Employment>();
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
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        Boatyard thisBoatyard = boatyard.GetComponent<Boatyard>();
        progressNum.text = "" + thisBoatyard.getProgressNum() + "/100";
        Storage storage = boatyard.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
        lumberNum.text = "" + storage.getLumberCount();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this boatyard cannot produce any boats.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This boatyard is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This boatyard is producing boats at peak efficiency.";
        }
    }

    /// <summary>
    /// Toggles the building on/off
    /// </summary>
    public void toggleActivate()
    {
        bool activated = boatyard.GetComponent<Employment>().toggleActivated();
        if (!activated)
        {
            activateButton.image.sprite = activateSprite;
        }
        else
        {
            activateButton.image.sprite = deactivateSprite;
        }
    }

    /// <summary>
    /// Removes the game object from the game.
    /// </summary>
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Sets the boatyard object this popup is displaying information on.
    /// </summary>
    /// <param name="boatyard">the boatyard the popup is displaying information on</param>
    public void setBoatyard(GameObject boatyard)
    {
        this.boatyard = boatyard;
    }
}
