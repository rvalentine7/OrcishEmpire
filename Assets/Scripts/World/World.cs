using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains information about the world the player is in.
/// </summary>
public class World : MonoBehaviour {
    private int populationCount;
    private float paymentTime;
    private float manageTradeTime;

    //A queue to spread out how quickly immigrants can move in
    private Queue<GameObject> homesToMoveInTo = new Queue<GameObject>();
    private float nextEarliestMoveInTime = 0.0f;

    //Contains lists of each of the water sections in the map
    private List<List<WaterTile>> waterSections = new List<List<WaterTile>>();
    private int lowestEmptyWaterSection = 0;

    public float paymentInterval;
    public float manageTradeInterval;
    public float taxPercentage;
    public int currencyCount;
    public ConstructionNetwork constructNetwork;
    public TerrainNetwork terrainNetwork;
    public int mapSize;
    public Vector2 spawnLocation;//location where immigrants will spawn in
    public Vector2 exitLocation;//locations where emmigrants will exit the world
    public int wheatCost;
    public int meatCost;
    public int fishCost;
    public int eggsCost;
    public int hopsCost;
    public int beerCost;
    public int lumberCost;
    public int furnitureCost;
    public int ironCost;
    public int weaponCost;
    public int ochreCost;
    public int warPaintCost;
    public List<string> walkableTerrain;
    public List<string> nonWalkableTerrain;
    public List<string> buildableTerrain;
    public List<string> aqueductTerrain;
    public List<string> mountainousTerrain;
    public List<string> wateryTerrain;

    //Trade Management
    private TradeManager tradeManager;

    //Value used by multiple different buildings
    public const int HEALTH_BUILDING_RADIUS = 15;

    //Strings that are used in multiple places
    public const string WORLD_INFORMATION = "WorldInformation";
    public const string JOB_PAYMENT = "Job Payment";
    public const string TAX = "Tax";
    //Buildings
    public const string HOUSE = "House";
    public const string BUILDING = "Building";
    public const string BUILDINGS = "Buildings";
    public const string TALL_BUILDINGS = "TallBuildings";
    public const string WAREHOUSE = "Warehouse";
    public const string ROAD = "Road";
    public const string STAIRS = "Stairs";
    public const string LOW_BRIDGE = "LowBridge";
    public const string HIGH_BRIDGE = "HighBridge";
    //Terrain
    public const string WATER = "Water";
    public const string TREES = "Trees";
    public const string FISHING_SPOT = "FishingSpot";
    //UI
    public const string BUILD_OBJECT = "BuildObject";
    public const string POPUP = "Popup";
    public const string WORLD_UI = "WorldUI";
    public const string BUILD_PANEL = "BuildPanel";
    //Resources
    public const string MEAT = "Meat";
    public const string WHEAT = "Wheat";
    public const string EGGS = "Eggs";
    public const string FISH = "Fish";
    public const string HOPS = "Hops";
    public const string BEER = "Beer";
    public const string LUMBER = "Lumber";
    public const string FURNITURE = "Furniture";
    public const string IRON = "Iron";
    public const string WEAPON = "Weapon";
    public const string OCHRE = "Ochre";
    public const string WAR_PAINT = "WarPaint";

    private PopulationAndCurrency populationAndCurrency;

    /// <summary>
    /// Initializes the necessary information for a world.
    /// </summary>
    void Awake()
    {
        populationCount = 0;
        paymentTime = paymentInterval;
        manageTradeTime = manageTradeInterval;
        //eventually change these to be general and passed in by a level
        // manager
        constructNetwork = new ConstructionNetwork();
        terrainNetwork = new TerrainNetwork();
        mapSize = 42;
        spawnLocation = new Vector2(0f, 5.41f);
        exitLocation = new Vector2(39f, 5.41f);

        GameObject populationAndCurrencyUI = GameObject.Find("ResourcesPanel");
        populationAndCurrency = populationAndCurrencyUI.GetComponent<PopulationAndCurrency>();

        tradeManager = new TradeManager(this);
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        //Updates the time at which workers should be paid
        if (paymentInterval > 0.0f && Time.time > paymentTime)
        {
            paymentTime = Time.time + paymentInterval;
        }

        //Spreads out how often immigrants are moving in to houses
        if (homesToMoveInTo.Count > 0 && Time.time > nextEarliestMoveInTime)
        {
            //0.5f, 0.0f, and 1.0f are all random times I've chosen to spread out how quickly a new immigrant can be created
            nextEarliestMoveInTime = Time.time + 0.5f + Random.Range(0.0f, 1.0f);
            GameObject homeThatCanBeMovedInTo = homesToMoveInTo.Dequeue();
            if (homeThatCanBeMovedInTo)
            {
                homeThatCanBeMovedInTo.GetComponent<AvailableHome>().spawnImmigrant();
            }
        }

        //Manage Trade Routes
        if (Time.time > manageTradeTime)
        {
            manageTradeTime = Time.time + manageTradeInterval;
            tradeManager.manageTradeRoutes();
        }
    }

    /// <summary>
    /// Gets the current population in the city
    /// </summary>
    /// <returns>the city's current population</returns>
    public int getPopulation()
    {
        return this.populationCount;
    }

    /// <summary>
    /// Updates the city's population count
    /// </summary>
    /// <param name="populationChange">how much the population count should change by</param>
    public void updatePopulation(int populationChange)
    {
        this.populationCount += populationChange;
        populationAndCurrency.updatePopulationCount(this.populationCount);
    }

    /// <summary>
    /// Gets the city's currency count
    /// </summary>
    /// <returns>The amount of currency the city currently has</returns>
    public int getCurrency()
    {
        return this.currencyCount;
    }

    /// <summary>
    /// Updates the amount of currency in the city
    /// </summary>
    /// <param name="currencyChange">how much the currency will change by</param>
    public void updateCurrency(int currencyChange)
    {
        this.currencyCount += currencyChange;
        populationAndCurrency.updateCurrencyCount(this.currencyCount);
    }

    /// <summary>
    /// Get the time at which upkeep should occur and workers should be paid
    /// </summary>
    /// <returns>The time for pay to go out</returns>
    public float getPaymentTime()
    {
        return paymentTime;
    }

    /// <summary>
    /// Gets the tax percentage for the city
    /// </summary>
    /// <returns>The tax percentage for the city</returns>
    public float getTaxPercentage()
    {
        return taxPercentage;
    }

    /// <summary>
    /// Adds a home that immigrants are trying to move in to
    /// </summary>
    /// <param name="availableHome">The home immigrants are trying to move in to</param>
    public void addHomeToMoveInTo(GameObject availableHome)
    {
        homesToMoveInTo.Enqueue(availableHome);
    }

    /// <summary>
    /// Adds a water tile to a water section based on its number
    /// </summary>
    /// <param name="waterTile">The water tile to add</param>
    public void addToWaterSection(WaterTile waterTile)
    {
        int waterSectionIndex = waterTile.getWaterSectionNum();
        //If the water section index is >= the number of water sections, a new list is needed
        if (waterSectionIndex == waterSections.Count)
        {
            List<WaterTile> waterTiles = new List<WaterTile>();
            waterTiles.Add(waterTile);
            waterSections.Add(waterTiles);
        }
        else if (waterSectionIndex < waterSections.Count)
        {
            waterSections[waterSectionIndex].Add(waterTile);
        }
        else
        {
            Debug.LogError("World.cs: Trying to add a water section more than 1 index beyond the current highest number index");
        }

        //If a water section was just added to an empty index. Find the new lowest index after this one
        if (waterSectionIndex == lowestEmptyWaterSection)
        {
            lowestEmptyWaterSection++;
            bool foundLowest = false;
            while (lowestEmptyWaterSection < waterSections.Count && !foundLowest)
            {
                if (waterSections[lowestEmptyWaterSection].Count == 0)
                {
                    foundLowest = true;
                }
                lowestEmptyWaterSection++;
            }
        }
    }

    /// <summary>
    /// Removes a water tile from the associated water section
    /// </summary>
    /// <param name="waterTile">The water tile to remove</param>
    public void removeFromWaterSection(WaterTile waterTile)
    {
        int waterSectionIndex = waterTile.getWaterSectionNum();
        //Index -1 is not stored in the data structure and the index must exist
        if (waterSectionIndex != -1 && waterSectionIndex < waterSections.Count)
        {
            waterSections[waterSectionIndex].Remove(waterTile);
            //If this becomes empty and is a lower index than the current lowest empty index
            if (waterSections[waterSectionIndex].Count == 0 && waterSectionIndex < lowestEmptyWaterSection)
            {
                lowestEmptyWaterSection = waterSectionIndex;
            }
        }
    }

    /// <summary>
    /// Gets the number of water tiles in a section
    /// </summary>
    /// <param name="waterSectionIndex"></param>
    /// <returns>The number of water tiles in a section</returns>
    public int getNumInWaterSection(int waterSectionIndex)
    {
        //Can only return a number for water sections that exist and are not part of -1
        return (waterSectionIndex == -1 || waterSectionIndex >= waterSections.Count) ? -1 : waterSections[waterSectionIndex].Count;
    }

    /// <summary>
    /// Gets the lowest empty water section number
    /// </summary>
    /// <returns>The lowest empty water section</returns>
    public int getFirstEmptyWaterSection()
    {
        //return waterSections.Count;
        return lowestEmptyWaterSection;
    }

    /// <summary>
    /// Gets the distance between two points
    /// </summary>
    /// <param name="firstLocation">The first point</param>
    /// <param name="secondLocation">The second point</param>
    /// <returns>The distance between two points</returns>
    public float getDistanceBetweenPoints(Vector2 firstLocation, Vector2 secondLocation) {
        return Mathf.Sqrt(Mathf.Pow(secondLocation.x - firstLocation.x, 2) + Mathf.Pow(secondLocation.y - firstLocation.y, 2));
    }

    /// <summary>
    /// Gets the trade manager for this world
    /// </summary>
    /// <returns>The trade manager for this world</returns>
    public TradeManager getTradeManager()
    {
        return this.tradeManager;
    }

    /// <summary>
    /// Returns the cost of a particular goods item from its name
    /// </summary>
    /// <param name="goodsName">The name of the goods to return the cost of</param>
    /// <returns>The cost of a particular goods item</returns>
    public int getGoodsCost(string goodsName)
    {
        goodsName = goodsName.Replace(" ", "");

        if (goodsName.Equals(WHEAT))
        {
            return wheatCost;
        }
        if (goodsName.Equals(MEAT))
        {
            return meatCost;
        }
        if (goodsName.Equals(FISH))
        {
            return fishCost;
        }
        if (goodsName.Equals(EGGS))
        {
            return eggsCost;
        }
        if (goodsName.Equals(HOPS))
        {
            return hopsCost;
        }
        if (goodsName.Equals(BEER))
        {
            return beerCost;
        }
        if (goodsName.Equals(LUMBER))
        {
            return lumberCost;
        }
        if (goodsName.Equals(FURNITURE))
        {
            return furnitureCost;
        }
        if (goodsName.Equals(IRON))
        {
            return ironCost;
        }
        if (goodsName.Equals(WEAPON))
        {
            return weaponCost;
        }
        if (goodsName.Equals(OCHRE))
        {
            return ochreCost;
        }
        if (goodsName.Equals(WAR_PAINT))
        {
            return warPaintCost;
        }
         return -1;
    }

    //with a level manager, I will need getters and setters for information such as "spawnLocation" so that
    // the level manager can input the data and all other classes will get the data from getters
}
