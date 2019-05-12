using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MudBathPopup : MonoBehaviour
{
    public GameObject mudBath;
    public GameObject imageOnCanvas;
    public Sprite dryImage;
    public Sprite wetImage;
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text timeToRefillNum;
    public Text timeUntilDryNum;
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
        Employment employment = mudBath.GetComponent<Employment>();
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

        //The currentlyWet variable will always match up with whether the bath is dry or wet
        MudBath mudBathClass = mudBath.GetComponent<MudBath>();
        float timeToRefill = mudBathClass.getTimeLeftToRefill();
        timeToRefillNum.text = "" + (timeToRefill < 0 ? "--" : ("" + timeToRefill + "s"));
        float timeUntilDry = mudBathClass.getTimeUntilDry();
        timeUntilDryNum.text = "" + (timeUntilDry == 0 ? "--" : ("" + timeUntilDry + "s"));
        if (employment.getNumWorkers() == 0)
        {
            status.text = "This mud bath needs workers and water access in order to be open for orcs to bathe in.";
        }
        else if (employment.getWorkerCap() > employment.getNumHealthyWorkers())
        {
            status.text = "Due to a lack of employees, this mud bath will dry up on occasion.";
        }
        else
        {
            status.text = "This mud bath is always open for orcs to bathe in.";
        }
    }

    /**
     * Toggles the building on/off
     */
    public void toggleActivate()
    {
        bool activated = mudBath.GetComponent<Employment>().toggleActivated();
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
     * Sets the mud bath object this popup is displaying information on.
     */
    public void setMudBath(GameObject mudBath)
    {
        this.mudBath = mudBath;
    }
}
