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
    private int eggCount;
    //water
    private int waterCount;
    //crafting resources
    private int lumberCount;
    private int ironCount;
    private int ochreCount;
    private int hopsCount;
    private int boatCount;
    //sellable goods
    private int furnitureCount;
    private int weaponsCount;
    private int armorCount;
    private int beerCount;
    private int warPaintCount;
    //private int treasureCount;
    //non-tangible resources (have their own max limit)
    private int entertainmentLevel;
    //maximum amount of tangible resources this storage building can hold
    public int storageMax;

    /**
     * Initializes the storage
     */
    void Start () {
        meatCount = 0;
        wheatCount = 0;
        fishCount = 0;
        eggCount = 0;

        waterCount = 0;

        lumberCount = 0;
        ironCount = 0;
        ochreCount = 0;
        hopsCount = 0;

        furnitureCount = 0;
        weaponsCount = 0;
        armorCount = 0;
        beerCount = 0;
        warPaintCount = 0;
        //treasureCount = 0;

        entertainmentLevel = 0;
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
        //TODO: add up other tangible resources as they are added to the game
        return meatCount + wheatCount + eggCount + hopsCount + fishCount
            + waterCount + lumberCount + ironCount + furnitureCount
            + weaponsCount + armorCount + beerCount + ochreCount
            + warPaintCount;
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
        if (storageType.Equals("Market") || storageType.Equals("Warehouse") || storageType.Equals(World.HOUSE))
        {
            //foods
            if (name.Equals(World.MEAT) && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
            if (name.Equals(World.WHEAT) && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
            if (name.Equals(World.EGGS) && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
            if (name.Equals(World.FISH) && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
            //sellable goods
            if (name.Equals("Furniture") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
            if (name.Equals("Weapon") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
        }
        if (storageType.Equals(World.HOUSE))
        {
            HouseInformation houseInfo = GetComponent<HouseInformation>();
            if (name.Equals(World.WATER) && num <= storageMax && houseInfo.getNumInhabitants() > 0)
            {
                accepts = true;
            }
            if (houseInfo.getNumInhabitants() == 0)
            {
                accepts = false;
            }
            //Entertainment doesn't care about storage count because it is not tangible
            if (name.Equals("Entertainment") && houseInfo.getNumInhabitants() > 0)
            {
                accepts = true;
            }
        }
        if (storageType.Equals("Warehouse"))
        {
            //crafting resources
            if (name.Equals(World.LUMBER) && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
            if (name.Equals("Iron") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
            if (name.Equals("Hops") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
            if (name.Equals("Beer") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
            if (name.Equals("Ochre") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
            if (name.Equals("WarPaint") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
        }
        if (storageType.Equals("Weaponsmith"))
        {
            if (name.Equals("Iron") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
        }
        if (storageType.Equals("FurnitureWorkshop") || storageType.Equals("Boatyard"))
        {
            if (name.Equals(World.LUMBER) && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
        }
        if (storageType.Equals("Brewery"))
        {
            if (name.Equals("Hops") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
        }
        if (storageType.Equals("Pub"))
        {
            if (name.Equals("Beer") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
        }
        if (storageType.Equals("WarPaintWorkshop"))
        {
            if (name.Equals("Ochre") && num <= storageMax - getCurrentAmountStored())
            {
                accepts = true;
            }
        }
        //TODO: add other ItemProduction buildings here and check their required raw resources
        return accepts;
    }

    /**
     * Adds a resource to the storage
     * @param name the name of the resource to add
     * @param num the amount of the resource to add
     */
    public void addResource(string name, int num)
    {
        if (name.Equals(World.MEAT))
        {
            meatCount += num;
        }
        else if (name.Equals(World.WHEAT))
        {
            wheatCount += num;
        }
        else if (name.Equals(World.EGGS))
        {
            eggCount += num;
        }
        else if (name.Equals(World.FISH))
        {
            fishCount += num;
        }
        else if (name.Equals("Hops"))
        {
            hopsCount += num;
        }
        else if (name.Equals("Beer"))
        {
            beerCount += num;
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
        else if (name.Equals("Iron"))
        {
            ironCount += num;
        }
        else if (name.Equals("Weapon"))
        {
            weaponsCount += num;
        }
        else if (name.Equals(World.LUMBER))
        {
            lumberCount += num;
        }
        else if (name.Equals("Furniture"))
        {
            furnitureCount += num;
        }
        else if (name.Equals("Ochre"))
        {
            ochreCount += num;
        }
        else if (name.Equals("WarPaint"))
        {
            warPaintCount += num;
        }
        else if (name.Equals("Entertainment"))
        {
            entertainmentLevel += num;
        }
    }

    /**
     * Removes a resource from the storage
     * @param name the name of the resource to remove
     * @param num the amount of the resource to remove
     */
    public void removeResource(string name, int num)
    {
        if (name.Equals(World.MEAT))
        {
            meatCount -= num;
        }
        else if (name.Equals(World.WHEAT))
        {
            wheatCount -= num;
        }
        else if (name.Equals(World.EGGS))
        {
            eggCount -= num;
        }
        else if (name.Equals(World.FISH))
        {
            fishCount -= num;
        }
        else if (name.Equals("Hops"))
        {
            hopsCount -= num;
        }
        else if (name.Equals("Beer"))
        {
            beerCount -= num;
        }
        else if (name.Equals(World.WATER))
        {
            waterCount -= num;
        }
        else if (name.Equals("Iron"))
        {
            ironCount -= num;
        }
        else if (name.Equals("Weapon"))
        {
            weaponsCount -= num;
        }
        else if (name.Equals(World.LUMBER))
        {
            lumberCount -= num;
        }
        else if (name.Equals("Furniture"))
        {
            furnitureCount -= num;
        }
        else if (name.Equals("Ochre"))
        {
            ochreCount -= num;
        }
        else if (name.Equals("WarPaint"))
        {
            warPaintCount -= num;
        }
        else if (name.Equals("Entertainment"))
        {
            entertainmentLevel -= num;
        }
    }

    /**
     * Gets the number of a specified resource type that this storage holds
     * @param resourceName the name of the resource to check the count of
     * @return the amount of the specified resource
     */
    public int getResourceCount(string resourceName)
    {
        if (resourceName.Equals(World.MEAT))
        {
            return getMeatCount();
        }
        else if (resourceName.Equals(World.WHEAT))
        {
            return getWheatCount();
        }
        else if (resourceName.Equals(World.EGGS))
        {
            return getEggCount();
        }
        else if (resourceName.Equals(World.FISH))
        {
            return getFishCount();
        }
        else if (resourceName.Equals("Hops"))
        {
            return getHopsCount();
        }
        else if (resourceName.Equals("Beer"))
        {
            return getBeerCount();
        }
        else if (resourceName.Equals("Water"))
        {
            return getWaterCount();
        }
        else if (resourceName.Equals("Iron"))
        {
            return getIronCount();
        }
        else if (resourceName.Equals("Weapon"))
        {
            return getWeaponCount();
        }
        else if (resourceName.Equals(World.LUMBER))
        {
            return getLumberCount();
        }
        else if (resourceName.Equals("Furniture"))
        {
            return getFurnitureCount();
        }
        else if (resourceName.Equals("Ochre"))
        {
            return getOchreCount();
        }
        else if (resourceName.Equals("WarPaint"))
        {
            return getWarPaintCount();
        }
        else if (resourceName.Equals("Entertainment"))
        {
            return getEntertainmentLevel();
        }
        return 0;
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
     * Returns the total amount of eggs in storage
     * @return eggCount the amount of eggs in storage
     */
    public int getEggCount()
    {
        return eggCount;
    }

    /// <summary>
    /// Returns the total amount of fish in storage
    /// </summary>
    /// <returns>The amount of fish in storage</returns>
    public int getFishCount()
    {
        return fishCount;
    }

    /**
     * Returns the total amount of hops in storage
     * @return hopsCount the amount of hops in storage
     */
    public int getHopsCount()
    {
        return hopsCount;
    }

    /**
     * Returns the total amount of beer in storage
     * @return beerCount the amount of beer in storage
     */
    public int getBeerCount()
    {
        return beerCount;
    }

    /**
     * Gets the total amount of food at this storage facility.
     * @return the total amount of food in storage
     */
    public int getFoodCount()
    {
        //TODO: add other food types to this count as they are added to the game
        return meatCount + wheatCount + eggCount + fishCount;
    }
    
    /**
     * Returns the total amount of water in storage
     * @return waterCount the amount of water in storage
     */
    public int getWaterCount()
    {
        return waterCount;
    }

    /**
     * Returns the total amount of iron in storage
     * @return waterCount the amount of iron in storage
     */
    public int getIronCount()
    {
        return ironCount;
    }

    /**
     * Returns the total amount of weapons in storage
     * @return weaponsCount the amount of weapons in storage
     */
    public int getWeaponCount()
    {
        return weaponsCount;
    }

    /**
     * Returns the total amount of lumber in storage
     * @return lumberCount the amount of lumber in storage
     */
    public int getLumberCount()
    {
        return lumberCount;
    }

    /**
     * Returns the total amount of furniture in storage
     * @return furnitureCount the amount of furniture in storage
     */
    public int getFurnitureCount()
    {
        return furnitureCount;
    }

    /**
     * Returns the total amount of ochre in storage
     * @return ochreCount the amount of ochre in storage
     */
     public int getOchreCount()
     {
         return ochreCount;
     }

    /**
     * Returns the total amount of war paint in storage
     * @return warPaintCount the amount of war paint in storage
     */
     public int getWarPaintCount()
     {
         return warPaintCount;
     }

    /**
     * Returns the current entertainment level
     * @return entertainmentLevel the current entertainment level
     */
    public int getEntertainmentLevel()
    {
        return entertainmentLevel;
    }
}
