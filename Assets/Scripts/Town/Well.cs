using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Provides water to nearby houses.
 */
public class Well : MonoBehaviour {
    private GameObject[,] constructArr;
    private float checkTime;
    public float timeInterval;
    public int waterRadius;
    public int waterPerTick;
    public GameObject wellPopupObject;

    /**
     * Initializes the Well.
     */
    void Start () {
        checkTime = 0.0f;
    }
	
	/**
     * Gives water to nearby houses at a given time interval.
     */
	void Update () {
        if (Time.time > checkTime)
        {
            checkTime = Time.time + timeInterval;
            GameObject world = GameObject.Find("WorldInformation");
            World myWorld = world.GetComponent<World>();
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
                        HouseInformation houseInfo = constructArr[(int)wellPosition.x - waterRadius + i,
                            (int)wellPosition.y - waterRadius + j].GetComponent<HouseInformation>();
                        houseInfo.addWater(waterPerTick);
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
        if (GameObject.FindWithTag("Popup") == null)
        {
            Instantiate(wellPopupObject);
        }
    }
}
