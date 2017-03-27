using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Pig Farms raise pigs to produce meat which orc citizens use for food.
 */
public class PigFarm : MonoBehaviour {
    public GameObject deliveryOrc;
    public float timeInterval;
    public int meatProduced;
    private int progress;
    private float checkTime;
    private int numWorkers;
    private int workerValue;
    private Employment employment;
    private bool orcOutForDelivery;

    /**
     * Initializes the pig farm.
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
     * Updates progress on the meat production and sends out a worker to deliver
     *  the meat upon progress completion.
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
     * Click the object to see information about it.
     */
    void OnMouseDown()
    {
        Debug.Log("Number of workers: " + numWorkers + "/" + employment.getWorkerCap());
        Debug.Log("Production Progress: " + progress);
    }

    /**
     * Creates an orc to carry meat from the pig farm to a storage location.
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
        Delivery delivery = newDeliveryOrc.GetComponent<Delivery>();
        delivery.addResources("Meat", meatProduced);
        delivery.setOriginalLocation(spawnPosition);
        delivery.setOrcEmployment(gameObject);
    }

    /**
     * Sets the boolean status of whether or not the delivery worker
     * is out for delivery or at the pig farm.
     * @param status is whether or not the orc is out for delivery
     */
    public void setDeliveryStatus(bool status)
    {
        orcOutForDelivery = status;
    }
}
