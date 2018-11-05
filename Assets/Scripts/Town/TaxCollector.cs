using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TaxCollector : MonoBehaviour {
    public GameObject taxCollectorOrc;
    public float taxBreakTime;

    private GameObject world;
    private World myWorld;
    private GameObject[,] constructArr;
    private bool orcOutForCollection;
    private Employment employment;
    private float taxPercentage;
    private int taxesCollected;
    private float timeUntilNextTax;

    /**
     * Initializes the marketplace.
     */
    void Start()
    {
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        employment = gameObject.GetComponent<Employment>();
        taxPercentage = myWorld.getTaxPercentage();
        taxesCollected = 0;
        timeUntilNextTax = 0.0f;
    }

    /**
     * Creates a worker for collecting taxes
     */
    void Update ()
    {
        orcOutForCollection = employment.getWorkerDeliveringGoods();

        if (orcOutForCollection == false && employment.getNumWorkers() > 0 && Time.time > timeUntilNextTax)
        {
            orcOutForCollection = true;
            employment.setWorkerDeliveringGoods(true);
            createCollectionOrc();
        }
    }

    /**
     * Creates an orc to collect resources for the marketplace to distribute.
     */
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
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1)].tag == "Road")
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1));
                foundSpawn = true;
            }
            //checking the row above the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)].tag == "Road")
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
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag == "Road")
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j));
                foundSpawn = true;
            }
            //checking the column to the right of the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag == "Road")
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

    /**
     * Sets the boolean status of whether or not the collector worker
     * is out for collection or at the market.
     * @param status is whether or not the orc is out for collection
     */
    public void setCollectorStatus(bool status)
    {
        if (status == false)
        {
            timeUntilNextTax = Time.time + taxBreakTime;
        }
        employment.setWorkerDeliveringGoods(status);
        orcOutForCollection = status;
    }

    /**
     * Gets whether the orc is currently out collecting taxes
     * @return orcOutForCollection whether the orc is currently out collecting taxes
     */
    public bool getCollectorStatus()
    {
        return orcOutForCollection;
    }

    /**
     * Updates the city's currency based on taxes received from the tax collector
     * @param currencyFromTaxes the currency received from taxes to date
     */
    public void updateCityCurrencyFromTaxes(int currencyFromTaxes)
    {
        taxesCollected += currencyFromTaxes;
        myWorld.updateCurrency(currencyFromTaxes);
    }

    /**
     * Gets the amount of currency collected by this building
     * @param taxesCollected the amount of currency collected by this building
     */
    public int getTaxesCollected()
    {
        return taxesCollected;
    }
}
