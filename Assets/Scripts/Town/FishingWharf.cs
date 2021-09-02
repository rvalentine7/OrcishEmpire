using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uses boats to gather fish from fishing spots in the water
/// </summary>
public class FishingWharf : BoatRequester
{
    private World myWorld;
    private StandardBoat standardBoat;//TODO: make standardboat and fishing boat inherit Boat?
    private FishingBoat fishingBoat;
    private bool hasBoat;
    private int numFishingTripsTakenByBoat;
    private bool hasFish;
    private Employment employment;
    private bool fishingBoatWaiting;

    public int numFishingTripsBeforeBoatBreaks;
    public GameObject fishingBoatGameObject;
    public int searchRadius;
    public GameObject deliveryOrc;
    public int numResourceProduced;

    /// <summary>
    /// Initializes the building
    /// </summary>
    void Start()
    {
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        hasBoat = false;
        numFishingTripsTakenByBoat = 0;
        hasFish = false;
        employment = gameObject.GetComponent<Employment>();
        fishingBoatWaiting = false;
    }

    /// <summary>
    /// Logic for what action the building should take at any given point in time
    /// </summary>
    void Update()//TODO: enable/disable
    {
        if (employment.getNumHealthyWorkers() > 0)
        {
            //Handling delivery orcs
            if (hasFish && !employment.getWorkerDeliveringGoods())
            {
                //We got fish to deliver
                createDeliveryOrc();
                hasFish = false;
            }

            //Handling boats
            if (!hasBoat)
            {
                //Search for a boat
                StartCoroutine(findStandardBoat());
            }
            else if (hasBoat)
            {
                if (fishingBoatWaiting)
                {
                    if (!hasFish)
                    {
                        takeFishFromBoat();
                    }
                }
                else if (standardBoat == null && fishingBoat == null && !hasFish)
                {
                    //We have a boat, but we're not fishing. Try to fish
                    Vector2 fishingBoatSpawnLocation = findBoatSpawnPosition();
                    GameObject newFishingBoat = Instantiate(fishingBoatGameObject, new Vector2(fishingBoatSpawnLocation.x, fishingBoatSpawnLocation.y), Quaternion.identity);
                    fishingBoat = newFishingBoat.GetComponent<FishingBoat>();
                    fishingBoat.setFishingWharf(gameObject);
                }
            }
        }
    }

    /// <summary>
    /// The fishing boat returned with fish.  This updates numbers associated with the fishing boat successfully returning from a fishing trip
    /// </summary>
    public void fishingBoatReturnedWithFish()
    {
        fishingBoatWaiting = true;
    }

    /// <summary>
    /// Takes the fish from the fishing boat
    /// </summary>
    private void takeFishFromBoat()
    {
        fishingBoatWaiting = false;
        fishingBoat.destroyFishingBoat();
        fishingBoat = null;
        hasFish = true;
        if (++numFishingTripsTakenByBoat >= numFishingTripsBeforeBoatBreaks)
        {
            numFishingTripsTakenByBoat = 0;
            hasBoat = false;
        }
    }

    /// <summary>
    /// Finds a boat from a Boatyard
    /// </summary>
    /// <returns>A time delay to split this search over multiple frames</returns>
    private IEnumerator findStandardBoat()
    {
        //checks for the closest boatyard
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
    /// Receive a StandardBoat that can be used by this building
    /// </summary>
    /// <param name="boat">The boat to be used by this building</param>
    public override void receiveBoat(StandardBoat boat)
    {
        standardBoat = boat;
        hasBoat = true;
    }

    /// <summary>
    /// A standard boat has arrived.  We can forget about the standard boat now
    /// as we still know hasBoat is true
    /// </summary>
    public override void standardBoatArrived()
    {
        standardBoat = null;
    }

    /// <summary>
    /// Removes a standard boat from this building and updates hasBoat to be false
    /// </summary>
    public override void cancelStandardBoat()
    {
        standardBoat = null;
        hasBoat = false;
    }

    /// <summary>
    /// Removes a fishing boat from this building and updates hasBoat to be false
    /// </summary>
    public void removeFishingBoat()
    {
        fishingBoat = null;
        hasBoat = false;
    }

    /// <summary>
    /// Returns the location a boat will be received by this building
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
    /// Finds a valid position to spawn a boat
    /// </summary>
    /// <returns>A valid position to spawn a boat</returns>
    private Vector2 findBoatSpawnPosition()
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
        deliver.addResources(World.FISH, numResourceProduced);
        deliver.setOriginalLocation(spawnPosition);
        deliver.setOrcEmployment(gameObject);
        employment.setWorkerDeliveringGoods(true);
    }

    /// <summary>
    /// Gets the employment for the fishing wharf
    /// </summary>
    /// <returns>The employment for the fishing wharf</returns>
    public Employment getEmployment()
    {
        return employment;
    }

    /// <summary>
    /// Gets whether this building has a boat tied to it
    /// </summary>
    /// <returns>Whether the building has a boat tied to it</returns>
    public bool getHasBoat()
    {
        return this.hasBoat;
    }

    /// <summary>
    /// Gets whether the fishing boat is waiting to drop off fish at the building
    /// </summary>
    /// <returns>Whether the fishing boat is waiting to drop off fish at the building</returns>
    public bool getFishingBoatWaiting()
    {
        return this.fishingBoatWaiting;
    }

    /// <summary>
    /// Gets whether the fishing boat is out fishing
    /// </summary>
    /// <returns>Whether the fishing boat is out fishing</returns>
    public bool getFishingBoatOutFishing()
    {
        return fishingBoat;
    }

    /// <summary>
    /// Gets whether a standard boat exists
    /// </summary>
    /// <returns>Whether a standard boat exists</returns>
    public bool getStandardBoat()
    {
        return standardBoat;
    }

    /// <summary>
    /// Returns the progress the fishing boat has made in gathering fish
    /// </summary>
    /// <returns>The progress the fishing boat has made in gathering fish</returns>
    public double getProgressNum()
    {
        double progressNum = 0.0;
        if (hasFish)
        {
            progressNum = 100.0;
        }
        else if (fishingBoat != null)
        {
            progressNum = fishingBoat.getProgressNum();
        }
        return progressNum;
    }
}
