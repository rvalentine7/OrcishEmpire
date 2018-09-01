using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Provides water to nearby houses.
 */
public class Well : MonoBehaviour {
    private GameObject[,] constructArr;
    private GameObject world;
    private World myWorld;
    public float timeInterval;
    public int waterRadius;
    public int waterPerTick;
    public GameObject wellPopupObject;

    /**
     * Initializes the Well.
     */
    void Start () {
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        updateWaterSupplying(true);
    }
	
	/**
     * Gives water to nearby houses at a given time interval.
     */
	void Update () {
        /*if (Time.time > checkTime)
        {
            checkTime = Time.time + timeInterval;
            constructArr = myWorld.constructNetwork.getConstructArr();
            Vector2 wellPosition = gameObject.transform.position;
            //search for nearby houses and supply them with water
            for (int i = 0; i <= waterRadius * 2; i++)
            {
                for (int j = 0; j <= waterRadius * 2; j++)
                {
                    if (wellPosition.x - waterRadius + i >= 0 && wellPosition.y - waterRadius + j >= 0
                        && wellPosition.x - waterRadius + i <= 39 && wellPosition.y - waterRadius + j <= 39
                        && constructArr[(int)wellPosition.x - waterRadius + i, (int)wellPosition.y - waterRadius + j] != null
                        && constructArr[(int)wellPosition.x - waterRadius + i, (int)wellPosition.y - waterRadius + j].tag == "House")
                    {
                        //HouseInformation houseInfo = constructArr[(int)wellPosition.x - waterRadius + i,
                        //    (int)wellPosition.y - waterRadius + j].GetComponent<HouseInformation>();
                        Storage storage = constructArr[(int)wellPosition.x - waterRadius + i,
                            (int)wellPosition.y - waterRadius + j].GetComponent<Storage>();
                        if (storage.acceptsResource("Water", waterPerTick))
                        {
                            storage.addResource("Water", waterPerTick);
                        }
                        //houseInfo.addWater(waterPerTick);
                    }
                }
            }
        }*/
	}

    /**
     * Updates whether this well is supplying water to nearby buildings
     * @param supplying whether the well is supplying water to nearby buildings
     */
    public void updateWaterSupplying(bool supplying)
    {
        constructArr = myWorld.constructNetwork.getConstructArr();
        Vector2 wellPosition = gameObject.transform.position;
        //search for nearby houses and supply them with water
        for (int i = 0; i <= waterRadius * 2; i++)
        {
            for (int j = 0; j <= waterRadius * 2; j++)
            {
                if (wellPosition.x - waterRadius + i >= 0 && wellPosition.y - waterRadius + j >= 0
                            && wellPosition.x - waterRadius + i <= 39 && wellPosition.y - waterRadius + j <= 39
                            && gameObject.transform.position.x != (int)wellPosition.x - waterRadius + i//avoid adding/removing water to/from itself
                            && gameObject.transform.position.y != (int)wellPosition.y - waterRadius + j
                            && constructArr[(int)wellPosition.x - waterRadius + i, (int)wellPosition.y - waterRadius + j] != null
                            && constructArr[(int)wellPosition.x - waterRadius + i, (int)wellPosition.y - waterRadius + j].tag == "Building"
                            && constructArr[(int)wellPosition.x - waterRadius + i, (int)wellPosition.y - waterRadius + j].GetComponent<Fountain>() == null
                            && constructArr[(int)wellPosition.x - waterRadius + i, (int)wellPosition.y - waterRadius + j].GetComponent<Reservoir>() == null)
                {
                    Employment employment = constructArr[(int)wellPosition.x - waterRadius + i,
                        (int)wellPosition.y - waterRadius + j].GetComponent<Employment>();
                    if (supplying)
                    {
                        employment.addWaterSource();
                    }
                    else
                    {
                        employment.removeWaterSource();
                    }
                }
                if (wellPosition.x - waterRadius + i >= 0 && wellPosition.y - waterRadius + j >= 0
                        && wellPosition.x - waterRadius + i <= 40 && wellPosition.y - waterRadius + j <= 40
                        && constructArr[(int)wellPosition.x - waterRadius + i, (int)wellPosition.y - waterRadius + j] != null
                        && constructArr[(int)wellPosition.x - waterRadius + i, (int)wellPosition.y - waterRadius + j].tag == "House")
                {
                    HouseInformation houseInformation = constructArr[(int)wellPosition.x - waterRadius + i,
                        (int)wellPosition.y - waterRadius + j].GetComponent<HouseInformation>();
                    if (supplying)
                    {
                        houseInformation.addWaterSource();
                    }
                    else
                    {
                        houseInformation.removeWaterSource();
                    }
                }
            }
        }
    }

    /**
     * Click the object to see information about it
     */
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameObject.FindWithTag("Popup") == null)
        {
            Instantiate(wellPopupObject);
        }
    }
}
