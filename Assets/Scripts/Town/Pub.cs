using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pub : MonoBehaviour {
    private bool active;
    private GameObject world;
    private World myWorld;
    private GameObject[,] structureArr;
    private Storage myStorage;
    private bool orcMovingGoods;
    private float checkTime;
    private bool consumingBeer;

    public GameObject collectorOrc;
    public int collectorCarryCapacity;
    public int housingSearchRadius;
    public string requiredResourceName;
    public int beerConsumptionTime;
    public int beerConsumedEachTick;
    public int lowEmployeePenaltyNum;//TODO: at some point in time, will want to make this number be great enough
    // that even if I am only lacking 1 employee, there will be times where entertainment does not reach houses
    // before decaying

    // Use this for initialization
    void Start () {
        active = true;
        world = GameObject.Find(World.WORLD_INFORMATION);
        myWorld = world.GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        myStorage = gameObject.GetComponent<Storage>();
        orcMovingGoods = false;
        checkTime = 0.0f;
        consumingBeer = false;
    }
	
	// Update is called once per frame
	void Update () {
		if (active)
        {
            Employment employment = gameObject.GetComponent<Employment>();
            if (employment.getNumHealthyWorkers() > 0)
            {
                orcMovingGoods = employment.getWorkerDeliveringGoods();
                //Collect beer
                if (employment.getNumHealthyWorkers() > 0 && myStorage.getResourceCount(requiredResourceName) < myStorage.getStorageMax() && orcMovingGoods == false)
                {
                    employment.setWorkerDeliveringGoods(true);
                    createCollectionOrc();
                }
                
                if (myStorage.getBeerCount() > 0)
                {
                    //checkTime takes longer if there are fewer than the maximum number of employees
                    //consume beer
                    if (!consumingBeer)
                    {
                        checkTime = Time.time + beerConsumptionTime + (employment.getWorkerCap() - employment.getNumHealthyWorkers()) * lowEmployeePenaltyNum;
                        consumingBeer = true;
                    }
                    if (Time.time > checkTime && consumingBeer)
                    {
                        myStorage.removeResource("Beer", beerConsumedEachTick);
                        checkTime = Time.time + beerConsumptionTime + (employment.getWorkerCap() - employment.getNumHealthyWorkers()) * lowEmployeePenaltyNum;

                        //supply entertainment
                        for (int i = 0; i < housingSearchRadius * 2; i++)
                        {
                            for (int j = 0; j < housingSearchRadius * 2; j++)
                            {
                                if (Mathf.RoundToInt(gameObject.transform.position.x) - housingSearchRadius + i >= 0
                                    && Mathf.RoundToInt(gameObject.transform.position.y) - housingSearchRadius + j >= 0
                                    && Mathf.RoundToInt(gameObject.transform.position.x) - housingSearchRadius + i <= 39
                                    && Mathf.RoundToInt(gameObject.transform.position.y) - housingSearchRadius + j <= 39
                                    && structureArr[Mathf.RoundToInt(gameObject.transform.position.x) - housingSearchRadius + i,
                                    Mathf.RoundToInt(gameObject.transform.position.y) - housingSearchRadius + j] != null
                                    && structureArr[Mathf.RoundToInt(gameObject.transform.position.x) - housingSearchRadius + i,
                                    Mathf.RoundToInt(gameObject.transform.position.y) - housingSearchRadius + j].tag == World.HOUSE)
                                {
                                    GameObject house = structureArr[Mathf.RoundToInt(gameObject.transform.position.x) - housingSearchRadius + i,
                                        Mathf.RoundToInt(gameObject.transform.position.y) - housingSearchRadius + j];
                                    house.GetComponent<HouseInformation>().setEntertainmentLevel(1);
                                }
                            }
                        }
                    }
                }
                else if (myStorage.getBeerCount() == 0)
                {
                    consumingBeer = false;
                }
            }
        }
	}

    /**
     * Creates an orc to collect resources for the marketplace to distribute.
     */
    public void createCollectionOrc()
    {
        GameObject world = GameObject.Find(World.WORLD_INFORMATION);
        World myWorld = world.GetComponent<World>();
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
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 1) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 1) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag == World.ROAD)
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 1) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j));
                foundSpawn = true;
            }
            j++;
        }

        GameObject newCollectorOrc = Instantiate(collectorOrc, new Vector2(spawnPosition.x, spawnPosition.y + 0.4f), Quaternion.identity);
        Collect collect = newCollectorOrc.GetComponent<Collect>();
        collect.setResourcesToCollect(requiredResourceName);
        collect.setOriginalLocation(spawnPosition);
        collect.setOrcEmployment(gameObject);
        collect.setCarryingCapacity(collectorCarryCapacity);//TODO: decide on whether or not carrying capacity should be based on num 
        // workers at this employment
    }

    /**
     * Sets the boolean status of whether or not the delivery worker
     * is out for delivery or at the production site.
     * @param status is whether or not the orc is out for delivery
     */
    public void setOrcTransportStatus(bool status)
    {
        orcMovingGoods = status;
    }

    /**
     * Gets the time remaining on the current drinks before a "beer" resource is consumed
     * @return the time remaining on the current drinks
     */
    public int getTimeLeftOnCurrentDrinks()
    {
        return Mathf.RoundToInt(checkTime - Time.time);
    }

    /**
     * Toggles whether the building is active
     */
    public void toggleActive()
    {
        active = !active;
    }
}
