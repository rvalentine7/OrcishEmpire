using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FishingWharfPopup : MonoBehaviour
{
    public GameObject fishingWharf;
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text storageCapacity;
    public Text fishNum;
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
        FishingWharf thisFishingWharf = fishingWharf.GetComponent<FishingWharf>();
        progressNum.text = "" + thisFishingWharf.getProgressNum() + "/100";
        Employment employment = fishingWharf.GetComponent<Employment>();
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
        //TODO: if waiting on a standard boat, if it's trying to find a fishing spot
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this fishing wharf cannot collect fish.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This fishing wharf is collecting slowly due to a lack of workers.";
        }
        else
        {
            status.text = "This fishing wharf is collecting fish at peak efficiency.";
        }
    }

    /// <summary>
    /// Toggles the building on/off
    /// </summary>
    public void toggleActivate()
    {
        bool activated = fishingWharf.GetComponent<Employment>().toggleActivated();
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
    /// <param name="fishingWharf">the fishing wharf the popup is displaying information on</param>
    public void setFishingWharf(GameObject fishingWharf)
    {
        this.fishingWharf = fishingWharf;
    }
}
