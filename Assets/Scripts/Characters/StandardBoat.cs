using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A boat created by a Boatyard to be used by buildings such as FishingWharfs
/// </summary>
public class StandardBoat : Animated
{
    private BoatRequester boatRequester;
    private GameObject requestingBuilding;
    private GameObject[,] network;
    private World myWorld;
    private GameObject[,] structureArr;
    private GameObject[,] terrainArr;
    private bool runningAStar;
    private List<Vector2> path;
    private bool changePath;

    public float stepSize;
    public Sprite standardBoatNS;
    public Sprite standardBoatWE;

    /// <summary>
    /// Sets up the boat with some default information
    /// </summary>
    void Start()
    {
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        runningAStar = false;
        changePath = false;

        StartCoroutine(findPathToBoatRequester(returnPath =>
        {
            path = returnPath;
        }));
    }

    /// <summary>
    /// Tells the boat to move to the building requesting it as long as A* is not running
    /// </summary>
    void Update()
    {
        if (!runningAStar)
        {
            StartCoroutine(travelToRequester());
        }
    }

    /// <summary>
    /// Moves the boat to the building requesting it
    /// </summary>
    /// <returns>A time delay that splits the method execution over multiple frames</returns>
    private IEnumerator travelToRequester()
    {
        //if the place of employment is destroyed, this gameobject should be as well
        if (!requestingBuilding)
        {
            Destroy(gameObject);
        }

        if (path == null || path.Count == 0 || changePath == true)
        {
            if (runningAStar == false)
            {
                yield return StartCoroutine(findPathToBoatRequester(returnPath =>
                {
                    path = returnPath;
                }));
                //if the place of employment is destroyed, this gameobject should be as well
                if (!requestingBuilding)
                {
                    Destroy(gameObject);
                }
                //Could not find a path, likely in a different water section (otherwise, there's a bug)
                if (path == null || path.Count == 0)
                {
                    //Notify boatrequester it isn't getting a boat any more
                    boatRequester.cancelStandardBoat();
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
            Vector2 newLocation = new Vector2(currentLocation.x + unitVector.x * stepSize * Time.timeScale, currentLocation.y
                + unitVector.y * stepSize * Time.timeScale);
            gameObject.transform.position = newLocation;

            //animation
            if (unitVector.x > 0 && Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
            {
                //Moving right
                if (flipped)
                {
                    flipSprite();
                }
                gameObject.GetComponent<SpriteRenderer>().sprite = this.standardBoatWE;
            }
            else if (unitVector.x < 0 && Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
            {
                //Moving left
                if (!flipped)
                {
                    flipSprite();
                }
                gameObject.GetComponent<SpriteRenderer>().sprite = this.standardBoatWE;
            }
            else if (unitVector.y > 0 && Mathf.Abs(vector.y) > Mathf.Abs(vector.x))
            {
                //Moving up
                if (flipped)
                {
                    flipSprite();
                }
                gameObject.GetComponent<SpriteRenderer>().sprite = this.standardBoatNS;
            }
            else if (unitVector.y < 0 && Mathf.Abs(vector.y) > Mathf.Abs(vector.x))
            {
                //Moving down
                if (!flipped)
                {
                    flipSprite();
                }
                gameObject.GetComponent<SpriteRenderer>().sprite = this.standardBoatNS;
            }

            //if the agent gets to the next vector then delete it from the path
            // and go to the next available vector
            float distanceBetweenPoints = Mathf.Sqrt((nextLocation.x - gameObject.transform.position.x)
                * (nextLocation.x - gameObject.transform.position.x) + (nextLocation.y - gameObject.transform.position.y)
                * (nextLocation.y - gameObject.transform.position.y));
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
            if (path.Count == 1)//0 would be the building itself.  We don't want the boat to sail on top of the building
            {
                //Boat has arrived at its requester
                boatRequester.standardBoatArrived();
                Destroy(gameObject);
            }
        }
        yield return null;
    }

    /// <summary>
    /// Finds a path to the BoatRequester
    /// </summary>
    /// <param name="returnPath">The path to the BoatRequester</param>
    /// <returns>A time delay to split this method execution over multiple frames</returns>
    private IEnumerator findPathToBoatRequester(System.Action<List<Vector2>> returnPath)
    {
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
                //Need to include the areas of the requesting building
                else if (structureArr[i, j] == requestingBuilding)
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
        }, currentLocation, requestingBuilding, network));
        //if the place of employment is destroyed, this gameobject should be as well
        if (!requestingBuilding)
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
    /// Sets up the boat with information on the building requesting it
    /// </summary>
    /// <param name="boatRequesterGameObject">The building requesting this boat</param>
    public void initialize(GameObject boatRequesterGameObject)
    {
        this.boatRequester = boatRequesterGameObject.GetComponent<BoatRequester>();
        this.boatRequester.receiveBoat(this);
        this.requestingBuilding = boatRequesterGameObject;
    }
}
