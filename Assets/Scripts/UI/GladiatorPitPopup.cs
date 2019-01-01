using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GladiatorPitPopup : MonoBehaviour {
    public GameObject gladiatorPit;
    public Text status;
    public Text employeeNum;
    public Text gladiatorsNumText;
    public Text trainingProgressNumText;
    public Button activateButton;
    public Sprite activateSprite;
    public Sprite deactivateSprite;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
        Employment employment = gladiatorPit.GetComponent<Employment>();
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
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this pit cannot train any gladiators.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This pit is training gladiators slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This pit is training gladiators at peak efficiency.";
        }
        GladiatorPit gladiatorPitScript = gladiatorPit.GetComponent<GladiatorPit>();
        gladiatorsNumText.text = "" + gladiatorPitScript.getNumReadyGladiators();
        trainingProgressNumText.text = "" + gladiatorPitScript.getTrainingProgress() + "/100";
    }

    /**
     * Toggles the building on/off
     */
    public void toggleActivate()
    {
        bool activated = gladiatorPit.GetComponent<Employment>().toggleActivated();
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
     * Sets the farm object this popup is displaying information on.
     */
    public void setGladiatorPit(GameObject gladiatorPit)
    {
        this.gladiatorPit = gladiatorPit;
    }
}
