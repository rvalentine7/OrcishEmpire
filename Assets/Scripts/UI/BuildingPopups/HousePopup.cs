using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup for a house
/// </summary>
public class HousePopup : Popup
{
    public GameObject houseImage;
    public Text statusText;
    public Text inhabitantCount;
    public Text sickInhabitantCount;
    public Text atHospitalCount;
    public Text employedInhabCount;
    public Text storageCapacity;
    public Text foodCount;
    public Text waterCount;
    public Text entertainmentLevel;
    public Text goldCount;

    /// <summary>
    /// Updates the information displayed on the popup.
    /// </summary>
    new void Update () {
        base.Update();

        HouseInformation houseInfo = objectOfPopup.GetComponent<HouseInformation>();
        Storage storage = objectOfPopup.GetComponent<Storage>();
        storageCapacity.text = "" + storage.getCurrentAmountStored() + "/" + storage.getStorageMax();
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
        sickInhabitantCount.text = "" + houseInfo.getNumSickInhabitants() + "/" + houseInfo.getNumInhabitants();
        atHospitalCount.text = "" + houseInfo.getNumInhabitantsAtHospital();
        employedInhabCount.text = "" + houseInfo.getNumEmployedInhabs();
        foodCount.text = "" + storage.getFoodCount();
        waterCount.text = "" + houseInfo.getWaterCount();
        entertainmentLevel.text = "" + houseInfo.getEntertainmentLevel();
        goldCount.text = "" + houseInfo.getHouseholdCurrency();
	}
}
