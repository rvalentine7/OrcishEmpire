using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FishingWharfPopup : MonoBehaviour
{
    public GameObject fishingWharfGameObject;
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text progressNum;
    public Button activateButton;
    public Sprite activateSprite;
    public Sprite deactivateSprite;
    private bool initialClick;
    private FishingWharf fishingWharf;

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
        FishingWharf thisFishingWharf = fishingWharfGameObject.GetComponent<FishingWharf>();
        progressNum.text = "" + thisFishingWharf.getProgressNum() + "/100";
        Employment employment = fishingWharfGameObject.GetComponent<Employment>();
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
            status.text = "Without any employees, this fishing wharf cannot collect fish.";
        }
        else if (!fishingWharf.getHasBoat())
        {
            status.text = "This fishing wharf is waiting to receive a boat in order to fish.";
        }
        else if (fishingWharf.getStandardBoat())
        {
            status.text = "This fishing wharf is waiting on a boat to arrive in order to start fishing";
        }
        else if (fishingWharf.getFishingBoatWaiting())
        {
            status.text = "The fishing boat is waiting on a delivery orc to take the fish from the boat";
        }
        else if (fishingWharf.getFishingBoatOutFishing())
        {
            status.text = "The fishing boat is out fishing";
        }
        else if (fishingWharf.getProgressNum() == 100 && !fishingWharf.getFishingBoatOutFishing())
        {
            status.text = "This fishing wharf is waiting on a delivery orc to deliver fish before the fishing boat" +
                " can go fishing.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This fishing wharf is fishing slowly due to a lack of workers.";
        }
        else
        {
            status.text = "This fishing wharf is fishing at peak efficiency.";
        }
    }

    /// <summary>
    /// Toggles the building on/off
    /// </summary>
    public void toggleActivate()
    {
        bool activated = fishingWharfGameObject.GetComponent<Employment>().toggleActivated();
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
    /// Sets the fishing wharf object this popup is displaying information on.
    /// </summary>
    /// <param name="fishingWharfGameObject">the fishing wharf the popup is displaying information on</param>
    public void setFishingWharf(GameObject fishingWharfGameObject)
    {
        this.fishingWharfGameObject = fishingWharfGameObject;
        this.fishingWharf = fishingWharfGameObject.GetComponent<FishingWharf>();
    }
}
