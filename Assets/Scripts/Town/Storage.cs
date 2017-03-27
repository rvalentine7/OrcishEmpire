using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Storage holds various types of resources which buildings such as Warehouses and Marketplaces use
 * to help provide resources to citizens.
 */
public class Storage : MonoBehaviour {
    public string storageType;
    private int meatCount;
    private int meatMax;

    /**
     * Initializes the storage
     */
    void Start () {
        meatCount = 0;
        meatMax = 100;
    }

    /**
     * Determines whether or not this storage facility accepts a type of resource and if it does, if it has room
     * for the resource.
     */
    public bool acceptsResource(string name, int num)
    {
        bool accepts = false;
        if (storageType.Equals("Market"))
        {
            if (name.Equals("Meat") && num <= (meatMax - meatCount))
            {
                accepts = true;
            }
        }
        return accepts;
    }

    /**
     * Adds a resource to the storage
     */
    public void addResource(string name, int num)
    {
        if (name.Equals("Meat"))
        {
            addMeat(num);
        }
    }

    /**
     * Adds meat to the sstorage.
     */
    private void addMeat(int num)
    {
        meatCount += num;
    }

    /**
     * Returns both the total amount of meat as well as the max
     * number storage can hold.
     */
    public List<int> getMeatInfo()
    {
        List<int> meatInfo = new List<int>();
        meatInfo.Add(meatCount);
        meatInfo.Add(meatMax);
        return meatInfo;
    }
}
