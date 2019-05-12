using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BarberPopup : MonoBehaviour
{
    public GameObject barber;
    public Button activateButton;
    public Sprite activateSprite;
    public Sprite deactivateSprite;
    private bool initialClick;
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text customersNum;

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
        Employment employment = barber.GetComponent<Employment>();
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
        if (employment.getNumWorkers() == 0)
        {
            status.text = "";
        }
        else if (employment.getWorkerCap() > employment.getNumHealthyWorkers())
        {
            status.text = "";
        }
        else
        {
            status.text = "";
        }

    }

    /**
     * Toggles the building on/off
     */
    public void toggleActivate()
    {
        bool activated = barber.GetComponent<Employment>().toggleActivated();
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
     * Sets the barber object this popup is displaying information on.
     */
    public void setBarber(GameObject barber)
    {
        this.barber = barber;
    }
}
