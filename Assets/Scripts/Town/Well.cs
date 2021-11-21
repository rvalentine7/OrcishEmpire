using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Provides water to nearby houses.
/// </summary>
public class Well : Building {
    private GameObject[,] constructArr;
    private World myWorld;
    public float timeInterval;
    public int waterRadius;
    public int waterPerTick;

    /// <summary>
    /// Initializes the Well.
    /// </summary>
    void Start () {
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        updateWaterSupplying(true);
    }

    /// <summary>
    /// Gives water to nearby houses at a given time interval.
    /// </summary>
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

    /// <summary>
    /// Updates whether this well is supplying water to nearby buildings
    /// </summary>
    /// <param name="supplying">whether the well is supplying water to nearby buildings</param>
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
                            && constructArr[(int)wellPosition.x - waterRadius + i, (int)wellPosition.y - waterRadius + j].tag.Equals(World.BUILDING)
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
                        && constructArr[(int)wellPosition.x - waterRadius + i, (int)wellPosition.y - waterRadius + j].tag.Equals(World.HOUSE))
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
                //add water source to tiles so that new houses know there is water there
                if (wellPosition.x - waterRadius + i >= 0 && wellPosition.y - waterRadius + j >= 0
                        && wellPosition.x - waterRadius + i <= 40 && wellPosition.y - waterRadius + j <= 40)
                {
                    GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
                    if (supplying)
                    {
                        terrainArr[(int)wellPosition.x - waterRadius + i, (int)wellPosition.y - waterRadius + j].GetComponent<Tile>().addWaterSource();
                    }
                    else
                    {
                        terrainArr[(int)wellPosition.x - waterRadius + i, (int)wellPosition.y - waterRadius + j].GetComponent<Tile>().removeWaterSource();
                    }
                }
            }
        }
    }
}
