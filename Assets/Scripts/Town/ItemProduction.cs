using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Item productions creates goods from a production type building using raw resources (weaponsmith, furniture workshop, etc.) and then
/// creates a delivery worker to deliver the goods upon completion of the goods.
/// </summary>
public class ItemProduction : Building
{
    public GameObject collectorOrc;
    public int collectorCarryCapacity;
    public GameObject deliveryOrc;
    public float timeToProduce;
    public string requiredResourceName;
    public string resourceName;
    public int resourceProduced;
    private float progress;
    private float prevUpdateTime;
    private Employment employment;
    private Storage storage;
    private int requiredResourcesInUse;
    private bool orcMovingGoods;

    /// <summary>
    /// Initializes the production site.
    /// </summary>
    void Start()
    {
        employment = gameObject.GetComponent<Employment>();
        storage = gameObject.GetComponent<Storage>();
        progress = 0;
        prevUpdateTime = 0.0f;
        requiredResourcesInUse = 0;
        orcMovingGoods = false;
    }

    /// <summary>
    /// Updates progress on the production and sends out a worker to deliver
    /// the goods upon progress completion.
    /// </summary>
    void Update()
    {
        orcMovingGoods = employment.getWorkerDeliveringGoods();
        int numHealthyWorkers = employment.getNumHealthyWorkers();
        //Deliver Goods
        if (progress == 100 && orcMovingGoods == false)
        {
            progress = 0;
            requiredResourcesInUse = 0;
            employment.setWorkerDeliveringGoods(true);
            orcMovingGoods = employment.getWorkerDeliveringGoods();
            createDeliveryOrc();
        }
        //Collect goods
        if (numHealthyWorkers > 0 && storage.getResourceCount(requiredResourceName) + collectorCarryCapacity <= storage.getStorageMax() && orcMovingGoods == false)
        {
            employment.setWorkerDeliveringGoods(true);
            createCollectionOrc();
        }
        //Starts using the required raw resource
        if (requiredResourcesInUse == 0 && storage.getResourceCount(requiredResourceName) >= collectorCarryCapacity)
        {
            requiredResourcesInUse = storage.getStorageMax();
            storage.removeResource(requiredResourceName, collectorCarryCapacity);
        }
        //Work on producing goods
        if (numHealthyWorkers > 0 && requiredResourcesInUse == collectorCarryCapacity)
        {
            if (progress < 100)
            {
                float progressedTime = Time.time - prevUpdateTime;
                float effectiveTimeToFinish = timeToProduce * (employment.getWorkerCap() / numHealthyWorkers);
                progress += progressedTime / effectiveTimeToFinish * 100;
                if (progress >= 100)
                {
                    progress = 100;
                }
            }
        }

        prevUpdateTime = Time.time;
    }

    /// <summary>
    /// Creates an orc to carry resources from the production site to a storage location.
    /// This building favors placing an orc at the first available road segment
    /// it finds in the order of: bottom, top, left, right
    /// </summary>
    private void createDeliveryOrc()
    {
        World myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
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

        GameObject newDeliveryOrc = Instantiate(deliveryOrc, new Vector2(spawnPosition.x, spawnPosition.y + 0.4f), Quaternion.identity);
        Deliver deliver = newDeliveryOrc.GetComponent<Deliver>();
        deliver.addResources(resourceName, resourceProduced);
        deliver.setOriginalLocation(spawnPosition);
        deliver.setOrcEmployment(gameObject);
    }

    /// <summary>
    /// Creates an orc to collect resources for the marketplace to distribute.
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
    /// Sets the boolean status of whether or not the delivery worker
    /// is out for delivery or at the production site.
    /// </summary>
    /// <param name="status">whether or not the orc is out for delivery</param>
    public void setOrcTransportStatus(bool status)
    {
        orcMovingGoods = status;
    }

    /// <summary>
    /// Gets the progress towards completion out of 100.
    /// </summary>
    /// <returns>how close the production is to completion</returns>
    public int getProgressNum()
    {
        return Mathf.FloorToInt(progress);
    }
}
