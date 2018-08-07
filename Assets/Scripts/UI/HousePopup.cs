using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HousePopup : MonoBehaviour {
    public GameObject house;
    public Text inhabitantCount;
    public Text houseSize;
    public Text employedInhabCount;
    public Text foodCount;
    public Text waterCount;
    public Text entertainmentLevel;
    public Text goldCount;

	// Use this for initialization
	void Start () {
		
	}
	
	/**
     * Updates the information displayed on the popup.
     */
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
        HouseInformation houseInfo = house.GetComponent<HouseInformation>();
        Storage storage = house.GetComponent<Storage>();
        inhabitantCount.text = "" + houseInfo.getNumInhabitants();
        houseSize.text = "" + houseInfo.getHouseSize();
        employedInhabCount.text = "" + houseInfo.getNumEmployedInhabs();
        foodCount.text = "" + storage.getFoodCount();
        waterCount.text = "" + storage.getWaterCount();
        entertainmentLevel.text = "" + houseInfo.getEntertainmentLevel();
        //use houseinformation to get the gold the house has
	}

    /**
     * Removes the game object from the game.
     */
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    /**
     * Sets the house object this popup is displaying information on.
     */
    public void setHouse(GameObject house)
    {
        this.house = house;
    }
}
