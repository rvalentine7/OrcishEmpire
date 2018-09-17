using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Contains information regarding the workers at a place of employment. All objects tagged "Building"
 * are also places of employment.
 */
public class Employment : MonoBehaviour {
    public int workerCap;
    public int workerValue;
    private int numWorkers;
    private Dictionary<GameObject, int> workerHouses;
    private bool openForBusiness;
    private bool workerDeliveringGoods;
    private int numWaterSources;
    private GameObject world;
    private World myWorld;

    /**
     * Initializes necessary variables
     */
    void Start () {
        workerHouses = new Dictionary<GameObject, int>();
        openForBusiness = false;
        workerDeliveringGoods = false;
        numWaterSources = 0;
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
        //Check if tiles already have water, update numWaterSources if so
        int width = (int)gameObject.GetComponent<BoxCollider2D>().size.x;
        int height = (int)gameObject.GetComponent<BoxCollider2D>().size.y;
        for (int r = 0;  r < height; r++)
        {
            for (int c = 0;  c < width; c++)
            {
                if (terrainArr[Mathf.RoundToInt(gameObject.transform.position.x) - Mathf.CeilToInt(width / 2.0f - 1) + c,
                    Mathf.RoundToInt(gameObject.transform.position.y) - Mathf.CeilToInt(height / 2.0f - 1) + r].GetComponent<Tile>().hasWater())
                {
                    addWaterSource();
                }
            }
        }
    }

    /**
     * A place of employment can only take in workers if it is connected to a road.
     */
    void Update ()
    {
        GameObject world = GameObject.Find("WorldInformation");
        World myWorld = world.GetComponent<World>();
        GameObject[,] structArr = myWorld.constructNetwork.getConstructArr();
        int width = (int) gameObject.GetComponent<BoxCollider2D>().size.x;
        int height = (int) gameObject.GetComponent<BoxCollider2D>().size.y;
        //checking areas around the object to see if there is a road
        Vector2 employmentPos = gameObject.transform.position;
        openForBusiness = false;
        int i = 0;
        while (openForBusiness == false && i < width)
        {
            //checking the row below the gameObject
            if (structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1)].tag == "Road")
            {
                openForBusiness = true;
            }
            //checking the row above the gameObject
            else if (structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)].tag == "Road")
            {
                openForBusiness = true;
            }
            i++;
        }
        int j = 0;
        while (openForBusiness == false && j < height)
        {
            //checking the column to the left of the gameObject
            if (structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag == "Road")
            {
                openForBusiness = true;
            }
            //checking the column to the right of the gameObject
            else if (structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag == "Road")
            {
                openForBusiness = true;
            }
            j++;
        }
        //if the employment loses access to roads while running, its employees become unemployed
        if (!openForBusiness)
        {
            removeAllWorkers();
        }
    }

    /**
     * Tells if this place is open for business
     * @return whether or not this place of employment is currently operating
     */
    public bool getOpenForBusiness()
    {
        return openForBusiness;
    }

    /**
     * Returns how many job slots are still available for orcs to work in.
     * @return the number of available jobs at the work place
     */
    public int numJobsAvailable()
    {
        return workerCap - numWorkers;
    }

    /**
     * Adds a worker to the pig farm.
     * @param num the number of workers to add
     * @param workerHouse the house the workers came from
     */
    public void addWorker(int num, GameObject workerHouse)
    {
        if (numWorkers < workerCap)
        {
            numWorkers += num;
            if (workerHouses.ContainsKey(workerHouse))
            {
                workerHouses[workerHouse] += num;
            } else
            {
                workerHouses.Add(workerHouse, num);
            }
        }
    }

    /**
     * Removes workers from the employment.
     * @param num the number of workers to remove from the work place
     * @param workerHouse the house the removed workers came from
     */
    public void removeWorkers(int num, GameObject workerHouse)
    {
        if (numWorkers >= num)
        {
            numWorkers -= num;
            if (workerHouses[workerHouse] == num)
            {
                workerHouses.Remove(workerHouse);
            }
        }
        else
        {
            Debug.Log("An employment is having issues with the removeWorkers method.");
        }
    }

    /**
     * Removes all of the workers from the place of employment
     */
    public void removeAllWorkers()
    {
        foreach (KeyValuePair<GameObject, int> kvp in workerHouses)
        {
            HouseInformation houseInfo = kvp.Key.GetComponent<HouseInformation>();
            houseInfo.removeWorkLocation(gameObject);
        }
        workerHouses = new Dictionary<GameObject, int>();
        numWorkers = 0;
    }

    /**
     * Destroy this employment and unemploy the workers.
     */
    public void destroyEmployment()
    {
        //lets the houses of the workers know the inhabitants are now unemployed
        if (numWorkers > 0)
        {
            foreach(KeyValuePair<GameObject, int> kvp in workerHouses)
            {
                HouseInformation houseInfo = kvp.Key.GetComponent<HouseInformation>();
                houseInfo.removeWorkLocation(gameObject);
            }
        }
        //Water supplying sources need to stop supplying water
        if (gameObject.GetComponent<Reservoir>() != null)
        {
            gameObject.GetComponent<Reservoir>().updatePipes(false);
        }
        else if (gameObject.GetComponent<Fountain>() != null)
        {
            gameObject.GetComponent<Fountain>().updateWaterSupplying(false);
        }
        else if (gameObject.GetComponent<Well>() != null)
        {
            gameObject.GetComponent<Well>().updateWaterSupplying(false);
        }
        Destroy(gameObject);
    }

    /**
     * Gets the number of workers currently at the work place.
     * @return the current number of workers here
     */
    public int getNumWorkers()
    {
        return numWorkers;
    }

    /**
     * Gets the value for how much work an employee does during one time interval.
     * @return the amount of work each employee puts in during one time interval
     */
    public int getWorkerValue()
    {
        return workerValue;
    }

    /**
     * Gets the maximum number of workers allowed at the work place.
     * @return the maximum number of workers who can be employed here
     */
    public int getWorkerCap()
    {
        return workerCap;
    }

    /**
     * Sets whether or not a worker is out delivering goods for the employment.
     * @param delivering whether or not a worker is out delivering goods
     */
    public void setWorkerDeliveringGoods(bool delivering)
    {
        workerDeliveringGoods = delivering;
    }

    /**
     * Gets whether or not a worker is out delivering goods
     * @return whether or not a worker is out delivering goods
     */
    public bool getWorkerDeliveringGoods()
    {
        return workerDeliveringGoods;
    }

    /**
     * Adds a source of water to the building
     * @param waterSource the source the water is coming from
     */
    public void addWaterSource()
    {
        numWaterSources++;//TODO: remove and update to be like removeWaterSource()
        if (gameObject.GetComponent<Fountain>() != null && numWaterSources >= 1)
        {
            gameObject.GetComponent<Fountain>().updateFilled(true);
        }
    }

    /**
     * Removes a source of water to the building
     * @param waterSource the source of water to remove
     */
    public void removeWaterSource()
    {
        numWaterSources--;//TODO: remove
        bool hasWaterAccess = false;
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
        BoxCollider2D boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        int width = Mathf.RoundToInt(boxCollider2D.size.x);
        int height = Mathf.RoundToInt(boxCollider2D.size.y);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (terrainArr[(int)gameObject.transform.position.x + i - Mathf.CeilToInt(width / 2.0f - 1),
                    (int)gameObject.transform.position.y + j - Mathf.CeilToInt(height / 2.0f - 1)].GetComponent<Tile>().hasWater())
                {
                    hasWaterAccess = true;
                }
            }
        }
        if (gameObject.GetComponent<Fountain>() != null && hasWaterAccess/* && numWaterSources == 0*/)
        {
            gameObject.GetComponent<Fountain>().updateFilled(false);
        }
    }
}
