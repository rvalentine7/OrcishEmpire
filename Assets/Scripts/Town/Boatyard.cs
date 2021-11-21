using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates boats from lumber and provides the boats to BoatRequesters
/// </summary>
public class Boatyard : Building
{
    public GameObject collectorOrc;
    public GameObject boat;
    public int collectorCarryCapacity;
    public float timeToProduce;
    public string requiredResourceName;
    private float progress;
    private float prevUpdateTime;
    private Employment employment;
    private Storage storage;
    private int requiredResourcesInUse;
    private bool orcMovingGoods;
    private List<BoatRequester> boatRequesters;
    private World myWorld;
    int boatyardWidth;
    int boatyardHeight;


    /// <summary>
    /// Initializes the BoatYard
    /// </summary>
    void Start()
    {
        employment = gameObject.GetComponent<Employment>();
        storage = gameObject.GetComponent<Storage>();
        progress = 0.0f;
        prevUpdateTime = 0.0f;
        requiredResourcesInUse = 0;
        orcMovingGoods = false;
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        boatyardWidth = (int)gameObject.GetComponent<BoxCollider2D>().size.x;
        boatyardHeight = (int)gameObject.GetComponent<BoxCollider2D>().size.y;

        boatRequesters = new List<BoatRequester>();
    }

    /// <summary>
    /// Collects lumber, creates boats, and delivers boats to any buildings waiting on them
    /// </summary>
    void Update()
    {
        int numHealthyWorkers = employment.getNumHealthyWorkers();
        if (numHealthyWorkers > 0)
        {
            orcMovingGoods = employment.getWorkerDeliveringGoods();

            //Collect goods
            if (storage.getResourceCount(requiredResourceName) < storage.getStorageMax() && orcMovingGoods == false)
            {
                employment.setWorkerDeliveringGoods(true);
                createCollectionOrc();
            }
            //A boat has been made. Deliver it if there is someone waiting on it
            if (progress == 100 && boatRequesters.Count > 0)
            {
                bool deliveredBoat = false;
                int i = 0;
                while (!deliveredBoat && i < boatRequesters.Count)
                {
                    List<int> waterSectionNumbers = boatRequesters[i].getNearbyWaterSections();
                    Vector2 connectionLocation = findBoatSpawnLocation(waterSectionNumbers);
                    //If the requester is still looking for a boat, deliver this one
                    if (connectionLocation.x != -1 && boatRequesters[i].canReceiveBoat())
                    {
                        spawnBoat(boatRequesters[i], connectionLocation);
                        deliveredBoat = true;
                        progress = 0;
                        boatRequesters.RemoveAt(i);
                    }
                    i++;
                }

            }
            //Starts using the required raw resource
            if (requiredResourcesInUse == 0 && storage.getResourceCount(requiredResourceName) == storage.getStorageMax())
            {
                requiredResourcesInUse = storage.getStorageMax();
                storage.removeResource(requiredResourceName, collectorCarryCapacity);
            }
            //Work on producing goods
            if (requiredResourcesInUse == collectorCarryCapacity)
            {
                if (progress < 100)
                {
                    float progressedTime = Time.unscaledTime - prevUpdateTime;
                    float effectiveTimeToFinish = timeToProduce / (employment.getNumWorkers() / numHealthyWorkers);
                    progress += progressedTime / effectiveTimeToFinish * 100;
                    if (progress >= 100)
                    {
                        progress = 100;
                    }
                }
            }
        }

        prevUpdateTime = Time.unscaledTime;
    }

    /// <summary>
    /// Used to deliver boats to BoatRequester buildings
    /// </summary>
    /// <param name="boatRequester">A building requesting a boat</param>
    /// <param name="waterSectionNumbers">Water sections we're trying to see if this building can spawn a boat in</param>
    public void deliverBoat(BoatRequester boatRequester, List<int> waterSectionNumbers)
    {
        if (!boatRequesters.Contains(boatRequester))
        {
            Vector2 connectionLocation = findBoatSpawnLocation(waterSectionNumbers);

            if (connectionLocation.x != -1)
            {
                //It's your lucky day! We already have a boat waiting to be delivered to your loving hands.
                if (progress == 100)
                {
                    spawnBoat(boatRequester, connectionLocation);
                    progress = 0;
                }
                //No boat ready, but we'll add you to the list
                else
                {
                    boatRequesters.Add(boatRequester);
                }
            }
        }
    }

    /// <summary>
    /// Finds a spawn location for a boat given potential water section numbers
    /// </summary>
    /// <param name="waterSectionNumbers">Water sections we're trying to see if this building can spawn a boat in</param>
    /// <returns>A water section a boat can be spawned in or (-1, -1) if there is no location</returns>
    private Vector2 findBoatSpawnLocation(List<int> waterSectionNumbers)
    {
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();

        Vector2 connectionLocation = new Vector2(-1, -1);
        Vector2 employmentPos = gameObject.transform.position;
        int i = 0;
        while (connectionLocation.x == -1 && i < boatyardWidth)
        {
            //checking the row below the gameObject
            if (connectionLocation.x == -1 && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) - 1)] != null
                && myWorld.wateryTerrain.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) - 1)].tag)
                && waterSectionNumbers.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) - 1)].GetComponent<WaterTile>().getWaterSectionNum()))
            {
                connectionLocation = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                    (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) - 1));
            }
            //checking the row above the gameObject
            else if (connectionLocation.x == -1 && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + 1)] != null
                && myWorld.wateryTerrain.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + 1)].tag)
                && waterSectionNumbers.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + 1)].GetComponent<WaterTile>().getWaterSectionNum()))
            {
                connectionLocation = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                    (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + 1));
            }
            i++;
        }
        int j = 0;
        while (connectionLocation.x == -1 && j < boatyardHeight)
        {
            //checking the column to the left of the gameObject
            if (connectionLocation.x == -1 && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j)] != null
                && myWorld.wateryTerrain.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j)].tag)
                && waterSectionNumbers.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j)].GetComponent<WaterTile>().getWaterSectionNum()))
            {
                connectionLocation = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) - 1),
                    (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j));
            }
            //checking the column to the right of the gameObject
            else if (connectionLocation.x == -1 && terrainArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(boatyardWidth / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j)] != null
                && myWorld.wateryTerrain.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(boatyardWidth / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j)].tag)
                && waterSectionNumbers.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(boatyardWidth / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j)].GetComponent<WaterTile>().getWaterSectionNum()))
            {
                connectionLocation = new Vector2((Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(boatyardWidth / 2.0f - 0.5f) + 1),
                    (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j));
            }
            j++;
        }

        return connectionLocation;
    }

    /// <summary>
    /// Spawns a boat and tells it where to go
    /// </summary>
    /// <param name="boatRequester">The GameObject requesting the boat</param>
    /// <param name="boatSpawnLocation">The location to spawn the boat</param>
    private void spawnBoat(BoatRequester boatRequester, Vector2 boatSpawnLocation)
    {
        requiredResourcesInUse = 0;
        GameObject newBoat = Instantiate(boat, new Vector2(boatSpawnLocation.x, boatSpawnLocation.y + 0.4f), Quaternion.identity);
        StandardBoat newStandardBoat = newBoat.GetComponent<StandardBoat>();
        newStandardBoat.initialize(boatRequester.getGameObject());
    }
    
    /// <summary>
    /// Creates an orc to collect resources for the marketplace to distribute.
    /// TODO: there should be class this and itemproduction inherit from for this method
    /// </summary>
    public void createCollectionOrc()
    {
        GameObject[,] structArr = myWorld.constructNetwork.getConstructArr();
        //checking areas around the farm to place an orc on a road
        Vector2 employmentPos = gameObject.transform.position;
        bool foundSpawn = false;
        Vector2 spawnPosition = new Vector2();
        int i = 0;
        while (!foundSpawn && i < boatyardWidth)
        {
            //checking the row below the gameObject
            if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) - 1)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) - 1)].tag.Equals(World.ROAD))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) - 1));
                foundSpawn = true;
            }
            //checking the row above the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + 1)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + 1)].tag.Equals(World.ROAD))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + 1));
                foundSpawn = true;
            }
            i++;
        }
        int j = 0;
        while (!foundSpawn && j < boatyardHeight)
        {
            //checking the column to the left of the gameObject
            if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j)].tag.Equals(World.ROAD))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(boatyardWidth / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j));
                foundSpawn = true;
            }
            //checking the column to the right of the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(boatyardWidth / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(boatyardWidth / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j)].tag.Equals(World.ROAD))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(boatyardWidth / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(boatyardHeight / 2.0f - 1) + j));
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

    /// <summary>
    /// Gets the progress towards completion out of 100.
    /// TODO: this method is repeated in a lot of classes and is used by popup objects... make a class this can inherit the method from?
    /// </summary>
    /// <returns>progress how far the production is to completion</returns>
    public int getProgressNum()
    {
        return Mathf.FloorToInt(progress);
    }
}
