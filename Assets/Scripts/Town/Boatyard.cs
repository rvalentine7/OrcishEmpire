using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates boats from lumber and provides the boats to BoatRequesters
/// </summary>
public class Boatyard : MonoBehaviour
{
    public GameObject collectorOrc;
    public GameObject boat;
    public int collectorCarryCapacity;
    public float timeInterval;
    public string requiredResourceName;
    public string resourceName;
    private int progress;
    private float checkTime;
    private int numWorkers;
    private int workerValue;
    private Employment employment;
    private Storage storage;
    private int requiredResourcesInUse;
    private bool orcMovingGoods;
    private List<BoatRequester> boatRequesters;


    /// <summary>
    /// Initializes the BoatYard
    /// </summary>
    void Start()
    {
        employment = gameObject.GetComponent<Employment>();
        storage = gameObject.GetComponent<Storage>();
        progress = 0;
        checkTime = 0.0f;
        numWorkers = employment.getNumHealthyWorkers();
        workerValue = employment.getWorkerValue();
        requiredResourcesInUse = 0;
        orcMovingGoods = false;
        
        boatRequesters = new List<BoatRequester>();
    }

    /// <summary>
    /// Collects lumber, creates boats, and delivers boats to any buildings waiting on them
    /// </summary>
    void Update()
    {
        orcMovingGoods = employment.getWorkerDeliveringGoods();
        numWorkers = employment.getNumHealthyWorkers();
        //Collect goods
        if (numWorkers > 0 && storage.getResourceCount(requiredResourceName) < storage.getStorageMax() && orcMovingGoods == false)
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
                Vector2 boatSpawnLocation = boatRequesters[i].receiveBoat(gameObject);
                //If the requester is still looking for a boat, deliver this one
                if (boatSpawnLocation.x != -1)
                {
                    spawnBoat(boatRequesters[i], boatSpawnLocation);
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
        if (numWorkers > 0 && Time.time > checkTime && requiredResourcesInUse == collectorCarryCapacity)
        {
            checkTime = Time.time + timeInterval;
            if (progress < 100)
            {
                if (progress + numWorkers * workerValue > 100)
                {
                    progress = 100;
                }
                else
                {
                    progress += numWorkers * workerValue;
                }
            }
        }
    }

    /// <summary>
    /// Used to deliver boats to BoatRequester buildings
    /// </summary>
    /// <param name="boatRequester">A building requesting a boat</param>
    public void deliverBoat(BoatRequester boatRequester)
    {
        //It's your lucky day! We already have a boat waiting to be delivered to your loving hands.
        if (progress == 100)
        {
            Vector2 boatSpawnLocation = boatRequester.receiveBoat(gameObject);
            if (boatSpawnLocation.x != -1)
            {
                spawnBoat(boatRequester, boatSpawnLocation);
                progress = 0;
            }
        }
        //No boat ready, but we'll add you to the list
        else
        {
            boatRequesters.Add(boatRequester);
        }
    }

    /// <summary>
    /// Spawns a boat and tells it where to go
    /// </summary>
    /// <param name="boatRequester">The GameObject requesting the boat</param>
    private void spawnBoat(BoatRequester boatRequester, Vector2 boatSpawnLocation)
    {
        GameObject newBoat = Instantiate(boat, new Vector2(boatSpawnLocation.x, boatSpawnLocation.y + 0.4f), Quaternion.identity);
        //TODO: tell newBoat what to do. (tell it the boatRequester to travel to)
    }
    
    /// <summary>
    /// Creates an orc to collect resources for the marketplace to distribute.
    /// TODO: there should be class this and itemproduction inherit from for this method
    /// </summary>
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
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1)].tag.Equals(World.ROAD))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1));
                foundSpawn = true;
            }
            //checking the row above the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)].tag.Equals(World.ROAD))
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
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag.Equals(World.ROAD))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j));
                foundSpawn = true;
            }
            //checking the column to the right of the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag.Equals(World.ROAD))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
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

    /// <summary>
    /// Gets the progress towards completion out of 100.
    /// TODO: this method is repeated in a lot of classes and is used by popup objects... make a class this can inherit the method from?
    /// </summary>
    /// <returns>progress how far the production is to completion</returns>
    public int getProgressNum()
    {
        return progress;
    }
}
