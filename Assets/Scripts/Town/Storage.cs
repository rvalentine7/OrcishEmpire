﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Storage holds various types of resources which buildings such as Warehouses and Marketplaces use
 * to help provide resources to citizens.
 */
public class Storage : MonoBehaviour {
    public string storageType;
    private int meatCount;
    public int storageMax;

    /**
     * Initializes the storage
     */
    void Start () {
        meatCount = 0;
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
        return meatCount;
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
        if (storageType.Equals("Market") || storageType.Equals("Warehouse"))
        {
            if (name.Equals("Meat") && num <= (storageMax - meatCount))
            {
                accepts = true;
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
            addMeat(num);
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
            removeMeat(num);
        }
    }

    /**
     * Adds meat to the storage.
     * @param num the number of meat to add
     */
    private void addMeat(int num)
    {
        meatCount += num;
    }

    /**
     * Removes meat from the storage.
     * @param num the number of meat to remove
     */
    private void removeMeat(int num)
    {
        meatCount -= num;
    }

    /**
     * Returns both the total amount of meat in storage
     * @return meatCount the amount of meat in storage
     */
    public int getMeatCount()
    {
        return meatCount;
    }

    /**
     * Gets the total amount of food at this storage facility.
     * @return the total amount of food in storage
     */
    public int getFoodCount()
    {
        //TODO: add other food types to this count as they are added to the game
        return meatCount;
    }
}
