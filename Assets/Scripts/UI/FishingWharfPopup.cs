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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * Toggles the building on/off
     */
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

    /**
     * Removes the game object from the game.
     */
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    /**
     * Sets the fishing wharf object this popup is displaying information on.
     * @param fishingWharf the fishing wharf the popup is displaying information on
     */
    public void setFishingWharf(GameObject fishingWharf)
    {
        this.fishingWharf = fishingWharf;
    }
}
