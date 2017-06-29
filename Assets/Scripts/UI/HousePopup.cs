using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HousePopup : MonoBehaviour {
    public GameObject house;
    public Text inhabitantCount;
    public Text houseSize;
    public Text employedInhabCount;
    public Text foodCount;
    public Text waterCount;

	// Use this for initialization
	void Start () {
		
	}
	
	/**
     * Updates the information displayed on the popup.
     */
	void Update () {
        HouseInformation houseInfo = house.GetComponent<HouseInformation>();
        inhabitantCount.text = "" + houseInfo.getNumInhabitants();
        houseSize.text = "" + houseInfo.getHouseSize();
        employedInhabCount.text = "" + houseInfo.getNumEmployedInhabs();
        foodCount.text = "" + houseInfo.getFoodCount();
        waterCount.text = "" + houseInfo.getWaterCount();
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
