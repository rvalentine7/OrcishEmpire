using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Production creates goods from a raw resource production type building (farm, lumbermill, iron mine, etc.) and then
/// creates a delivery worker to deliver the goods upon completion of the goods.
/// </summary>
public class Production : Building {
    public GameObject deliveryOrc;
    public float timeToProduce;
    public string resourceName;
    public int resourceProduced;
    private World myWorld;
    private float progress;
    private float prevUpdateTime;
    private int numHealthyWorkers;
    private Employment employment;
    private bool orcOutForDelivery;
    //private bool nearTrees;//TODO: use nearTrees to update the status of the building in the UI popup for lumbermills
    private bool active;

    /// <summary>
    /// Initializes the production site.
    /// </summary>
    void Start () {
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        employment = gameObject.GetComponent<Employment>();
        progress = 0.0f;
        prevUpdateTime = 0.0f;
        numHealthyWorkers = employment.getNumHealthyWorkers();
        orcOutForDelivery = false;
        //nearTrees = true;
        active = true;
    }

    /// <summary>
    /// Updates progress on the production and sends out a worker to deliver
    /// the goods upon progress completion.
    /// </summary>
    void Update()
    {
        orcOutForDelivery = employment.getWorkerDeliveringGoods();
        numHealthyWorkers = employment.getNumHealthyWorkers();
        if (gameObject.name.Contains(World.LUMBER) && active)
        {
            //Lumber mills need to be near trees in order to function
            //nearTrees = true;

            GameObject world = GameObject.Find(World.WORLD_INFORMATION);
            World myWorld = world.GetComponent<World>();
            GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
            Vector2 millPos = gameObject.transform.position;
            if (!terrainArr[(int)(millPos.x - Mathf.CeilToInt(2 / 2.0f - 1)),
                (int)(millPos.y - Mathf.FloorToInt(2 / 2))].tag.Equals(World.TREES)//below bottom left corner
                && !terrainArr[(int)(millPos.x - Mathf.CeilToInt(2 / 2.0f - 1)),
                (int)(millPos.y + Mathf.FloorToInt(2 / 2) + 1)].tag.Equals(World.TREES)//above top left corner
                && !terrainArr[(int)(millPos.x + Mathf.FloorToInt(2 / 2)),
                (int)(millPos.y - Mathf.FloorToInt(2 / 2))].tag.Equals(World.TREES)//below bottom right corner
                && !terrainArr[(int)(millPos.x + Mathf.FloorToInt(2 / 2)),
                (int)(millPos.y + Mathf.FloorToInt(2 / 2) + 1)].tag.Equals(World.TREES)//above top right corner
                && !terrainArr[(int)(millPos.x + Mathf.FloorToInt(2 / 2) + 1),
                (int)(millPos.y - Mathf.FloorToInt(2 / 2) + 1)].tag.Equals(World.TREES)//to the right of bottom right corner
                && !terrainArr[(int)(millPos.x + Mathf.FloorToInt(2 / 2) + 1),
                (int)(millPos.y + Mathf.FloorToInt(2 / 2))].tag.Equals(World.TREES)//to the right of the top right corner
                && !terrainArr[(int)(millPos.x - Mathf.CeilToInt(2 / 2.0f - 1) - 1),
                (int)(millPos.y - Mathf.FloorToInt(2 / 2) + 1)].tag.Equals(World.TREES)//to the left of the bottom left corner
                && !terrainArr[(int)(millPos.x - Mathf.CeilToInt(2 / 2.0f - 1) - 1),
                (int)(millPos.y + Mathf.FloorToInt(2 / 2))].tag.Equals(World.TREES))//to the left of the top left corner
            {
                //nearTrees = false;
                active = false;
            }
        }
        if (numHealthyWorkers > 0 && active && progress < 100)
        {
            float progressedTime = Time.time - prevUpdateTime;
            float effectiveTimeToFinish = timeToProduce * (employment.getWorkerCap() / numHealthyWorkers);
            progress += progressedTime / effectiveTimeToFinish * 100;
            if (progress >= 100)
            {
                progress = 100;
            }
        }
        if (progress == 100 && orcOutForDelivery == false)
        {
            progress = 0;
            orcOutForDelivery = true;
            employment.setWorkerDeliveringGoods(true);
            createDeliveryOrc();
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
    /// Sets the boolean status of whether or not the delivery worker
    /// is out for delivery or at the production site.
    /// </summary>
    /// <param name="status">whether or not the orc is out for delivery</param>
    public void setDeliveryStatus(bool status)
    {
        orcOutForDelivery = status;
    }

    /// <summary>
    /// Gets the progress towards completion out of 100.
    /// </summary>
    /// <returns>how far the production is to completion</returns>
    public int getProgressNum()
    {
        return Mathf.FloorToInt(progress);
    }
}
