using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Aqueduct : MonoBehaviour {
    public float timeInterval;//for checking if it should be filled with water
    public float waterDelay;
    public Sprite empty2NE;
    public Sprite empty2NS;
    public Sprite empty2NW;
    public Sprite empty2SE;
    public Sprite empty2SW;
    public Sprite empty2WE;
    public Sprite empty2WEArch;
    public Sprite empty3NSE;
    public Sprite empty3NSW;
    public Sprite empty3NWE;
    public Sprite empty3SWE;
    public Sprite empty4;
    public Sprite filled2NE;
    public Sprite filled2NS;
    public Sprite filled2NW;
    public Sprite filled2SE;
    public Sprite filled2SW;
    public Sprite filled2WE;
    public Sprite filled2WEArch;
    public Sprite filled3NSE;
    public Sprite filled3NSW;
    public Sprite filled3NWE;
    public Sprite filled3SWE;
    public Sprite filled4;

    private GameObject world;
    private World myWorld;
    private GameObject topConnection = null;
    private GameObject botConnection = null;
    private GameObject leftConnection = null;
    private GameObject rightConnection = null;
    private List<GameObject> waterSourcesFromTop;//TODO: change these to hold references to the reservoirs filled with water
    private List<GameObject> waterSourcesFromBot;
    private List<GameObject> waterSourcesFromLeft;
    private List<GameObject> waterSourcesFromRight;
    private List<GameObject> waterSources;
    private bool initialPlacement;
    private bool preparingToUpdateWaterSources;
    private float timeToUpdateWaterSources;

    private void Awake()
    {
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        topConnection = null;
        botConnection = null;
        leftConnection = null;
        rightConnection = null;
        waterSourcesFromTop = new List<GameObject>();
        waterSourcesFromBot = new List<GameObject>();
        waterSourcesFromLeft = new List<GameObject>();
        waterSourcesFromRight = new List<GameObject>();
        waterSources = new List<GameObject>();
        initialPlacement = true;
        preparingToUpdateWaterSources = true;
        timeToUpdateWaterSources = Time.time + waterDelay;
        //updateConnections();
        //updateNeighbors();
        //fillNeighbors();
    }

    // Use this for initialization
    void Start () {
        updateConnections();
        updateNeighbors();
        //fillNeighbors();
    }
	
	// Update is called once per frame
	void Update () {
        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        Vector2 aqueductPos = gameObject.transform.position;
        if (structureArr[(int)aqueductPos.x, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y].tag == "Road"
            && topConnection == null && botConnection == null && leftConnection == null && rightConnection == null)
        {
            Destroy(gameObject);
        }
        if (preparingToUpdateWaterSources && Time.time > timeToUpdateWaterSources)
        {
            preparingToUpdateWaterSources = false;
            fillNeighbors();
        }
        //TODO: water originates from nearWater reservoirs.  If a reservoir is not near water, check if filled
    }

    /**
     * Updates the visuals and connections of the aqueduct being placed
     */
    public void updateConnections()
    {
        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        Vector2 aqueductPos = gameObject.transform.position;
        int waterSourcesCount = waterSourcesFromTop.Count + waterSourcesFromBot.Count + waterSourcesFromLeft.Count + waterSourcesFromRight.Count;

        bool north = false;
        bool south = false;
        bool west = false;
        bool east = false;
        //Checking the adjacent aqueducts to determine what connections are available
        int nearbyAqueductCount = 0;
        if ((int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<Aqueduct>() != null)
        {
            nearbyAqueductCount++;
            north = true;
        }
        if ((int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<Aqueduct>() != null)
        {
            nearbyAqueductCount++;
            south = true;
        }
        if ((int)aqueductPos.x - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<Aqueduct>() != null)
        {
            nearbyAqueductCount++;
            west = true;
        }
        if ((int)aqueductPos.x + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<Aqueduct>() != null)
        {
            nearbyAqueductCount++;
            east = true;
        }

        int nearbyArchCount = 0;
        bool topArch = false;
        if ((int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].tag == "Road"
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>() != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>().getAqueduct() != null)
        {
            topArch = true;
            nearbyArchCount++;
            north = true;
        }
        bool botArch = false;
        if ((int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].tag == "Road"
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>() != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>().getAqueduct() != null)
        {
            botArch = true;
            nearbyArchCount++;
            south = true;
        }
        bool leftArch = false;
        if ((int)aqueductPos.x - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].tag == "Road"
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>() != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct() != null)
        {
            leftArch = true;
            nearbyArchCount++;
            west = true;
        }
        bool rightArch = false;
        if ((int)aqueductPos.x + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].tag == "Road"
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>() != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct() != null)
        {
            rightArch = true;
            nearbyArchCount++;
            east = true;
        }

        int nearbyReservoirCount = 0;
        if ((int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<Reservoir>() != null)
        {
            GameObject reservoirObj = structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1];
            if (gameObject.transform.position.x == Mathf.RoundToInt(reservoirObj.transform.position.x))
            {
                nearbyReservoirCount++;
                north = true;
            }
        }
        if ((int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<Reservoir>() != null)
        {
            GameObject reservoirObj = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1];
            if (gameObject.transform.position.x == Mathf.RoundToInt(reservoirObj.transform.position.x))
            {
                nearbyReservoirCount++;
                south = true;
            }
        }
        if ((int)aqueductPos.x - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<Reservoir>() != null)
        {
            GameObject reservoirObj = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y];
            if (gameObject.transform.position.y == Mathf.RoundToInt(reservoirObj.transform.position.y))
            {
                nearbyReservoirCount++;
                west = true;
            }
        }
        if ((int)aqueductPos.x + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<Reservoir>() != null)
        {
            GameObject reservoirObj = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y];
            if (gameObject.transform.position.y == Mathf.RoundToInt(reservoirObj.transform.position.y))
            {
                nearbyReservoirCount++;
                east = true;
            }
        }

        bool overRoad = false;
        if (structureArr[(int)aqueductPos.x, (int)aqueductPos.y].tag == "Road")
        {
            overRoad = true;
        }

        //TODO: update the 4 and 3 connections to account for when an arch is going north/south or east/west and an aqueduct is placed on the east/west or north/south respectively
        //Update sprite and add the connections.  Need to check if any are arches to see if I need to update based on those... currently adding sprites
        //Four connections
        if (north && south && west && east)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
            bool northArchConnectionValid = false;
            bool southArchConnectionValid = false;
            bool westArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                if (topArch)
                {
                    Aqueduct topAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (topAqueductScript.getLeftConnection() == null && topAqueductScript.getRightConnection() == null)
                    {
                        northArchConnectionValid = true;
                    }
                }
                if (botArch)
                {
                    Aqueduct botAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (botAqueductScript.getLeftConnection() == null && botAqueductScript.getRightConnection() == null)
                    {
                        southArchConnectionValid = true;
                    }
                }
                if (leftArch)
                {
                    Aqueduct leftAqueductScript = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (leftAqueductScript.getTopConnection() == null && leftAqueductScript.getBotConnection() == null)
                    {
                        westArchConnectionValid = true;
                    }
                }
                if (rightArch)
                {
                    Aqueduct rightAqueductScript = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (rightAqueductScript.getTopConnection() == null && rightAqueductScript.getBotConnection() == null)
                    {
                        eastArchConnectionValid = true;
                    }
                }
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            topConnection = northConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] : null;
            botConnection = southConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] : null;
            leftConnection = westConnectionValid ? structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] : null;
            rightConnection = eastConnectionValid ? structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] : null;
            //TODO: will need to check for if the connection is an arch/reservoir/aqueduct as they will have different components
            if (northConnectionValid && southConnectionValid && westConnectionValid && eastConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled4;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty4;
                }
            }
            else if (northConnectionValid && westConnectionValid && eastConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled3NWE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty3NWE;
                }
            }
            else if (southConnectionValid && westConnectionValid && eastConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled3SWE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty3SWE;
                }
            }
            else if (northConnectionValid && southConnectionValid && eastConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled3NSE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty3NSE;
                }
            }
            else if (northConnectionValid && southConnectionValid && westConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled3NSW;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty3NSW;
                }
            }
            else if (northConnectionValid && southConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                }
            }
            else if (northConnectionValid && westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NW;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NW;
                }
            }
            else if (southConnectionValid && westConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2SW;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2SW;
                }
            }
            else if (northConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                }
            }
            else if (southConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                }
            }
            else if (westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                }
            }
        }
        //Three connections
        else if (north && south && west)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
            bool northArchConnectionValid = false;
            bool southArchConnectionValid = false;
            bool westArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                if (topArch)
                {
                    Aqueduct topAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (topAqueductScript.getLeftConnection() == null && topAqueductScript.getRightConnection() == null)
                    {
                        northArchConnectionValid = true;
                    }
                }
                if (botArch)
                {
                    Aqueduct botAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (botAqueductScript.getLeftConnection() == null && botAqueductScript.getRightConnection() == null)
                    {
                        southArchConnectionValid = true;
                    }
                }
                if (leftArch)
                {
                    Aqueduct leftAqueductScript = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (leftAqueductScript.getTopConnection() == null && leftAqueductScript.getBotConnection() == null)
                    {
                        westArchConnectionValid = true;
                    }
                }
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            topConnection = northConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] : null;
            botConnection = southConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] : null;
            leftConnection = westConnectionValid ? structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] : null;
            rightConnection = null;
            //TODO: will need to check for if the connection is an arch/reservoir/aqueduct as they will have different components
            if (northConnectionValid && southConnectionValid && westConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled3NSW;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty3NSW;
                }
            }
            else if (northConnectionValid && southConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                }
            }
            else if (northConnectionValid && westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NW;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NW;
                }
            }
            else if (southConnectionValid && westConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2SW;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2SW;
                }
            }
            else if (northConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                }
            }
            else if (southConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                }
            }
            else if (westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                }
            }
        }
        else if (north && south && east)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
            bool northArchConnectionValid = false;
            bool southArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                if (topArch)
                {
                    Aqueduct topAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (topAqueductScript.getLeftConnection() == null && topAqueductScript.getRightConnection() == null)
                    {
                        northArchConnectionValid = true;
                    }
                }
                if (botArch)
                {
                    Aqueduct botAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (botAqueductScript.getLeftConnection() == null && botAqueductScript.getRightConnection() == null)
                    {
                        southArchConnectionValid = true;
                    }
                }
                if (rightArch)
                {
                    Aqueduct rightAqueductScript = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (rightAqueductScript.getTopConnection() == null && rightAqueductScript.getBotConnection() == null)
                    {
                        eastArchConnectionValid = true;
                    }
                }
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            topConnection = northConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] : null;
            botConnection = southConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] : null;
            rightConnection = eastConnectionValid ? structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] : null;
            leftConnection = null;
            //TODO: will need to check for if the connection is an arch/reservoir/aqueduct as they will have different components
            if (northConnectionValid && southConnectionValid && eastConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled3NSE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty3NSE;
                }
            }
            else if (northConnectionValid && southConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                }
            }
            else if (northConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NE;
                }
            }
            else if (southConnectionValid && eastConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2SE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2SE;
                }
            }
            else if (northConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                }
            }
            else if (southConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                }
            }
            else if (eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                }
            }
        }
        else if (north && west && east)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
            bool northArchConnectionValid = false;
            bool westArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                if (topArch)
                {
                    Aqueduct topAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (topAqueductScript.getLeftConnection() == null && topAqueductScript.getRightConnection() == null)
                    {
                        northArchConnectionValid = true;
                    }
                }
                if (leftArch)
                {
                    Aqueduct leftAqueductScript = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (leftAqueductScript.getTopConnection() == null && leftAqueductScript.getBotConnection() == null)
                    {
                        westArchConnectionValid = true;
                    }
                }
                if (rightArch)
                {
                    Aqueduct rightAqueductScript = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (rightAqueductScript.getTopConnection() == null && rightAqueductScript.getBotConnection() == null)
                    {
                        eastArchConnectionValid = true;
                    }
                }
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            topConnection = northConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] : null;
            leftConnection = westConnectionValid ? structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] : null;
            rightConnection = eastConnectionValid ? structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] : null;
            botConnection = null;
            //TODO: will need to check for if the connection is an arch/reservoir/aqueduct as they will have different components
            if (northConnectionValid && westConnectionValid && eastConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled3NWE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty3NWE;
                }
            }
            else if (northConnectionValid && westConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NW;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NW;
                }
            }
            else if (northConnectionValid && eastConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NE;
                }
            }
            else if (westConnectionValid && eastConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                }
            }
            else if (northConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                }
            }
            else if (westConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                }
            }
            else if (eastConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                }
            }
        }
        else if (south && west && east)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
            bool southArchConnectionValid = false;
            bool westArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                if (botArch)
                {
                    Aqueduct botAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (botAqueductScript.getLeftConnection() == null && botAqueductScript.getRightConnection() == null)
                    {
                        southArchConnectionValid = true;
                    }
                }
                if (leftArch)
                {
                    Aqueduct leftAqueductScript = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (leftAqueductScript.getTopConnection() == null && leftAqueductScript.getBotConnection() == null)
                    {
                        westArchConnectionValid = true;
                    }
                }
                if (rightArch)
                {
                    Aqueduct rightAqueductScript = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (rightAqueductScript.getTopConnection() == null && rightAqueductScript.getBotConnection() == null)
                    {
                        eastArchConnectionValid = true;
                    }
                }
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            botConnection = southConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] : null;
            leftConnection = westConnectionValid ? structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] : null;
            rightConnection = eastConnectionValid ? structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] : null;
            topConnection = null;
            //TODO: will need to check for if the connection is an arch/reservoir/aqueduct as they will have different components
            if (southConnectionValid && westConnectionValid && eastConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled3SWE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty3SWE;
                }
            }
            else if (southConnectionValid && westConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2SW;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2SW;
                }
            }
            else if (southConnectionValid && eastConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2SE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2SE;
                }
            }
            else if (westConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                }
            }
            else if (southConnectionValid)
            {
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                }
            }
            else if (westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                }
            }
            else if (eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                }
            }
        }
        //Two connections
        else if (north && south)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
            bool northArchConnectionValid = false;
            bool southArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                if (topArch)
                {
                    Aqueduct topAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (topAqueductScript.getLeftConnection() == null && topAqueductScript.getRightConnection() == null)
                    {
                        northArchConnectionValid = true;
                    }
                }
                if (botArch)
                {
                    Aqueduct botAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (botAqueductScript.getLeftConnection() == null && botAqueductScript.getRightConnection() == null)
                    {
                        southArchConnectionValid = true;
                    }
                }
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            topConnection = northConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] : null;
            botConnection = southConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] : null;
            leftConnection = null;
            rightConnection = null;
            //TODO: will need to check for if the connection is an arch/reservoir/aqueduct as they will have different components
            if (!overRoad || initialPlacement)
            {
                if (northConnectionValid && southConnectionValid)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
                else if (northConnectionValid)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
                else if (southConnectionValid)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
            }
        }
        else if (north && west)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
            bool northArchConnectionValid = false;
            bool westArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                if (topArch)
                {
                    Aqueduct topAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (topAqueductScript.getLeftConnection() == null && topAqueductScript.getRightConnection() == null)
                    {
                        northArchConnectionValid = true;
                    }
                }
                if (leftArch)
                {
                    Aqueduct leftAqueductScript = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (leftAqueductScript.getTopConnection() == null && leftAqueductScript.getBotConnection() == null)
                    {
                        westArchConnectionValid = true;
                    }
                }
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            topConnection = northConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] : null;
            leftConnection = westConnectionValid ? structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] : null;
            botConnection = null;
            rightConnection = null;
            //TODO: will need to check for if the connection is an arch/reservoir/aqueduct as they will have different components
            if (!overRoad || initialPlacement)
            {
                if (northConnectionValid && westConnectionValid)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NW;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NW;
                    }
                }
                else if (northConnectionValid)
                {
                    gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
                else if (westConnectionValid)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                    }
                }
            }

        }
        else if (north && east)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
            bool northArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                if (topArch)
                {
                    Aqueduct topAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (topAqueductScript.getLeftConnection() == null && topAqueductScript.getRightConnection() == null)
                    {
                        northArchConnectionValid = true;
                    }
                }
                if (rightArch)
                {
                    Aqueduct rightAqueductScript = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (rightAqueductScript.getTopConnection() == null && rightAqueductScript.getBotConnection() == null)
                    {
                        eastArchConnectionValid = true;
                    }
                }
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            topConnection = northConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] : null;
            rightConnection = eastConnectionValid ? structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] : null;
            botConnection = null;
            leftConnection = null;
            //TODO: will need to check for if the connection is an arch/reservoir/aqueduct as they will have different components
            if (!overRoad || initialPlacement)
            {
                if (northConnectionValid && eastConnectionValid)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NE;
                    }
                }
                else if (northConnectionValid)
                {
                    gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
                else if (eastConnectionValid)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                    }
                }
            }
        }
        else if (south && west)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
            bool southArchConnectionValid = false;
            bool westArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                if (botArch)
                {
                    Aqueduct botAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (botAqueductScript.getLeftConnection() == null && botAqueductScript.getRightConnection() == null)
                    {
                        southArchConnectionValid = true;
                    }
                }
                if (leftArch)
                {
                    Aqueduct leftAqueductScript = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (leftAqueductScript.getTopConnection() == null && leftAqueductScript.getBotConnection() == null)
                    {
                        westArchConnectionValid = true;
                    }
                }
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            botConnection = southConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] : null;
            leftConnection = westConnectionValid ? structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] : null;
            topConnection = null;
            rightConnection = null;
            //TODO: will need to check for if the connection is an arch/reservoir/aqueduct as they will have different components
            if (!overRoad || initialPlacement)
            {
                if (southConnectionValid && westConnectionValid)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2SW;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2SW;
                    }
                }
                else if (southConnectionValid)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
                else if (westConnectionValid)
                {
                    gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                    }
                }
            }
        }
        else if (south && east)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
            bool southArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                if (botArch)
                {
                    Aqueduct botAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (botAqueductScript.getLeftConnection() == null && botAqueductScript.getRightConnection() == null)
                    {
                        southArchConnectionValid = true;
                    }
                }
                if (rightArch)
                {
                    Aqueduct rightAqueductScript = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (rightAqueductScript.getTopConnection() == null && rightAqueductScript.getBotConnection() == null)
                    {
                        eastArchConnectionValid = true;
                    }
                }
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            botConnection = southConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] : null;
            rightConnection = eastConnectionValid ? structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] : null;
            topConnection = null;
            leftConnection = null;
            //TODO: will need to check for if the connection is an arch/reservoir/aqueduct as they will have different components
            if (!overRoad || initialPlacement)
            {
                if (southConnectionValid && eastConnectionValid)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2SE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2SE;
                    }
                }
                else if (southConnectionValid)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
                else if (eastConnectionValid)
                {
                    gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                    }
                }
            }
        }
        else if (west && east)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
            bool westArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                if (leftArch)
                {
                    Aqueduct leftAqueductScript = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (leftAqueductScript.getTopConnection() == null && leftAqueductScript.getBotConnection() == null)
                    {
                        westArchConnectionValid = true;
                    }
                }
                if (rightArch)
                {
                    Aqueduct rightAqueductScript = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (rightAqueductScript.getTopConnection() == null && rightAqueductScript.getBotConnection() == null)
                    {
                        eastArchConnectionValid = true;
                    }
                }
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            leftConnection = westConnectionValid ? structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] : null;
            rightConnection = eastConnectionValid ? structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] : null;
            topConnection = null;
            botConnection = null;
            //TODO: will need to check for if the connection is an arch/reservoir/aqueduct as they will have different components
            if (!overRoad || initialPlacement)
            {
                if (westConnectionValid && eastConnectionValid)
                {
                    gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                    if (waterSourcesCount > 0)
                    {
                        if (overRoad)
                        {
                            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
                            gameObject.GetComponent<SpriteRenderer>().sprite = filled2WEArch;
                        }
                        else
                        {
                            gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                        }
                    }
                    else
                    {
                        if (overRoad)
                        {
                            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
                            gameObject.GetComponent<SpriteRenderer>().sprite = empty2WEArch;
                        }
                        else
                        {
                            gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                        }
                    }
                }
                else if (westConnectionValid)
                {
                    gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                    }
                }
                else if (eastConnectionValid)
                {
                    gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                    }
                }
            }
            else if (overRoad && westConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
                if (waterSourcesCount > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = filled2WEArch;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = empty2WEArch;
                }
            }
        }
        //One connection
        else if (north)
        {
            topConnection = null;
            botConnection = null;
            leftConnection = null;
            rightConnection = null;
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
            if (nearbyReservoirCount > 0)
            {
                topConnection = structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1];
                if (overRoad)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
                else
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
            }
            else if (nearbyArchCount > 0)
            {
                //Check if it can connect
                bool connectionPossible = false;
                Aqueduct topAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                if (topAqueductScript.getLeftConnection() == null && topAqueductScript.getRightConnection() == null)
                {
                    connectionPossible = true;
                }
                //TODO: have a method to check connections and see if this gameObject is the only connection? if so, connection is possible... other option is to check if any aqueducts around
                // what I'm checking if I can connect with

                if (connectionPossible)
                {
                    topConnection = structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1];
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
            }
            else
            {
                topConnection = structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1];
                if (overRoad)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
                else
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
            }
        }
        else if (south)
        {
            topConnection = null;
            botConnection = null;
            leftConnection = null;
            rightConnection = null;
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
            if (nearbyReservoirCount > 0)
            {
                botConnection = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1];
                if (overRoad)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
                else
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
            }
            else if (nearbyArchCount > 0)
            {
                //Check if it can connect
                bool connectionPossible = false;
                Aqueduct botAqueductScript = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                if (botAqueductScript.getLeftConnection() == null && botAqueductScript.getRightConnection() == null)
                {
                    connectionPossible = true;
                }

                if (connectionPossible)
                {
                    botConnection = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1];
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
            }
            else
            {
                botConnection = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1];
                if (overRoad)
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
                else
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2NS;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2NS;
                    }
                }
            }
        }
        else if (west)
        {
            topConnection = null;
            botConnection = null;
            leftConnection = null;
            rightConnection = null;
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
            if (nearbyReservoirCount > 0)
            {
                leftConnection = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y];
                if (overRoad)
                {
                    gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WEArch;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WEArch;
                    }
                }
                else
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                    }
                }
            }
            else if (nearbyArchCount > 0)
            {
                //Check if it can connect
                bool connectionPossible = false;
                if (leftArch)
                {
                    Aqueduct leftAqueductScript = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (leftAqueductScript.getTopConnection() == null && leftAqueductScript.getBotConnection() == null)
                    {
                        connectionPossible = true;
                    }
                }

                if (connectionPossible)
                {
                    leftConnection = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y];
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                    }
                }
            }
            else
            {
                leftConnection = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y];
                if (overRoad)
                {
                    gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WEArch;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WEArch;
                    }
                }
                else
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                    }
                }
            }
        }
        else if (east)
        {
            topConnection = null;
            botConnection = null;
            leftConnection = null;
            rightConnection = null;
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
            if (nearbyReservoirCount > 0)
            {
                rightConnection = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y];
                if (overRoad)
                {
                    gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WEArch;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WEArch;
                    }
                }
                else
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                    }
                }
            }
            else if (nearbyArchCount > 0)
            {
                //Check if it can connect
                bool connectionPossible = false;
                if (rightArch)
                {
                    Aqueduct rightAqueductScript = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                    if (rightAqueductScript.getTopConnection() == null && rightAqueductScript.getBotConnection() == null)
                    {
                        connectionPossible = true;
                    }
                }

                if (connectionPossible)
                {
                    rightConnection = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y];
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                    }
                }
            }
            else
            {
                rightConnection = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y];
                if (overRoad)
                {
                    gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WEArch;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WEArch;
                    }
                }
                else
                {
                    if (waterSourcesCount > 0)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = filled2WE;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = empty2WE;
                    }
                }
            }
        }
        initialPlacement = false;
        if (overRoad && topConnection == null && botConnection == null && leftConnection == null && rightConnection == null)
        {
            Destroy(gameObject);
        }

        //TODO: at the end, I should call a fill method that checks connected aqueducts/reservoirs if they have access to nearWater reservoirs

        //TODO: when deleting an aqueduct, update the surrounding aqueducts, if the surrounding aqueducts got water from the deleted aqueduct, update them so they don't get it from that direction
    }


    //maybe not use these?
    public void setTopConnection(GameObject aqueduct)
    {
        topConnection = aqueduct;
    }

    public void setBotConnection(GameObject aqueduct)
    {
        botConnection = aqueduct;
    }

    public void setLeftConnection(GameObject aqueduct)
    {
        leftConnection = aqueduct;
    }

    public void setRightConnection(GameObject aqueduct)
    {
        rightConnection = aqueduct;
    }

    /**
     * Gets the connected aqueduct to the north/top of this one
     */
    public GameObject getTopConnection()
    {
        return topConnection;
    }

    /*
     * Gets the connected aqueduct to the south/bot of this one
     */
    public GameObject getBotConnection()
    {
        return botConnection;
    }

    /**
     * Gets the connected aqueduct to the west/left of this one
     */
    public GameObject getLeftConnection()
    {
        return leftConnection;
    }

    /**
     * Gets the connected aqueduct to the east/right of this one
     */
    public GameObject getRightConnection()
    {
        return rightConnection;
    }

    /**
     * Updates the connecting neighbors of this aqueduct to also connect to this aqueduct
     */
    public void updateNeighbors()
    {
        if (topConnection != null)
        {
            if (topConnection.GetComponent<Reservoir>() != null)
            {
                topConnection.GetComponent<Reservoir>().updateConnections();
            }
            else if (topConnection.GetComponent<RoadInformation>() != null)
            {
                //Still update connections so that it can get a reference to this
                topConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().updateConnections();
            }
            else
            {
                topConnection.GetComponent<Aqueduct>().updateConnections();
            }
        }
        if (botConnection != null)
        {
            if (botConnection.GetComponent<Reservoir>() != null)
            {
                botConnection.GetComponent<Reservoir>().updateConnections();
            }
            else if (botConnection.GetComponent<RoadInformation>() != null)
            {
                //Still update connections so that it can get a reference to this
                botConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().updateConnections();
            }
            else
            {
                botConnection.GetComponent<Aqueduct>().updateConnections();
            }
        }
        if (leftConnection != null)
        {
            if (leftConnection.GetComponent<Reservoir>() != null)
            {
                leftConnection.GetComponent<Reservoir>().updateConnections();
            }
            else if (leftConnection.GetComponent<RoadInformation>() != null)
            {
                //Still update connections so that it can get a reference to this
                leftConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().updateConnections();
            }
            else
            {
                leftConnection.GetComponent<Aqueduct>().updateConnections();
            }
        }
        if (rightConnection != null)
        {
            if (rightConnection.GetComponent<Reservoir>() != null)
            {
                rightConnection.GetComponent<Reservoir>().updateConnections();
            }
            else if (rightConnection.GetComponent<RoadInformation>() != null)
            {
                //Still update connections so that it can get a reference to this
                rightConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().updateConnections();
            }
            else
            {
                rightConnection.GetComponent<Aqueduct>().updateConnections();
            }
        }
    }

    /**
     * Gets the aqueducts/reservoirs connected to this aqueduct
     */
    public List<GameObject> getConnections()
    {
        List<GameObject> connections = new List<GameObject>();
        if (topConnection != null)
        {
            connections.Add(topConnection);
        }
        if (botConnection != null)
        {
            connections.Add(botConnection);
        }
        if (leftConnection != null)
        {
            connections.Add(leftConnection);
        }
        if (rightConnection != null)
        {
            connections.Add(rightConnection);
        }
        return connections;
    }

    public void fillNeighbors()
    {
        //Get references to how the water sources are before they are updated to see if they need to update neighbors
        List<GameObject> prevNorthWaterSources = waterSourcesFromTop;
        List<GameObject> prevSouthWaterSources = waterSourcesFromBot;
        List<GameObject> prevWestWaterSources = waterSourcesFromLeft;
        List<GameObject> prevEastWaterSources = waterSourcesFromRight;
        //Updates the current water sources
        setWaterSources();
        //Water sources could be different now.  If they are, update neighbors
        bool northSourcesEqual = prevNorthWaterSources.All(waterSourcesFromTop.Contains) && prevNorthWaterSources.Count == waterSourcesFromTop.Count;
        bool southSourcesEqual = prevSouthWaterSources.All(waterSourcesFromBot.Contains) && prevSouthWaterSources.Count == waterSourcesFromBot.Count;
        bool westSourcesEqual = prevWestWaterSources.All(waterSourcesFromLeft.Contains) && prevWestWaterSources.Count == waterSourcesFromLeft.Count;
        bool eastSourcesEqual = prevEastWaterSources.All(waterSourcesFromRight.Contains) && prevEastWaterSources.Count == waterSourcesFromRight.Count;
        if (!northSourcesEqual || !southSourcesEqual || !westSourcesEqual || !eastSourcesEqual)
        {
            if (topConnection != null)
            {
                if (topConnection.tag == "Road")
                {
                    if (topConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                    {
                        topConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().setTimeToUpdateWaterSources();
                    }
                }
                else if (topConnection.GetComponent<Reservoir>() != null)
                {
                    topConnection.GetComponent<Reservoir>().setTimeToUpdateWaterSources();
                }
                else if (topConnection.GetComponent<Aqueduct>() != null)
                {
                    topConnection.GetComponent<Aqueduct>().setTimeToUpdateWaterSources();
                }
            }
            if (botConnection != null)
            {
                if (botConnection.tag == "Road")
                {
                    if (botConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                    {
                        botConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().setTimeToUpdateWaterSources();
                    }
                }
                else if (botConnection.GetComponent<Reservoir>() != null)
                {
                    botConnection.GetComponent<Reservoir>().setTimeToUpdateWaterSources();
                }
                else if (botConnection.GetComponent<Aqueduct>() != null)
                {
                    botConnection.GetComponent<Aqueduct>().setTimeToUpdateWaterSources();
                }
            }
            if (leftConnection != null)
            {
                if (leftConnection.tag == "Road")
                {
                    if (leftConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                    {
                        leftConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().setTimeToUpdateWaterSources();
                    }
                }
                else if (leftConnection.GetComponent<Reservoir>() != null)
                {
                    leftConnection.GetComponent<Reservoir>().setTimeToUpdateWaterSources();
                }
                else if (leftConnection.GetComponent<Aqueduct>() != null)
                {
                    leftConnection.GetComponent<Aqueduct>().setTimeToUpdateWaterSources();
                }
            }
            if (rightConnection != null)
            {
                if (rightConnection.tag == "Road")
                {
                    if (rightConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                    {
                        rightConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().setTimeToUpdateWaterSources();
                    }
                }
                else if (rightConnection.GetComponent<Reservoir>() != null)
                {
                    rightConnection.GetComponent<Reservoir>().setTimeToUpdateWaterSources();
                }
                else if (rightConnection.GetComponent<Aqueduct>() != null)
                {
                    rightConnection.GetComponent<Aqueduct>().setTimeToUpdateWaterSources();
                }
            }
        }
        updateConnections();
    }

    /**
     * Sets the water sources this aqueduct receives from its neighboring connections
     */
    public void setWaterSources()
    {
        waterSourcesFromTop = new List<GameObject>();
        waterSourcesFromBot = new List<GameObject>();
        waterSourcesFromLeft = new List<GameObject>();
        waterSourcesFromRight = new List<GameObject>();
        //Sets the water source lists (the 4 directions and the overall)
        if (topConnection != null)
        {
            if (topConnection.tag == "Road")
            {
                if (topConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                {
                    waterSourcesFromTop = topConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().getWaterSources();
                }
            }
            else if (topConnection.GetComponent<Reservoir>() != null)
            {
                waterSourcesFromTop = topConnection.GetComponent<Reservoir>().getWaterSources();
            }
            else if (topConnection.GetComponent<Aqueduct>() != null)
            {
                waterSourcesFromTop = topConnection.GetComponent<Aqueduct>().getWaterSources();
            }
        }
        if (botConnection != null)
        {
            if (botConnection.tag == "Road")
            {
                if (botConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                {
                    waterSourcesFromBot = botConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().getWaterSources();
                }
            }
            else if (botConnection.GetComponent<Reservoir>() != null)
            {
                waterSourcesFromBot = botConnection.GetComponent<Reservoir>().getWaterSources();
            }
            else if (botConnection.GetComponent<Aqueduct>() != null)
            {
                waterSourcesFromBot = botConnection.GetComponent<Aqueduct>().getWaterSources();
            }
        }
        if (leftConnection != null)
        {
            if (leftConnection.tag == "Road")
            {
                if (leftConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                {
                    waterSourcesFromLeft = leftConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().getWaterSources();
                }
            }
            else if (leftConnection.GetComponent<Reservoir>() != null)
            {
                waterSourcesFromLeft = leftConnection.GetComponent<Reservoir>().getWaterSources();
            }
            else if (leftConnection.GetComponent<Aqueduct>() != null)
            {
                waterSourcesFromLeft = leftConnection.GetComponent<Aqueduct>().getWaterSources();
            }
        }
        if (rightConnection != null)
        {
            if (rightConnection.tag == "Road")
            {
                if (rightConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                {
                    waterSourcesFromRight = rightConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().getWaterSources();
                }
            }
            else if (rightConnection.GetComponent<Reservoir>() != null)
            {
                waterSourcesFromRight = rightConnection.GetComponent<Reservoir>().getWaterSources();
            }
            else if (rightConnection.GetComponent<Aqueduct>() != null)
            {
                waterSourcesFromRight = rightConnection.GetComponent<Aqueduct>().getWaterSources();
            }
        }
        waterSources = new List<GameObject>();
        foreach (GameObject waterSource in waterSourcesFromTop)
        {
            if (!waterSources.Contains(waterSource))
            {
                waterSources.Add(waterSource);
            }
        }
        foreach (GameObject waterSource in waterSourcesFromBot)
        {
            if (!waterSources.Contains(waterSource))
            {
                waterSources.Add(waterSource);
            }
        }
        foreach (GameObject waterSource in waterSourcesFromLeft)
        {
            if (!waterSources.Contains(waterSource))
            {
                waterSources.Add(waterSource);
            }
        }
        foreach (GameObject waterSource in waterSourcesFromRight)
        {
            if (!waterSources.Contains(waterSource))
            {
                waterSources.Add(waterSource);
            }
        }
    }

    /**
     * Gets the sources filling this aqueduct
     */
    public List<GameObject> getWaterSources()
    {
        return waterSources;
    }

    public void removeWaterSource(GameObject waterSource)
    {
        //TODO: go through the 5 waterSources lists and remove this water source and then tell neighboring connections to do the same
    }

    /**
     * Sets the time at which the aqueduct should update its water sources
     */
    public void setTimeToUpdateWaterSources()
    {
        timeToUpdateWaterSources = Time.time + waterDelay;
        preparingToUpdateWaterSources = true;
    }

    /**
     * Destroys the aqueduct
     */
    public void destroyAqueduct()
    {
        Destroy(gameObject);
    }
}
