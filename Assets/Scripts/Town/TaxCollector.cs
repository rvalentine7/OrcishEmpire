using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A building that creates a tax collector to go around collecting taxes for the city
/// </summary>
public class TaxCollector : Building {
    public GameObject taxCollectorOrc;
    public float taxBreakTime;
    
    private World myWorld;
    private GameObject[,] constructArr;
    private bool orcOutForCollection;
    private Employment employment;
    private float taxPercentage;
    private int taxesCollected;
    private float timeUntilNextTax;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        employment = gameObject.GetComponent<Employment>();
        taxPercentage = myWorld.getTaxPercentage();
        taxesCollected = 0;
        timeUntilNextTax = 0.0f;
    }

    /// <summary>
    /// Creates a worker for collecting taxes
    /// </summary>
    void Update ()
    {
        orcOutForCollection = employment.getWorkerDeliveringGoods();

        if (orcOutForCollection == false && employment.getNumHealthyWorkers() > 0 && Time.time > timeUntilNextTax)
        {
            orcOutForCollection = true;
            employment.setWorkerDeliveringGoods(true);
            createCollectionOrc();
        }
    }

    /// <summary>
    /// Creates an orc to collect resources for the marketplace to distribute.
    /// </summary>
    public void createCollectionOrc()
    {
        GameObject[,] structArr = myWorld.constructNetwork.getConstructArr();
        int width = (int)gameObject.GetComponent<BoxCollider2D>().size.x;
        int height = (int)gameObject.GetComponent<BoxCollider2D>().size.y;
        //checking areas around the farm to place an orc on a road
        Vector2 employmentPos = gameObject.transform.position;
        bool foundSpawn = false;
        Vector2 spawnPosition = new Vector2();
        int i = 0;
        while (!foundSpawn && i < width)
        {
            //checking the row below the gameObject
            if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1)].tag == World.ROAD)
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1));
                foundSpawn = true;
            }
            //checking the row above the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)].tag == World.ROAD)
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1));
                foundSpawn = true;
            }
            i++;
        }
        int j = 0;
        while (!foundSpawn && j < height)
        {
            //checking the column to the left of the gameObject
            if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag == World.ROAD)
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j));
                foundSpawn = true;
            }
            //checking the column to the right of the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag == World.ROAD)
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j));
                foundSpawn = true;
            }
            j++;
        }

        GameObject newCollectorOrc = Instantiate(taxCollectorOrc, new Vector2(spawnPosition.x, spawnPosition.y + 0.4f), Quaternion.identity);
        CollectTaxes collectTaxes = newCollectorOrc.GetComponent<CollectTaxes>();
        collectTaxes.setTaxPercentage(taxPercentage / 100);
        collectTaxes.setOriginalLocation(spawnPosition);
        collectTaxes.setOrcEmployment(gameObject);
    }

    /// <summary>
    /// Sets the boolean status of whether or not the collector worker
    /// is out for collection or at the market.
    /// </summary>
    /// <param name="status">whether or not the orc is out for collection</param>
    public void setCollectorStatus(bool status)
    {
        if (status == false)
        {
            timeUntilNextTax = Time.time + taxBreakTime;
        }
        employment.setWorkerDeliveringGoods(status);
        orcOutForCollection = status;
    }

    /// <summary>
    /// Gets whether the orc is currently out collecting taxes
    /// </summary>
    /// <returns>whether the orc is currently out collecting taxes</returns>
    public bool getCollectorStatus()
    {
        return orcOutForCollection;
    }

    /// <summary>
    /// Updates the city's currency based on taxes received from the tax collector
    /// </summary>
    /// <param name="currencyFromTaxes">the currency received from taxes to date</param>
    public void updateCityCurrencyFromTaxes(int currencyFromTaxes)
    {
        taxesCollected += currencyFromTaxes;
        myWorld.updateCurrency(currencyFromTaxes);
    }

    /// <summary>
    /// Gets the amount of currency collected by this building
    /// </summary>
    /// <returns>the amount of currency collected by this building</returns>
    public int getTaxesCollected()
    {
        return taxesCollected;
    }
}
