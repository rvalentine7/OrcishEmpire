using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A boat that goes fishing for a fishing wharf
/// </summary>
public class FishingBoat : Animated
{
    private GameObject fishingWharfBuilding;
    private FishingWharf fishingWharf;
    private GameObject[,] network;
    private GameObject world;
    private World myWorld;
    private GameObject[,] structureArr;
    private GameObject[,] terrainArr;
    private bool runningAStar;
    private bool findingNearestFishingSpot;
    private List<Vector2> path;
    private bool changePath;
    private bool startedFishing;
    private bool finishedFishing;
    private float prevFishingTime;
    private GameObject goal;
    private float effectiveProgress;
    private Employment employment;

    public int searchRadius;
    public float stepSize;
    public Sprite fishingBoatNS;
    public Sprite fishingBoatWE;
    public float fishingTime;

    /// <summary>
    /// Initializes the fishing boat
    /// </summary>
    void Start()
    {
        world = GameObject.Find(World.WORLD_INFORMATION);
        myWorld = world.GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        runningAStar = false;
        findingNearestFishingSpot = false;
        changePath = false;
        startedFishing = false;
        finishedFishing = false;
        prevFishingTime = 0.0f;
        effectiveProgress = 0;

        StartCoroutine(findPathToGoal(returnPath =>
        {
            path = returnPath;
        }));
    }

    /// <summary>
    /// Updates the logic of the fishing boat
    /// </summary>
    void Update()
    {
        if (!fishingWharfBuilding)
        {
            Destroy(gameObject);
        }

        if (startedFishing && !finishedFishing)
        {
            float progressedTime = Time.unscaledTime - prevFishingTime;
            prevFishingTime = Time.unscaledTime;
            float effectiveTimeToFish = fishingTime / (employment.getNumWorkers() / employment.getNumHealthyWorkers());
            effectiveProgress += progressedTime / effectiveTimeToFish * 100;
            if (effectiveProgress >= 100)
            {
                effectiveProgress = 100;
                finishedFishing = true;
            }
        }
        else if (!runningAStar && !findingNearestFishingSpot)
        {
            StartCoroutine(travelToGoal());
        }
    }

    /// <summary>
    /// Updates the fishin boat's position on its way to the goal
    /// </summary>
    /// <returns>A time delay to split this method over multiple frames</returns>
    private IEnumerator travelToGoal()
    {
        //if the place of employment is destroyed, this gameobject should be as well
        if (!fishingWharfBuilding)
        {
            Destroy(gameObject);
        }

        if (path == null || path.Count == 0 || changePath == true)
        {
            if (runningAStar == false)
            {
                yield return StartCoroutine(findPathToGoal(returnPath =>
                {
                    path = returnPath;
                }));
                //if the place of employment is destroyed, this gameobject should be as well
                if (!fishingWharfBuilding)
                {
                    Destroy(gameObject);
                }
                //Could not find a path
                if ((path == null || path.Count == 0) && finishedFishing)
                {
                    fishingWharf.removeFishingBoat();
                    Destroy(gameObject);
                }
            }
            if (runningAStar == false)
            {
                changePath = false;
            }
        }
        else if (path != null && runningAStar == false)
        {
            //use path to go to the next available vector2 in it
            Vector2 currentLocation = gameObject.transform.position;
            if (path[0] == currentLocation)
            {
                path.RemoveAt(0);
            }
            Vector2 nextLocation = path[0];

            //take a step towards the nextLocation
            Vector2 vector = new Vector2(nextLocation.x - currentLocation.x, nextLocation.y - currentLocation.y);
            float magnitude = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
            Vector2 unitVector = new Vector2(vector.x / magnitude, vector.y / magnitude);
            Vector2 newLocation = new Vector2(currentLocation.x + unitVector.x * stepSize, currentLocation.y
                + unitVector.y * stepSize);
            gameObject.transform.position = newLocation;

            //animation
            if (unitVector.x > 0 && Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
            {
                //Moving right
                if (flipped)
                {
                    flipSprite();
                }
                gameObject.GetComponent<SpriteRenderer>().sprite = this.fishingBoatWE;
            }
            else if (unitVector.x < 0 && Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
            {
                //Moving left
                if (!flipped)
                {
                    flipSprite();
                }
                gameObject.GetComponent<SpriteRenderer>().sprite = this.fishingBoatWE;
            }
            else if (unitVector.y > 0 && Mathf.Abs(vector.y) > Mathf.Abs(vector.x))
            {
                //Moving up
                if (flipped)
                {
                    flipSprite();
                }
                gameObject.GetComponent<SpriteRenderer>().sprite = this.fishingBoatNS;
            }
            else if (unitVector.y < 0 && Mathf.Abs(vector.y) > Mathf.Abs(vector.x))
            {
                //Moving down
                if (!flipped)
                {
                    flipSprite();
                }
                gameObject.GetComponent<SpriteRenderer>().sprite = this.fishingBoatNS;
            }

            //if the agent gets to the next vector then delete it from the path
            // and go to the next available vector
            float distanceBetweenPoints = myWorld.getDistanceBetweenPoints(gameObject.transform.position, nextLocation);
            if (distanceBetweenPoints < stepSize)
            {
                path.RemoveAt(0);
                if (path.Count > 0 && structureArr[(int)path[0].x, (int)path[0].y] != null
                    && structureArr[(int)path[0].x, (int)path[0].y].tag.Equals(World.LOW_BRIDGE))
                {
                    //A bridge was constructed while travelling and the path is no longer viable
                    changePath = true;
                }
            }
            if (!finishedFishing && path.Count == 0)
            {
                //Reached fishing spot
                startedFishing = true;
                prevFishingTime = Time.unscaledTime;
            }
            else if (finishedFishing && path.Count == 1)
            {
                //Reached fishing wharf
                fishingWharf.fishingBoatReturnedWithFish();
            }
        }
        yield return null;
    }

    /// <summary>
    /// Finds the path to the goal
    /// </summary>
    /// <param name="returnPath">The path to the goal</param>
    /// <returns>A time delay to split this method over multiple frames</returns>
    private IEnumerator findPathToGoal(System.Action<List<Vector2>> returnPath)
    {
        //Setting the goal
        if (finishedFishing)
        {
            //Return to Fishing Wharf
            goal = fishingWharfBuilding;
        }
        else
        {
            //Go to fishing spot
            yield return StartCoroutine(findNearestAvailableFishingSpot(fishingSpot =>
            {
                goal = fishingSpot;
            }));
            //Couldn't find a valid fishing spot
            if (goal == null)
            {
                returnPath(new List<Vector2>());
                yield break;
            }
            //if the place of employment is destroyed, this gameobject should be as well
            if (!fishingWharfBuilding)
            {
                Destroy(gameObject);
            }
        }

        //Building the traversable network
        network = new GameObject[myWorld.mapSize, myWorld.mapSize];
        for (int i = 0; i < network.GetLength(0); i++)
        {
            for (int j = 0; j < network.GetLength(1); j++)
            {
                //Can travel on watery terrain that does not have a low bridge over it
                if ((structureArr[i, j] == null || structureArr[i, j].tag.Equals(World.HIGH_BRIDGE))
                    && myWorld.wateryTerrain.Contains(terrainArr[i, j].tag))
                {
                    network[i, j] = terrainArr[i, j];
                }
                else if (structureArr[i, j] == fishingWharfBuilding)
                {
                    network[i, j] = structureArr[i, j];
                }
            }
        }

        List<Vector2> path = new List<Vector2>();
        AstarSearch aStarSearch = new AstarSearch();
        Vector2 currentLocation = new Vector2(Mathf.RoundToInt(gameObject.transform.position.x),
            Mathf.RoundToInt(gameObject.transform.position.y));
        runningAStar = true;
        yield return StartCoroutine(aStarSearch.aStar(tempPath =>
        {
            runningAStar = false;
            if (tempPath != null && tempPath.Count > 0)
            {
                path = tempPath;
            }
        }, currentLocation, goal, network));//requestingBuilding would be the FishingSpot
        //if the place of employment is destroyed, this gameobject should be as well
        if (!fishingWharfBuilding)
        {
            Destroy(gameObject);
        }

        //no path exists. this object should be destroyed and the BoatRequester should be notified
        if (path.Count == 0)
        {
            returnPath(new List<Vector2>());
            yield break;
        }

        //A path exists
        returnPath(path);
        yield break;
    }
    
    /// <summary>
    /// Finds the nearest available fishing spot
    /// </summary>
    /// <param name="fishingSpot">The nearest available fishing spot</param>
    /// <returns>A time delay to split this method over multiple frames</returns>
    private IEnumerator findNearestAvailableFishingSpot(System.Action<GameObject> fishingSpot)
    {
        findingNearestFishingSpot = true;
        GameObject closestFishingSpot = null;
        float closestDistance = Mathf.Infinity;

        Vector2 fishingBoatLocation = gameObject.transform.position;
        int fishingBoatWaterSectionNum = terrainArr[Mathf.RoundToInt(fishingBoatLocation.x), Mathf.RoundToInt(fishingBoatLocation.y)].GetComponent<WaterTile>().getWaterSectionNum();
        for (int i = 0; i <= searchRadius * 2; i++)
        {
            for (int j = 0; j <= searchRadius * 2; j++)
            {
                if (fishingBoatLocation.x - searchRadius + i >= 0 && fishingBoatLocation.y - searchRadius + j >= 0
                        && fishingBoatLocation.x - searchRadius + i <= 39 && fishingBoatLocation.y - searchRadius + j <= 39
                        && terrainArr[(int)fishingBoatLocation.x - searchRadius + i, (int)fishingBoatLocation.y - searchRadius + j].tag.Equals(World.FISHING_SPOT)
                        && terrainArr[(int)fishingBoatLocation.x - searchRadius + i, (int)fishingBoatLocation.y - searchRadius + j].GetComponent<WaterTile>().getWaterSectionNum() == fishingBoatWaterSectionNum)
                {
                    float distance = myWorld.getDistanceBetweenPoints(fishingBoatLocation, new Vector2(fishingBoatLocation.x - searchRadius + i, fishingBoatLocation.y - searchRadius + j));
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestFishingSpot = terrainArr[(int)fishingBoatLocation.x - searchRadius + i, (int)fishingBoatLocation.y - searchRadius + j];
                    }
                }
            }
            yield return 0.1f;
        }
        fishingSpot(closestFishingSpot);
        findingNearestFishingSpot = false;
        yield break;
    }

    /// <summary>
    /// Sets the fishing wharf this fishing boat is tied to.  Also gets the employment
    /// </summary>
    /// <param name="fishingWharfBuilding">The fishing wharf this building is tied to</param>
    public void setFishingWharf(GameObject fishingWharfBuilding)
    {
        this.fishingWharfBuilding = fishingWharfBuilding;
        this.fishingWharf = fishingWharfBuilding.GetComponent<FishingWharf>();
        this.employment = fishingWharf.getEmployment();
    }

    /// <summary>
    /// Destroy the gameObject of this fishing boat
    /// </summary>
    public void destroyFishingBoat()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Gets the fishing progress
    /// </summary>
    /// <returns>The fishing progress</returns>
    public int getProgressNum()
    {
        return Mathf.FloorToInt(this.effectiveProgress);
    }
}
