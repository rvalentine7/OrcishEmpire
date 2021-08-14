using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingWharf : BoatRequester
{
    private World myWorld;
    private StandardBoat standardBoat;//TODO: make standardboat and fishing boat inherit Boat
    private FishingBoat fishingBoat;
    private bool hasBoat;

    public int numFishingTripsBeforeBoatBreaks;
    public int searchRadius;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        hasBoat = false;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        //TODO: When I look for boatyards, check the water section number.  If the boatyard doesn't have a boat already waiting, give the boatyard the list
        //      When the boatyard checks if the 

        /*
         Look for Boatyards
         Ask if they share a water section
         If they do, request a boat.
         If the boat has to cancel (a low bridge was constructed and blocks the path), update() should once again look for viable boatyards
         */

        //Search for a boat
        if (!hasBoat)
        {
            StartCoroutine(findStandardBoat());
        }
        
    }

    /// <summary>
    /// Finds a boat from a Boatyard
    /// </summary>
    /// <returns>A time delay to split this search over multiple frames</returns>
    private IEnumerator findStandardBoat()
    {
        //checks for the closest warehouse
        List<GameObject> discoveredDeliverLocs = new List<GameObject>();
        Vector2 fishingWharfLocation = gameObject.transform.position;
        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        for (int i = 0; i <= searchRadius * 2; i++)
        {
            for (int j = 0; j <= searchRadius * 2; j++)
            {
                if (fishingWharfLocation.x - searchRadius + i >= 0 && fishingWharfLocation.y - searchRadius + j >= 0
                        && fishingWharfLocation.x - searchRadius + i <= 39 && fishingWharfLocation.y - searchRadius + j <= 39
                        && structureArr[(int)fishingWharfLocation.x - searchRadius + i,
                        (int)fishingWharfLocation.y - searchRadius + j] != null
                        && structureArr[(int)fishingWharfLocation.x - searchRadius + i,
                        (int)fishingWharfLocation.y - searchRadius + j].tag.Equals(World.BUILDING)
                        && !discoveredDeliverLocs.Contains(structureArr[(int)fishingWharfLocation.x - searchRadius + i,
                        (int)fishingWharfLocation.y - searchRadius + j])
                        && structureArr[(int)fishingWharfLocation.x - searchRadius + i,
                        (int)fishingWharfLocation.y - searchRadius + j].GetComponent<Employment>().getNumWorkers() > 0
                        && structureArr[(int)fishingWharfLocation.x - searchRadius + i,
                        (int)fishingWharfLocation.y - searchRadius + j].GetComponent<Boatyard>() != null)
                {
                    structureArr[(int)fishingWharfLocation.x - searchRadius + i,
                        (int)fishingWharfLocation.y - searchRadius + j].GetComponent<Boatyard>().deliverBoat(this, getNearbyWaterSections());
                }
            }
            yield return 0.1f;
        }
        yield break;
    }

    /// <summary>
    /// Gets the water sections next to the building
    /// </summary>
    /// <returns>The water sections next to this building</returns>
    public override List<int> getNearbyWaterSections()
    {
        List<int> nearbyWaterSections = new List<int>();

        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
        int buildingWidth = (int)gameObject.GetComponent<BoxCollider2D>().size.x;
        int buildingHeight = (int)gameObject.GetComponent<BoxCollider2D>().size.y;

        Vector2 connectionLocation = new Vector2(-1, -1);
        Vector2 employmentPos = gameObject.transform.position;
        int i = 0;
        while (connectionLocation.x == -1 && i < buildingWidth)
        {
            //checking the row below the gameObject
            if (connectionLocation.x == -1 && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) - 1)] != null
                && myWorld.wateryTerrain.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) - 1)].tag))
            {
                nearbyWaterSections.Add(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                    (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) - 1)].GetComponent<WaterTile>().getWaterSectionNum());
            }
            //checking the row above the gameObject
            else if (connectionLocation.x == -1 && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(buildingHeight / 2.0f - 1) + 1)] != null
                && myWorld.wateryTerrain.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(buildingHeight / 2.0f - 1) + 1)].tag))
            {
                nearbyWaterSections.Add(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                    (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(buildingHeight / 2.0f - 1) + 1)].GetComponent<WaterTile>().getWaterSectionNum());
            }
            i++;
        }
        int j = 0;
        while (connectionLocation.x == -1 && j < buildingHeight)
        {
            //checking the column to the left of the gameObject
            if (connectionLocation.x == -1 && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j)] != null
                && myWorld.wateryTerrain.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j)].tag))
            {
                nearbyWaterSections.Add(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) - 1),
                    (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j)].GetComponent<WaterTile>().getWaterSectionNum());
            }
            //checking the column to the right of the gameObject
            else if (connectionLocation.x == -1 && terrainArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(buildingWidth / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j)] != null
                && myWorld.wateryTerrain.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(buildingWidth / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j)].tag))
            {
                nearbyWaterSections.Add(terrainArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(buildingWidth / 2.0f - 0.5f) + 1),
                    (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j)].GetComponent<WaterTile>().getWaterSectionNum());
            }
            j++;
        }

        return nearbyWaterSections;
    }

    /// <summary>
    /// Returns whether this building can receive a new boat
    /// </summary>
    /// <returns>Whether this building already has a boat</returns>
    public override bool canReceiveBoat()
    {
        return !hasBoat;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="boat"></param>
    public override void receiveBoat(StandardBoat boat)
    {
        standardBoat = boat;
        hasBoat = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void standardBoatArrived()
    {
        //TODO: convert to fishing boat
        standardBoat = null;
        Debug.Log("Standard boat arrived!");
    }

    /// <summary>
    /// 
    /// </summary>
    public override void cancelStandardBoat()
    {
        standardBoat = null;
        hasBoat = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="waterSection"></param>
    /// <returns></returns>
    public override Vector2 receivingLocation(int waterSection)
    {
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
        int buildingWidth = (int)gameObject.GetComponent<BoxCollider2D>().size.x;
        int buildingHeight = (int)gameObject.GetComponent<BoxCollider2D>().size.y;

        Vector2 employmentPos = gameObject.transform.position;
        int i = 0;
        while (i < buildingWidth)
        {
            //checking the row below the gameObject
            if (terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) - 1)] != null
                && myWorld.wateryTerrain.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) - 1)].tag)
                && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) - 1)].GetComponent<WaterTile>().getWaterSectionNum() == waterSection)
            {
                return new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                    (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) - 1));
            }
            //checking the row above the gameObject
            else if (terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(buildingHeight / 2.0f - 1) + 1)] != null
                && myWorld.wateryTerrain.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(buildingHeight / 2.0f - 1) + 1)].tag)
                && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(buildingHeight / 2.0f - 1) + 1)].GetComponent<WaterTile>().getWaterSectionNum() == waterSection)
            {
                return new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) + i),
                    (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(buildingHeight / 2.0f - 1) + 1));
            }
            i++;
        }
        int j = 0;
        while (j < buildingHeight)
        {
            //checking the column to the left of the gameObject
            if (terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j)] != null
                && myWorld.wateryTerrain.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j)].tag)
                && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j)].GetComponent<WaterTile>().getWaterSectionNum() == waterSection)
            {
                return new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(buildingWidth / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j));
            }
            //checking the column to the right of the gameObject
            else if (terrainArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(buildingWidth / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j)] != null
                && myWorld.wateryTerrain.Contains(terrainArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(buildingWidth / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j)].tag)
                && terrainArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(buildingWidth / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j)].GetComponent<WaterTile>().getWaterSectionNum() == waterSection)
            {
                return new Vector2((Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(buildingWidth / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(buildingHeight / 2.0f - 1) + j));
            }
            j++;
        }
        return new Vector2(-1, -1);
    }

    /// <summary>
    /// 
    /// </summary>
    public void removeBoat()
    {
        standardBoat = null;
        //TODO: fishingboat should be null, too
        hasBoat = false;
    }

    /// <summary>
    /// Finds a valid position to spawn a boat
    /// </summary>
    /// <returns>A valid position to spawn a boat</returns>
    private Vector2 findBoatSpawnPosition()//TODO: this will need to consider water section numbers
    {
        GameObject world = GameObject.Find(World.WORLD_INFORMATION);
        World myWorld = world.GetComponent<World>();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
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
            if (!foundSpawn && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1)] != null
                && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1)].tag.Equals(World.WATER))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1));
                foundSpawn = true;
            }
            //checking the row above the gameObject
            else if (!foundSpawn && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)] != null
                && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)].tag.Equals(World.WATER))
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
            if (!foundSpawn && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && terrainArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag.Equals(World.WATER))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j));
                foundSpawn = true;
            }
            //checking the column to the right of the gameObject
            else if (!foundSpawn && terrainArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && terrainArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag.Equals(World.WATER))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j));
                foundSpawn = true;
            }
            j++;
        }

        return spawnPosition;
    }

    /// <summary>
    /// Returns the progress the fishing boat has made in gathering fish
    /// </summary>
    /// <returns>The progress the fishing boat has made in gathering fish</returns>
    public double getProgressNum()
    {
        return 0.0;
    }
}
