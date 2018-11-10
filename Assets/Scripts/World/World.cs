using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Contains information about the world the player is in.
 */
public class World : MonoBehaviour {
    private int populationCount;
    private float paymentTime;

    public float paymentInterval;
    public float taxPercentage;
    public int currencyCount;
    public ConstructionNetwork constructNetwork;
    public TerrainNetwork terrainNetwork;
    public int mapSize;
    public Vector2 spawnLocation;//location where immigrants will spawn in
    public Vector2 exitLocation;//locations where emmigrants will exit the world
    public List<string> walkableTerrain;
    public List<string> nonWalkableTerrain;
    public List<string> buildableTerrain;
    public List<string> aqueductTerrain;
    public List<string> mountainousTerrain;
    public List<string> wateryTerrain;

    //Strings that are used in multiple places
    public const string JOB_PAYMENT = "Job Payment";
    public const string TAX = "Tax";
    public const string ROAD = "Road";
    public const string STAIRS = "Stairs";

    private PopulationAndCurrency populationAndCurrency;

    /**
     * Initializes the necessary information for a world.
     */
    void Awake()
    {
        populationCount = 0;
        paymentTime = paymentInterval;
        //eventually change these to be general and passed in by a level
        // manager
        constructNetwork = new ConstructionNetwork();
        terrainNetwork = new TerrainNetwork();
        mapSize = 42;
        spawnLocation = new Vector2(0f, 5.41f);
        exitLocation = new Vector2(39f, 5.41f);

        GameObject populationAndCurrencyUI = GameObject.Find("PopulationAndCurrency");
        populationAndCurrency = populationAndCurrencyUI.GetComponent<PopulationAndCurrency>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (paymentInterval > 0.0f && Time.time > paymentTime)
        {
            paymentTime = Time.time + paymentInterval;
        }
	}

    /**
     * Gets the current population in the city
     * @return the city's current population
     */
    public int getPopulation()
    {
        return this.populationCount;
    }

    /**
     * Updates the city's population count
     * @param populationChange how much the population count should change by
     */
    public void updatePopulation(int populationChange)
    {
        this.populationCount += populationChange;
        populationAndCurrency.updatePopulationCount(this.populationCount);
    }

    /**
     * Gets the city's currency count
     * @return currencyCount the amount of currency the city currently has
     */
    public int getCurrency()
    {
        return this.currencyCount;
    }

    /**
     * Updates the amount of currency in the city
     * @param currencyChange how much the currency will change by
     */
    public void updateCurrency(int currencyChange)
    {
        this.currencyCount += currencyChange;
        populationAndCurrency.updateCurrencyCount(this.currencyCount);
    }

    /**
     * Get the time at which upkeep should occur and workers should be paid
     * @return paymentTime the time for pay to go out
     */
    public float getPaymentTime()
    {
        return paymentTime;
    }

    public float getTaxPercentage()
    {
        return taxPercentage;
    }

    //with a level manager, I will need getters and setters for information such as "spawnLocation" so that
    // the level manager can input the data and all other classes will get the data from getters
}
