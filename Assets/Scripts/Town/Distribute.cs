using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Update the bit in awake that adds houses to visit so that houses with plenty of food already are not visited (prioritize houses with less food?)
//TODO: Collect, Delivery, and Distribute need to be updated with regards to heading back to the original location of the orc
//  (what happens when the original location is removed... need to find a different road segment)
/**
 * Sends a worker from the marketplace to nearby houses to deliver goods.
 */
public class Distribute : MonoBehaviour {
    //private Dictionary<string, int> resources;
    private GameObject[,] network;
    private Vector2 originalLocation;
    private GameObject placeOfEmployment;
    private List<Vector2> path;
    private Vector2 goal;
    private GameObject goalObject;
    private bool reachedGoal;
    private bool changePath;
    private bool runningAStar;
    private bool headingHome;
    private GameObject world;
    private World myWorld;
    private GameObject[,] structureArr;
    private GameObject[,] terrainArr;
    private Dictionary<GameObject, GameObject> locationsToVisit;
    public float stepSize;
    public int searchRadius;
    public int foodNumToDistribute;
    public int waterNumToDistribute;

    /**
     * Initializes the distribute class
     */
    void Awake()
    {
        //resources = new Dictionary<string, int>();
        originalLocation = new Vector2();
        reachedGoal = false;
        changePath = false;
        runningAStar = false;
        headingHome = false;
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        locationsToVisit = new Dictionary<GameObject, GameObject>();

        /**
         * Get a list of all squares in the worker's travel radius that have houses in a proximity of two
         * Key = House, Value = Closest Road
         */
        for (int i = (int)gameObject.transform.position.x - searchRadius; i <= (int)gameObject.transform.position.x + searchRadius; i++)
        {
            for (int j = (int)gameObject.transform.position.y - searchRadius; j <= (int)gameObject.transform.position.y + searchRadius; j++)
            {
                if (i > 0 && i < structureArr.GetLength(0) && j > 0 && j < structureArr.GetLength(1))
                {
                    //TODO: this will need to be updated if I decide to include houses with sizes different from 1x1
                    if (structureArr[i, j] != null && structureArr[i, j].tag == "House")
                    {
                        Storage houseStorage = structureArr[i, j].GetComponent<Storage>();
                        if (houseStorage.acceptsResource("Meat", foodNumToDistribute))
                        {
                            if (i - 1 > 0 && structureArr[i - 1, j] != null && structureArr[i - 1, j].tag == "Road")
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i - 1, j]);
                            }
                            else if (i + 1 < structureArr.GetLength(0) && structureArr[i + 1, j] != null
                                && structureArr[i + 1, j].tag == "Road")
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i + 1, j]);
                            }
                            else if (j - 1 > 0 && structureArr[i, j - 1] != null && structureArr[i, j - 1].tag == "Road")
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i, j - 1]);
                            }
                            else if (j + 1 < structureArr.GetLength(1) && structureArr[i, j + 1] != null
                                && structureArr[i, j + 1].tag == "Road")
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i, j + 1]);
                            }
                            else if (i - 2 > 0 && structureArr[i - 2, j] != null && structureArr[i - 2, j].tag == "Road")
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i - 2, j]);
                            }
                            else if (i + 2 < structureArr.GetLength(0) && structureArr[i + 2, j] != null
                                && structureArr[i + 2, j].tag == "Road")
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i + 2, j]);
                            }
                            else if (j - 2 > 0 && structureArr[i, j - 2] != null && structureArr[i, j - 2].tag == "Road")
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i, j - 2]);
                            }
                            else if (j + 2 < structureArr.GetLength(1) && structureArr[i, j + 2] != null
                                && structureArr[i, j + 2].tag == "Road")
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i, j + 2]);
                            }
                        }
                    }
                }
            }
        }
    }

    
    void Start () {
		
	}
	
	/**
     * Runs the distribution orc
     */
	void Update () {
        if (runningAStar == false)
        {
            StartCoroutine(runDistribute());
        }
    }

    /**
     * Sends the distribution orc to each house within its radius for as long as it has goods to distribute
     */
    private IEnumerator runDistribute()
    {
        //if the place of employment is destroyed, this gameobject should be as well
        if (!placeOfEmployment)
        {
            Destroy(gameObject);
        }
        //if the place of employment has no more goods to distribute, set locationsToVisit
        // to an empty dictionary and set it to change the path
        Storage employmentStorage = placeOfEmployment.GetComponent<Storage>();
        if ((locationsToVisit.Count == 0 && !headingHome) || 
            (!headingHome && employmentStorage.getFoodCount() == 0
            && employmentStorage.getWaterCount() == 0))
        {
            locationsToVisit = new Dictionary<GameObject, GameObject>();
            changePath = true;
            headingHome = true;
        }
        //plan a path to the closest value in locationsToVisit
        if (path == null || path.Count == 0 || changePath == true)
        {
            network = new GameObject[myWorld.mapSize, myWorld.mapSize];
            for (int i = 0; i < network.GetLength(0); i++)
            {
                for (int j = 0; j < network.GetLength(1); j++)
                {
                    if (structureArr[i, j] == null)
                    {
                        network[i, j] = terrainArr[i, j];
                    }
                    else if (structureArr[i, j].tag != "House")
                    {
                        network[i, j] = structureArr[i, j];
                    }
                }
            }
            //can start searching locations closest to the worker by starting the search around its current location in the dictionary
            //this will require going through the dictionary and checking distance from current gameobject location to the location
            // of the road associated with each house
            int shortestDistance = int.MaxValue;
            goalObject = null;
            foreach (KeyValuePair<GameObject, GameObject> kvp in locationsToVisit)
            {
                int kvpDistance = distance(kvp.Value.transform.position, gameObject.transform.position);
                if (kvpDistance < shortestDistance)
                {
                    shortestDistance = kvpDistance;
                    goalObject = kvp.Value;
                }
            }
            //if there are no houses left to visit, return to employment
            if (locationsToVisit.Count == 0)
            {
                goalObject = network[Mathf.RoundToInt(originalLocation.x), Mathf.RoundToInt(originalLocation.y)];
                headingHome = true;
            }
            AstarSearch aStarSearch = new AstarSearch();
            if (goalObject != null)
            {
                runningAStar = true;
                Vector2 location = gameObject.transform.position;
                yield return StartCoroutine(aStarSearch.aStar(tempPath =>
                {
                    runningAStar = false;
                    path = tempPath;
                    if (path != null)
                    {
                        goal = path[path.Count - 1];
                    }
                }, new Vector2(Mathf.RoundToInt(location.x), Mathf.RoundToInt(location.y)), goalObject, network));
                //if the place of employment is destroyed, this gameobject should be as well
                if (!placeOfEmployment)
                {
                    Destroy(gameObject);
                }
            }
            if (path != null)
            {
                changePath = false;
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer.enabled == false)
                {
                    spriteRenderer.enabled = true;
                }
            }
        }
        if (path != null && path.Count > 0)
        {
            Vector2 currentLocation = gameObject.transform.position;
            //If the orc starts in the goal spot
            if (network[(int)originalLocation.x, Mathf.RoundToInt(originalLocation.y)] != null
                && goalObject == network[Mathf.RoundToInt(originalLocation.x), Mathf.RoundToInt(originalLocation.y)]
                && currentLocation.x == originalLocation.x && Mathf.RoundToInt(currentLocation.y) == originalLocation.y)
            {
                path.RemoveAt(0);
                distributeGoods();
                //If the orc delivered right when it spawned and it has no more food, it is done
                if (headingHome && path.Count == 0)
                {
                    Marketplace market = placeOfEmployment.GetComponent<Marketplace>();
                    market.setDistributorStatus(false);
                    Destroy(gameObject);
                }
            }
            else
            {
                //use path to go to the next available vector2 in it
                float distance = Mathf.Sqrt((path[0].x - gameObject.transform.position.x)
                        * (path[0].x - gameObject.transform.position.x) + (path[0].y - gameObject.transform.position.y)
                        * (path[0].y - gameObject.transform.position.y));
                if (path[0] == currentLocation || distance < stepSize)
                {
                    path.RemoveAt(0);
                }
                Vector2 nextLocation = originalLocation;//TODO: this will need to be updated to make sure the location still exists
                if (path.Count > 0)
                {
                    nextLocation = path[0];
                }
                else
                {
                    changePath = true;
                }
                //if the orc is heading home or the goalobject exists, take a step; otherwise, change the path
                if ((goalObject != null || headingHome || reachedGoal) && (network[(int)nextLocation.x, (int)nextLocation.y] != null
                    && (network[(int)nextLocation.x, (int)nextLocation.y].tag != "Building"
                    || network[(int)nextLocation.x, (int)nextLocation.y] == goalObject)))
                {
                    //take a step towards the nextLocation
                    Vector2 vector = new Vector2(nextLocation.x - currentLocation.x, nextLocation.y - currentLocation.y);
                    float magnitude = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
                    Vector2 unitVector = new Vector2(vector.x / magnitude, vector.y / magnitude);
                    Vector2 newLocation = new Vector2(currentLocation.x + unitVector.x * stepSize, currentLocation.y
                        + unitVector.y * stepSize);
                    gameObject.transform.position = newLocation;

                    //if the agent gets to the next vector then delete it from the path
                    // and go to the next available vector
                    float distanceBetweenPoints = Mathf.Sqrt((nextLocation.x - gameObject.transform.position.x)
                        * (nextLocation.x - gameObject.transform.position.x) + (nextLocation.y - gameObject.transform.position.y)
                        * (nextLocation.y - gameObject.transform.position.y));
                    bool nextIsGoal = false;
                    if (nextLocation == goal)
                    {
                        nextIsGoal = true;
                    }
                    if (distanceBetweenPoints < stepSize)
                    {
                        path.RemoveAt(0);
                        if (!headingHome)
                        {
                            distributeGoods();
                            //If the agent is done distributing goods
                            if (changePath == true)
                            {
                                nextIsGoal = false;
                            }
                        }
                    }
                    if (path.Count == 0)
                    {
                        changePath = true;
                        //If the agent has distributed goods and is back home
                        if (nextIsGoal && headingHome)
                        {
                            distanceBetweenPoints = Mathf.Sqrt((goal.x - gameObject.transform.position.x)
                                * (goal.x - gameObject.transform.position.x) + (goal.y - gameObject.transform.position.y)
                                * (goal.y - gameObject.transform.position.y));
                            if (distanceBetweenPoints < stepSize)
                            {
                                Marketplace market = placeOfEmployment.GetComponent<Marketplace>();
                                market.setDistributorStatus(false);
                                Destroy(gameObject);
                            }
                        }
                    }
                }
                else
                {
                    changePath = true;
                }
                if (runningAStar == false && goalObject == null && !reachedGoal && !headingHome)
                {
                    changePath = true;
                }
            }
        }
        else
        {
            runningAStar = true;//Stopping the Update() method from  calling a new runDistribute()
            yield return new WaitForSeconds(1.0f);
            //if the place of employment is destroyed, this gameobject should be as well
            if (!placeOfEmployment)
            {
                Destroy(gameObject);
            }
            runningAStar = false;
        }

        yield return null;
    }

    /**
     * Distributes goods to locations around the orc's current location
     */
    public void distributeGoods()
    {
        Vector2 currentLocation = new Vector2(Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.y));
        //find the houses that are around this road goal object
        //probably best to use a nested for loop only searching around the gameObject's current position to see if there
        // are houses within a distance of 2 and if those houses are still in locationsToVisit (rather than foreach through locationsToVisit)
        ArrayList housesToDistributeTo = new ArrayList();
        for (int i = 1; i < 3; i++)
        {
            if (structureArr[(int)currentLocation.x - i, (int)currentLocation.y] != null
                && locationsToVisit.ContainsKey(structureArr[(int)currentLocation.x - i, (int)currentLocation.y]))
            {
                housesToDistributeTo.Add(structureArr[(int)currentLocation.x - i, (int)currentLocation.y]);
            }
            else if (structureArr[(int)currentLocation.x + i, (int)currentLocation.y] != null
                && locationsToVisit.ContainsKey(structureArr[(int)currentLocation.x + i, (int)currentLocation.y]))
            {
                housesToDistributeTo.Add(structureArr[(int)currentLocation.x + i, (int)currentLocation.y]);
            }
            else if (structureArr[(int)currentLocation.x, (int)currentLocation.y - i] != null
                && locationsToVisit.ContainsKey(structureArr[(int)currentLocation.x, (int)currentLocation.y - i]))
            {
            housesToDistributeTo.Add(structureArr[(int)currentLocation.x, (int)currentLocation.y - i]);
            }
            else if (structureArr[(int)currentLocation.x, (int)currentLocation.y + i] != null
                && locationsToVisit.ContainsKey(structureArr[(int)currentLocation.x, (int)currentLocation.y + i]))
            {
                housesToDistributeTo.Add(structureArr[(int)currentLocation.x, (int)currentLocation.y + i]);
            }
        }
        //Storage goalStorage = goal.GetComponent<Storage>();
        Storage employmentStorage = placeOfEmployment.GetComponent<Storage>();
        foreach (GameObject house in housesToDistributeTo)
        {
            Storage houseStorage = house.GetComponent<Storage>();
            if (employmentStorage.getFoodCount() > 0)
            {
                if (employmentStorage.getMeatCount() >= foodNumToDistribute
                    && houseStorage.acceptsResource("Meat", foodNumToDistribute))
                {
                    houseStorage.addResource("Meat", foodNumToDistribute);
                    employmentStorage.removeResource("Meat", foodNumToDistribute);
                }
                else if (employmentStorage.getMeatCount() > 0
                    && houseStorage.acceptsResource("Meat", foodNumToDistribute))
                {
                    houseStorage.addResource("Meat", employmentStorage.getMeatCount());
                    employmentStorage.removeResource("Meat", employmentStorage.getMeatCount());
                }
                if (employmentStorage.getWheatCount() >= foodNumToDistribute
                    && houseStorage.acceptsResource("Wheat", foodNumToDistribute))
                {
                    houseStorage.addResource("Wheat", foodNumToDistribute);
                    employmentStorage.removeResource("Wheat", foodNumToDistribute);
                }
                else if (employmentStorage.getWheatCount() > 0
                    && houseStorage.acceptsResource("Wheat", foodNumToDistribute))
                {
                    houseStorage.addResource("Wheat", employmentStorage.getWheatCount());
                    employmentStorage.removeResource("Wheat", employmentStorage.getWheatCount());
                }
                //TODO: add other types of food
            }
            //TODO: add other types of resources
            //Need to set public variables for how much any given house can hold special goods
            //Need to make it so houses will consume the goods over time
            //Need to make a way so I can turn off distribution to houses (option in UI popup?)

            locationsToVisit.Remove(house);
            if (!(employmentStorage.getFoodCount() > 0))
            {
                //needs to go back to employment
                headingHome = true;
                changePath = true;
                locationsToVisit = new Dictionary<GameObject, GameObject>();
                goalObject = placeOfEmployment;
                housesToDistributeTo = new ArrayList();
            }
        }
    }

    /**
     * Checks the distance between two points.
     * @param point1 is the first point
     * @param point2 is the second point
     * @return the distance between the two points rounded to an int
     */
    int distance(Vector2 point1, Vector2 point2)
    {
        return Mathf.RoundToInt(Mathf.Sqrt((point2.x - point1.x)
            * (point2.x - point1.x) + (point2.y - point1.y)
            * (point2.y - point1.y)));
    }

    /**
     * Sets the place the distribution orc starts at.
     * @param position is the position the distribution orc spawned in at
     */
    public void setOriginalLocation(Vector2 position)
    {
        originalLocation = position;
    }

    /**
     * Sets the place of employment this distribution orc works for.
     * @param employment the place of employment
     */
    public void setOrcEmployment(GameObject employment)
    {
        placeOfEmployment = employment;
    }
}
