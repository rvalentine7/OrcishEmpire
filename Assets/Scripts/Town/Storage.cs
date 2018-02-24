using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Storage holds various types of resources which buildings such as Warehouses and Marketplaces use
 * to help provide resources to citizens.
 */
public class Storage : MonoBehaviour {
    public string storageType;
    //foods
    private int meatCount;
    private int wheatCount;
    private int fishCount;
    //water
    private int waterCount;
    //crafting resources
    private int lumberCount;
    private int ironCount;
    //sellable goods
    private int furnitureCount;
    private int weaponsCount;
    private int armorCount;
    //non-tangible resources (have their own max limit)
    private int entertainment;
    //maximum amount of tangible resources this storage building can hold
    public int storageMax;

    /**
     * Initializes the storage
     */
    void Start () {
        meatCount = 0;
        wheatCount = 0;
        fishCount = 0;
        waterCount = 0;
        lumberCount = 0;
        ironCount = 0;
        furnitureCount = 0;
        weaponsCount = 0;
        armorCount = 0;
    }

    /**
     * Gets the total amount of storage space this building is capable of.
     * @return storageMax the maximum room available in this storage facility
     */
    public int getStorageMax()
    {
        return storageMax;
    }

    /**
     * Gets how much storage is currently taken up.
     * @return the amount of resources stored in this storage facility
     */
    public int getCurrentAmountStored()
    {
        //TODO: add up other resources as they are added to the game
        return meatCount + wheatCount + fishCount + waterCount + lumberCount + ironCount + furnitureCount + weaponsCount + armorCount;
    }

    /**
     * Determines whether or not this storage facility accepts a type of resource and if it does, if it has room
     * for the resource.
     * @param name the name of resource to check if this storage can accept it
     * @param num the amount of the resource to check if this storage can accept it
     */
    public bool acceptsResource(string name, int num)
    {
        bool accepts = false;
        if (storageType.Equals("Market") || storageType.Equals("Warehouse") || storageType.Equals("House"))
        {
            //foods
            if (name.Equals("Meat") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
            if (name.Equals("Wheat") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
            if (name.Equals("Fish") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
            //crafting resources
            if (name.Equals("Lumber") && num <= storageMax - getCurrentAmountStored())
            {

            }
            if (name.Equals("Iron") && num <= storageMax - getCurrentAmountStored())
            {

            }
            //sellable goods
            if (name.Equals("Furniture") && num <= storageMax - getCurrentAmountStored())
            {
                
            }
        }
        if (storageType.Equals("House"))
        {
            HouseInformation houseInfo = GetComponent<HouseInformation>();
            if (name.Equals("Water") && num <= storageMax && houseInfo.getNumInhabitants() > 0)
            {
                accepts = true;
            }
            if (houseInfo.getNumInhabitants() == 0)
            {
                accepts = false;
            }
        }
        return accepts;
    }

    /**
     * Adds a resource to the storage
     * @param name the name of the resource to add
     * @param num the amount of the resource to add
     */
    public void addResource(string name, int num)
    {
        if (name.Equals("Meat"))
        {
            meatCount += num;
        }
        else if (name.Equals("Wheat"))
        {
            wheatCount += num;
        }
        else if (name.Equals("Water"))
        {
            if (waterCount + num <= 100)
            {
                waterCount += num;
            }
            else
            {
                waterCount = 100;
            }
        }
    }

    /**
     * Removes a resource from the storage
     * @param name the name of the resource to remove
     * @param num the amount of the resource to remove
     */
    public void removeResource(string name, int num)
    {
        if (name.Equals("Meat"))
        {
            meatCount -= num;
        }
        else if (name.Equals("Wheat"))
        {
            wheatCount -= num;
        }
        else if (name.Equals("Water"))
        {
            waterCount -= num;
        }
    }

    /**
     * Returns the total amount of meat in storage
     * @return meatCount the amount of meat in storage
     */
    public int getMeatCount()
    {
        return meatCount;
    }

    /**
     * Returns the total amount of wheat in storage
     * @return wheatCount the amount of wheat in storage
     */
    public int getWheatCount()
    {
        return wheatCount;
    }

    /**
     * Gets the total amount of food at this storage facility.
     * @return the total amount of food in storage
     */
    public int getFoodCount()
    {
        //TODO: add other food types to this count as they are added to the game
        return meatCount + wheatCount + fishCount;
    }
    
    /**
     * Returns the total amount of water in storage
     * @return waterCount the amount of water in storage
     */
    public int getWaterCount()
    {
        return waterCount;
    }
}
