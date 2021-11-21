using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behavior for an orc to collect taxes from nearby houses
/// </summary>
public class CollectTaxes : Animated {
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
    private World myWorld;
    private GameObject[,] structureArr;
    private GameObject[,] terrainArr;
    private Dictionary<GameObject, GameObject> locationsToVisit;
    private float taxPercentage;
    private int goldFromTaxes;
    private Animator animator;

    public float stepSize;
    public int searchRadius;

    /// <summary>
    /// Initializes the collect taxes class
    /// </summary>
    private void Awake()
    {
        originalLocation = new Vector2();
        reachedGoal = false;
        changePath = false;
        runningAStar = false;
        headingHome = false;
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        locationsToVisit = new Dictionary<GameObject, GameObject>();
        goldFromTaxes = 0;
        animator = gameObject.GetComponent<Animator>();
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetHashCode();

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
                    if (structureArr[i, j] != null && structureArr[i, j].tag == World.HOUSE)
                    {
                        HouseInformation houseInfo = structureArr[i, j].GetComponent<HouseInformation>();
                        if (houseInfo.getHouseholdCurrency() > 1)//if the house has any money that can be collected. > 1 because it won't take money if only at 1 gold
                        {
                            if (i - 1 > 0 && structureArr[i - 1, j] != null && structureArr[i - 1, j].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i - 1, j]);
                            }
                            else if (i + 1 < structureArr.GetLength(0) && structureArr[i + 1, j] != null
                                && structureArr[i + 1, j].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i + 1, j]);
                            }
                            else if (j - 1 > 0 && structureArr[i, j - 1] != null && structureArr[i, j - 1].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i, j - 1]);
                            }
                            else if (j + 1 < structureArr.GetLength(1) && structureArr[i, j + 1] != null
                                && structureArr[i, j + 1].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i, j + 1]);
                            }
                            else if (i - 2 > 0 && structureArr[i - 2, j] != null && structureArr[i - 2, j].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i - 2, j]);
                            }
                            else if (i + 2 < structureArr.GetLength(0) && structureArr[i + 2, j] != null
                                && structureArr[i + 2, j].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i + 2, j]);
                            }
                            else if (j - 2 > 0 && structureArr[i, j - 2] != null && structureArr[i, j - 2].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i, j - 2]);
                            }
                            else if (j + 2 < structureArr.GetLength(1) && structureArr[i, j + 2] != null
                                && structureArr[i, j + 2].tag.Equals(World.ROAD))
                            {
                                locationsToVisit.Add(structureArr[i, j], structureArr[i, j + 2]);
                            }
                        }
                    }
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (runningAStar == false)
        {
            StartCoroutine(runCollectTaxes());
        }
	}

    /// <summary>
    /// Sends the tax collector orc to each house with gold within its radius to collect taxes
    /// </summary>
    /// <returns>The delay before continuing to run the method</returns>
    private IEnumerator runCollectTaxes()
    {
        //if the place of employment is destroyed, this gameobject should be as well
        if (placeOfEmployment == null)
        {
            Destroy(gameObject);
        }
        //plan a path to the closest value in locationsToVisit
        if (path == null || path.Count == 0 || changePath == true)
        {
            animator.SetBool(Animated.MOVING_DOWN, false);
            animator.SetBool(Animated.MOVING_UP, false);
            animator.SetBool(Animated.MOVING_SIDEWAYS, false);
            animator.SetBool(Animated.IDLE, true);
            currentCharacterAnimation = characterAnimation.Idle;

            network = new GameObject[myWorld.mapSize, myWorld.mapSize];
            for (int i = 0; i < network.GetLength(0); i++)
            {
                for (int j = 0; j < network.GetLength(1); j++)
                {
                    if (structureArr[i, j] == null)
                    {
                        network[i, j] = terrainArr[i, j];
                    }
                    else if (!structureArr[i, j].tag.Equals(World.HOUSE))
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
                if (placeOfEmployment == null)
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
                tax();
                //If the orc collected right when it spawned and has nowhere else to go
                if (headingHome && path.Count == 0)
                {
                    TaxCollector taxCollector = placeOfEmployment.GetComponent<TaxCollector>();
                    taxCollector.setCollectorStatus(false);
                    updateCityGold();
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
                    && (!network[(int)nextLocation.x, (int)nextLocation.y].tag.Equals(World.BUILDING)
                    || network[(int)nextLocation.x, (int)nextLocation.y] == goalObject)))
                {
                    //take a step towards the nextLocation
                    Vector2 vector = new Vector2(nextLocation.x - currentLocation.x, nextLocation.y - currentLocation.y);
                    float magnitude = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
                    Vector2 unitVector = new Vector2(vector.x / magnitude, vector.y / magnitude);
                    Vector2 newLocation = new Vector2(currentLocation.x + unitVector.x * stepSize, currentLocation.y
                        + unitVector.y * stepSize);
                    gameObject.transform.position = newLocation;

                    //animation
                    if (unitVector.x > 0 && Mathf.Abs(vector.x) > Mathf.Abs(vector.y) && currentCharacterAnimation != characterAnimation.Right)
                    {
                        if (flipped)
                        {
                            flipSprite();
                        }
                        animator.SetBool(Animated.IDLE, false);
                        animator.SetBool(Animated.MOVING_DOWN, false);
                        animator.SetBool(Animated.MOVING_UP, false);
                        animator.SetBool(Animated.MOVING_SIDEWAYS, true);
                        currentCharacterAnimation = characterAnimation.Right;
                    }
                    else if (unitVector.x < 0 && Mathf.Abs(vector.x) > Mathf.Abs(vector.y) && currentCharacterAnimation != characterAnimation.Left)
                    {
                        //left. needs to flip sprite because it reuses the sprite for moving right
                        if (!flipped)
                        {
                            flipSprite();
                        }
                        animator.SetBool(Animated.IDLE, false);
                        animator.SetBool(Animated.MOVING_DOWN, false);
                        animator.SetBool(Animated.MOVING_UP, false);
                        animator.SetBool(Animated.MOVING_SIDEWAYS, true);
                        currentCharacterAnimation = characterAnimation.Left;
                    }
                    else if (unitVector.y > 0 && Mathf.Abs(vector.y) > Mathf.Abs(vector.x) && currentCharacterAnimation != characterAnimation.Up)
                    {
                        if (flipped)
                        {
                            flipSprite();
                        }
                        animator.SetBool(Animated.IDLE, false);
                        animator.SetBool(Animated.MOVING_DOWN, false);
                        animator.SetBool(Animated.MOVING_SIDEWAYS, false);
                        animator.SetBool(Animated.MOVING_UP, true);
                        currentCharacterAnimation = characterAnimation.Up;
                    }
                    else if (unitVector.y < 0 && Mathf.Abs(vector.y) > Mathf.Abs(vector.x) && currentCharacterAnimation != characterAnimation.Down)
                    {
                        if (flipped)
                        {
                            flipSprite();
                        }
                        animator.SetBool(Animated.IDLE, false);
                        animator.SetBool(Animated.MOVING_SIDEWAYS, false);
                        animator.SetBool(Animated.MOVING_UP, false);
                        animator.SetBool(Animated.MOVING_DOWN, true);
                        currentCharacterAnimation = characterAnimation.Down;
                    }

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
                            tax();
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
                                TaxCollector taxCollector = placeOfEmployment.GetComponent<TaxCollector>();
                                taxCollector.setCollectorStatus(false);
                                updateCityGold();
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
            if (placeOfEmployment == null)
            {
                Destroy(gameObject);
            }
            runningAStar = false;
        }

        yield return null;
    }

    /// <summary>
    /// Tax nearby houses that have not yet been taxed
    /// </summary>
    private void tax()
    {
        Vector2 currentLocation = new Vector2(Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.y));
        //find the houses that are around this road goal object
        //probably best to use a nested for loop only searching around the gameObject's current position to see if there
        // are houses within a distance of 2 and if those houses are still in locationsToVisit (rather than foreach through locationsToVisit)
        ArrayList housesToCollectFrom = new ArrayList();
        for (int i = 1; i < 3; i++)
        {
            if (structureArr[(int)currentLocation.x - i, (int)currentLocation.y] != null
                && locationsToVisit.ContainsKey(structureArr[(int)currentLocation.x - i, (int)currentLocation.y]))
            {
                housesToCollectFrom.Add(structureArr[(int)currentLocation.x - i, (int)currentLocation.y]);
            }
            else if (structureArr[(int)currentLocation.x + i, (int)currentLocation.y] != null
                && locationsToVisit.ContainsKey(structureArr[(int)currentLocation.x + i, (int)currentLocation.y]))
            {
                housesToCollectFrom.Add(structureArr[(int)currentLocation.x + i, (int)currentLocation.y]);
            }
            else if (structureArr[(int)currentLocation.x, (int)currentLocation.y - i] != null
                && locationsToVisit.ContainsKey(structureArr[(int)currentLocation.x, (int)currentLocation.y - i]))
            {
                housesToCollectFrom.Add(structureArr[(int)currentLocation.x, (int)currentLocation.y - i]);
            }
            else if (structureArr[(int)currentLocation.x, (int)currentLocation.y + i] != null
                && locationsToVisit.ContainsKey(structureArr[(int)currentLocation.x, (int)currentLocation.y + i]))
            {
                housesToCollectFrom.Add(structureArr[(int)currentLocation.x, (int)currentLocation.y + i]);
            }
        }

        //Collect the taxes
        foreach (GameObject house in housesToCollectFrom)
        {
            HouseInformation houseInformation = house.GetComponent<HouseInformation>();
            int goldFromHouse = Mathf.FloorToInt(houseInformation.getGoldSinceLastTax() * taxPercentage);//RoundToInt?
            houseInformation.updateHouseholdCurrency(World.TAX, -goldFromHouse);
            goldFromTaxes += goldFromHouse;

            locationsToVisit.Remove(house);
        }
    }

    /// <summary>
    /// Updates the gold count of the city based on how much gold has been collected in taxes
    /// </summary>
    private void updateCityGold()
    {
        TaxCollector taxCollector = placeOfEmployment.GetComponent<TaxCollector>();
        taxCollector.updateCityCurrencyFromTaxes(goldFromTaxes);
        //myWorld.updateCurrency(goldFromTaxes);
    }
    
    /// <summary>
    /// Checks the distance between two points.
    /// </summary>
    /// <param name="point1">point1 is the first point</param>
    /// <param name="point2">point2 is the second point</param>
    /// <returns>the distance between the two points rounded to an int</returns>
    int distance(Vector2 point1, Vector2 point2)
    {
        return Mathf.RoundToInt(Mathf.Sqrt((point2.x - point1.x)
            * (point2.x - point1.x) + (point2.y - point1.y)
            * (point2.y - point1.y)));
    }
    
    /// <summary>
    /// Sets the tax percentage this tax collector will be using to collect taxes from houses
    /// </summary>
    /// <param name="taxPercentage">the tax percentage (0.0-1.0)</param>
    public void setTaxPercentage(float taxPercentage)
    {
        this.taxPercentage = taxPercentage;
    }
    
    /// <summary>
    /// Sets the place the distribution orc starts at.
    /// </summary>
    /// <param name="position">position is the position the distribution orc spawned in at</param>
    public void setOriginalLocation(Vector2 position)
    {
        originalLocation = position;
    }
    
    /// <summary>
    /// Sets the place of employment this distribution orc works for.
    /// </summary>
    /// <param name="employment">employment the place of employment</param>
    public void setOrcEmployment(GameObject employment)
    {
        placeOfEmployment = employment;
    }

    /// <summary>
    /// Gets whether the tax collector is returning to its place of employment
    /// </summary>
    /// <returns>Whether the tax collector is returning to its place of employment</returns>
    public bool getHeadingHome()
    {
        return headingHome;
    }
}
