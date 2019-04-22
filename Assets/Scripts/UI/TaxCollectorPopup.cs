using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TaxCollectorPopup : MonoBehaviour {
    public GameObject taxCollector;
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text taxesCollectedNum;
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

        Employment employment = taxCollector.GetComponent<Employment>();
        bool activated = employment.getActivated();
        if (!activated)
        {
            activateButton.image.sprite = activateSprite;
        }
        else
        {
            activateButton.image.sprite = deactivateSprite;
        }
        employeeNum.text = "" + employment.getNumWorkers();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        TaxCollector taxCollectorScript = taxCollector.GetComponent<TaxCollector>();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "This building is unable to collect taxes with no workers";
        }
        else if (taxCollectorScript.getCollectorStatus() == true)
        {
            status.text = "An orc is currently collecting taxes from nearby houses";
        }
        else
        {
            status.text = "An orc is getting ready to collect taxes from nearby houses";
        }
        taxesCollectedNum.text = "" + taxCollectorScript.getTaxesCollected();
    }

    /**
     * Toggles the building on/off
     */
    public void toggleActivate()
    {
        bool activated = taxCollector.GetComponent<Employment>().toggleActivated();
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
     * Sets the reservoir object this popup is displaying information on.
     * @param reservoir the reservoir the popup is displaying information on
     */
    public void setTaxCollector(GameObject taxCollector)
    {
        this.taxCollector = taxCollector;
    }
}
