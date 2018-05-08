using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Production creates goods from a raw resource production type building (farm, lumbermill, iron mine, etc.) and then
 * creates a delivery worker to deliver the goods upon completion of the goods.
 */
public class Production : MonoBehaviour {
    public GameObject deliveryOrc;
    public float timeInterval;
    public string resourceName;
    public int resourceProduced;
    private int progress;
    private float checkTime;
    private int numWorkers;
    private int workerValue;
    private Employment employment;
    private bool orcOutForDelivery;

    /**
     * Initializes the production site.
     */
    void Start () {
        employment = gameObject.GetComponent<Employment>();
        progress = 0;
        checkTime = 0.0f;
        numWorkers = employment.getNumWorkers();
        workerValue = employment.getWorkerValue();
        orcOutForDelivery = false;
    }
	
	/**
     * Updates progress on the production and sends out a worker to deliver
     *  the goods upon progress completion.
     */
	void Update () {
        orcOutForDelivery = employment.getWorkerDeliveringGoods();
        numWorkers = employment.getNumWorkers();
		if (numWorkers > 0 && Time.time > checkTime)
        {
            checkTime = Time.time + timeInterval;
            if (progress < 100)
            {
                if (progress + numWorkers * workerValue > 100)
                {
                    progress = 100;
                } else
                {
                    progress += numWorkers * workerValue;
                }
            }
        }
        if (progress == 100 && orcOutForDelivery == false)
        {
            progress = 0;
            orcOutForDelivery = true;
            employment.setWorkerDeliveringGoods(true);
            createDeliveryOrc();
        }
	}

    /**
     * Creates an orc to carry resources from the production site to a storage location.
     * This building favors placing an orc at the first available road segment
     *  it finds in the order of: bottom, top, left, right
     */
    private void createDeliveryOrc()
    {
        GameObject world = GameObject.Find("WorldInformation");
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
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 1) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 1) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag == "Road")
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 1) + 1),
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

    /**
     * Sets the boolean status of whether or not the delivery worker
     * is out for delivery or at the production site.
     * @param status is whether or not the orc is out for delivery
     */
    public void setDeliveryStatus(bool status)
    {
        orcOutForDelivery = status;
    }

    /**
     * Gets the progress towards completion out of 100.
     * @return progress how far the production is to completion
     */
    public int getProgressNum()
    {
        return progress;
    }
}
