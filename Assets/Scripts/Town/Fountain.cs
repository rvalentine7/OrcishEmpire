using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fountain : MonoBehaviour {
    public int timeInterval;
    public int waterRadius;
    public int waterPerTick;
    public Sprite filledFountain;
    public Sprite emptyFountain;

    private GameObject world;
    private World myWorld;
    private GameObject[,] terrainArr;
    private bool filled;

    private void Awake()
    {
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
    }

    // Use this for initialization
    void Start () {
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        if (terrainArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].GetComponent<Tile>().hasPipes())
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = filledFountain;
            gameObject.GetComponent<Employment>().addWaterSource();
        }
        else
        {
            filled = false;
            gameObject.GetComponent<SpriteRenderer>().sprite = emptyFountain;
        }
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    /**
     * Updates whether the fountain is filled
     * @param filled whether the fountain is filled
     */
    public void updateFilled(bool filled)
    {
        this.filled = filled;
        if (this.filled)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = filledFountain;
            updateWaterSupplying(true);
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = emptyFountain;
            updateWaterSupplying(false);
        }
    }

    /**
     * Updates whether this fountain is supplying water to nearby buildings
     * @param supplying whether the fountain is supplying water to nearby buildings
     */
    public void updateWaterSupplying(bool supplying)
    {
        GameObject[,] constructArr = myWorld.constructNetwork.getConstructArr();
        Vector2 fountainPosition = gameObject.transform.position;
        //search for nearby houses and supply them with water
        for (int i = 0; i <= waterRadius * 2; i++)
        {
            for (int j = 0; j <= waterRadius * 2; j++)
            {
                if (fountainPosition.x - waterRadius + i >= 0 && fountainPosition.y - waterRadius + j >= 0
                            && fountainPosition.x - waterRadius + i <= 39 && fountainPosition.y - waterRadius + j <= 39
                            && gameObject.transform.position.x != (int)fountainPosition.x - waterRadius + i//avoid adding/removing water to/from itself
                            && gameObject.transform.position.y != (int)fountainPosition.y - waterRadius + j
                            && constructArr[(int)fountainPosition.x - waterRadius + i, (int)fountainPosition.y - waterRadius + j] != null
                            && constructArr[(int)fountainPosition.x - waterRadius + i, (int)fountainPosition.y - waterRadius + j].tag == "Building"
                            && constructArr[(int)fountainPosition.x - waterRadius + i, (int)fountainPosition.y - waterRadius + j].GetComponent<Fountain>() == null
                            && constructArr[(int)fountainPosition.x - waterRadius + i, (int)fountainPosition.y - waterRadius + j].GetComponent<Reservoir>() == null)
                {
                    Employment employment = constructArr[(int)fountainPosition.x - waterRadius + i,
                        (int)fountainPosition.y - waterRadius + j].GetComponent<Employment>();
                    if (supplying)
                    {
                        employment.addWaterSource();
                    }
                    else
                    {
                        employment.removeWaterSource();
                    }
                }
                if (fountainPosition.x - waterRadius + i >= 0 && fountainPosition.y - waterRadius + j >= 0
                        && fountainPosition.x - waterRadius + i <= 40 && fountainPosition.y - waterRadius + j <= 40
                        && constructArr[(int)fountainPosition.x - waterRadius + i, (int)fountainPosition.y - waterRadius + j] != null
                        && constructArr[(int)fountainPosition.x - waterRadius + i, (int)fountainPosition.y - waterRadius + j].tag == "House")
                {
                    HouseInformation houseInformation = constructArr[(int)fountainPosition.x - waterRadius + i,
                        (int)fountainPosition.y - waterRadius + j].GetComponent<HouseInformation>();
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
     * Gets whether the fountain is filled
     * @return filled whether the fountain is filled
     */
    public bool getFilled()
    {
        return filled;
    }
}
