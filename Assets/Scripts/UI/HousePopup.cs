using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HousePopup : MonoBehaviour {
    public GameObject house;
    public GameObject houseImage;
    public Text statusText;
    public Text inhabitantCount;
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
        houseImage.GetComponent<Image>().sprite = houseInfo.getHouseSprite();
        int houseLevel = houseInfo.getHouseLevel();
        if (houseInfo.getNumInhabitants() == 0)//sign
        {
            statusText.text = "This location is currently waiting on orcs to move in.";
        }
        else if (houseLevel == 1)//tent
        {
            statusText.text = "With a stable supply of food and water, this house will upgrade.";
        }
        else if (houseLevel == 2)//wood with tent roof
        {
            statusText.text = "With regular access to entertainment, this house will upgrade.";
        }
        else if (houseLevel == 3)//wood
        {
            statusText.text = "This house cannot upgrade further.";
        }
        inhabitantCount.text = "" + houseInfo.getNumInhabitants() + "/" + houseInfo.getHouseSize();
        employedInhabCount.text = "" + houseInfo.getNumEmployedInhabs();
        foodCount.text = "" + storage.getFoodCount();
        waterCount.text = "" + houseInfo.getWaterCount();
        entertainmentLevel.text = "" + houseInfo.getEntertainmentLevel();
        goldCount.text = "" + houseInfo.getHouseholdCurrency();
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
