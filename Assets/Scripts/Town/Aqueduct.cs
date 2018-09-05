using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aqueduct : MonoBehaviour {
    public float timeInterval;//for checking if it should be filled with water
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
    private GameObject topConnection;
    private GameObject botConnection;
    private GameObject leftConnection;
    private GameObject rightConnection;
    private List<GameObject> waterSourcesFromTop;//TODO: change these to hold references to the reservoirs filled with water
    private List<GameObject> waterSourcesFromBot;
    private List<GameObject> waterSourcesFromLeft;
    private List<GameObject> waterSourcesFromRight;
    private bool initialPlacement;

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
        initialPlacement = true;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//TODO: aqueducts should not connect to archs if they are not in line with each other (no corners over a road)
        //TODO: have methods for filling from directions
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
        bool topAq = false;
        if ((int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<Aqueduct>() != null)
        {
            topAq = true;
            nearbyAqueductCount++;
            north = true;
        }
        bool botAq = false;
        if ((int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<Aqueduct>() != null)
        {
            botAq = true;
            nearbyAqueductCount++;
            south = true;
        }
        bool leftAq = false;
        if ((int)aqueductPos.x - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<Aqueduct>() != null)
        {
            leftAq = true;
            nearbyAqueductCount++;
            west = true;
        }
        bool rightAq = false;
        if ((int)aqueductPos.x + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<Aqueduct>() != null)
        {
            rightAq = true;
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
        bool topRes = false;
        if ((int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<Reservoir>() != null)
        {
            topRes = true;
            nearbyReservoirCount++;
            north = true;
        }
        bool botRes = false;
        if ((int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<Reservoir>() != null)
        {
            botRes = true;
            nearbyReservoirCount++;
            south = true;
        }
        bool leftRes = false;
        if ((int)aqueductPos.x - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<Reservoir>() != null)
        {
            leftRes = true;
            nearbyReservoirCount++;
            west = true;
        }
        bool rightRes = false;
        if ((int)aqueductPos.x + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<Reservoir>() != null)
        {
            rightRes = true;
            nearbyReservoirCount++;
            east = true;
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
            int numValidConnections = 4;
            bool northArchConnectionValid = false;
            bool southArchConnectionValid = false;
            bool westArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    //if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] == null
                    //    || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                    //    && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                    //    || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    //{
                        northArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (botArch)
                {
                    //if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] == null
                    //    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    //&& ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    //{
                        southArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (leftArch)
                {
                    //if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        westArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (rightArch)
                {
                    //if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        eastArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                numValidConnections -= numInvalidConnections;
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
            int numValidConnections = 3;
            bool northArchConnectionValid = false;
            bool southArchConnectionValid = false;
            bool westArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    //if ((int)aqueductpos.y + 2 < myworld.mapsize && structurearr[(int)aqueductpos.x, (int)aqueductpos.y + 2] == null
                    //    || (int)aqueductpos.y + 2 < myworld.mapsize && structurearr[(int)aqueductpos.x, (int)aqueductpos.y + 2] != null
                    //    && ((int)aqueductpos.y + 2 < myworld.mapsize && structurearr[(int)aqueductpos.x, (int)aqueductpos.y + 2].getcomponent<reservoir>() != null
                    //    || (int)aqueductpos.y + 2 < myworld.mapsize && structurearr[(int)aqueductpos.x, (int)aqueductpos.y + 2].getcomponent<aqueduct>() != null))
                    //{
                        northArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (botArch)
                {
                    //if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] == null
                    //    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    //&& ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    //{
                        southArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (leftArch)
                {
                    //if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        westArchConnectionValid = true;
                    //}
                    //else
                    //{
                        numInvalidConnections++;
                    //}
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            topConnection = northConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] : null;
            botConnection = southConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] : null;
            leftConnection = westConnectionValid ? structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] : null;
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
            int numValidConnections = 3;
            bool northArchConnectionValid = false;
            bool southArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    //if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] == null
                    //    || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                    //    && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                    //    || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    //{
                        northArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (botArch)
                {
                    //if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] == null
                    //    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    //&& ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    //{
                        southArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (rightArch)
                {
                    //if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        eastArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            topConnection = northConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] : null;
            botConnection = southConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] : null;
            rightConnection = eastConnectionValid ? structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] : null;
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
            int numValidConnections = 3;
            bool northArchConnectionValid = false;
            bool westArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    //if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] == null
                    //    || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                    //    && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                    //    || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    //{
                        northArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (leftArch)
                {
                    //if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        westArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (rightArch)
                {
                    //if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        eastArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            topConnection = northConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] : null;
            leftConnection = westConnectionValid ? structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] : null;
            rightConnection = eastConnectionValid ? structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] : null;
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
            int numValidConnections = 3;
            bool southArchConnectionValid = false;
            bool westArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (botArch)
                {
                    //if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] == null
                    //    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    //    && ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    //    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    //{
                        southArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (leftArch)
                {
                    //if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        westArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (rightArch)
                {
                    //if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        eastArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool westConnectionValid = (rightArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (leftArch ? eastArchConnectionValid : true);
            botConnection = southConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] : null;
            leftConnection = westConnectionValid ? structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] : null;
            rightConnection = eastConnectionValid ? structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] : null;
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
            int numValidConnections = 2;
            bool northArchConnectionValid = false;
            bool southArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    //if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] == null
                    //    || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                    //    && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                    //    || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    //{
                        northArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (botArch)
                {
                    //if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] == null
                    //    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    //&& ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    //{
                        southArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            topConnection = northConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] : null;
            botConnection = southConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] : null;
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
            int numValidConnections = 2;
            bool northArchConnectionValid = false;
            bool westArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    //if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] == null
                    //    || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                    //    && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                    //    || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    //{
                        northArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (leftArch)
                {
                    //if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        westArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            topConnection = northConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] : null;
            leftConnection = westConnectionValid ? structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] : null;
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
            int numValidConnections = 2;
            bool northArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    //if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] == null
                    //    || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                    //    && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                    //    || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    //{
                        northArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (rightArch)
                {
                    //if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        eastArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            topConnection = northConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] : null;
            rightConnection = eastConnectionValid ? structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] : null;
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
            int numValidConnections = 2;
            bool southArchConnectionValid = false;
            bool westArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (botArch)
                {
                    //if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] == null
                    //    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    //    && ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    //    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    //{
                        southArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (leftArch)
                {
                    //if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        westArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            botConnection = southConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] : null;
            leftConnection = westConnectionValid ? structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] : null;
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
            int numValidConnections = 2;
            bool southArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (botArch)
                {
                    //if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] == null
                    //    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    //&& ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    //{
                        southArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (rightArch)
                {
                    //if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        eastArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            botConnection = southConnectionValid ? structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] : null;
            rightConnection = eastConnectionValid ? structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] : null;
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
            int numValidConnections = 2;
            bool westArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (leftArch)
                {
                    //if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        westArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                if (rightArch)
                {
                    //if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] == null
                    //    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    //&& ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    //|| (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    //{
                        eastArchConnectionValid = true;
                    //}
                    //else
                    //{
                    //    numInvalidConnections++;
                    //}
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            leftConnection = westConnectionValid ? structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] : null;
            rightConnection = eastConnectionValid ? structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] : null;
            //TODO: will need to check for if the connection is an arch/reservoir/aqueduct as they will have different components
            if (!overRoad || initialPlacement)
            {
                if (westConnectionValid && eastConnectionValid)
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
        }
        //One connection
        else if (north)
        {
            topConnection = structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1];
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
            if (nearbyReservoirCount > 0)
            {
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
                if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                    && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                {
                    connectionPossible = true;
                }
                else if ((int)aqueductPos.x - 1 > 0 && (int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y + 1] == null
                    || structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y + 1].GetComponent<RoadInformation>() != null)
                {
                    connectionPossible = true;
                }
                else if ((int)aqueductPos.x + 1 < myWorld.mapSize && (int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y + 1] == null
                    || structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y + 1].GetComponent<RoadInformation>() != null)
                {
                    connectionPossible = true;
                }
                //TODO: have a method to check connections and see if this gameObject is the only connection? if so, connection is possible... other option is to check if any aqueducts around
                // what I'm checking if I can connect with

                if (connectionPossible)
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
            else
            {
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
            botConnection = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1];
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
            if (nearbyReservoirCount > 0)
            {
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
                if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    && ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                {
                    connectionPossible = true;
                }
                else if ((int)aqueductPos.x - 1 > 0 && (int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y - 1] == null
                    || structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y - 1].GetComponent<RoadInformation>() != null)
                {
                    connectionPossible = true;
                }
                else if ((int)aqueductPos.x + 1 < myWorld.mapSize && (int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y - 1] == null
                    || structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y - 1].GetComponent<RoadInformation>() != null)
                {
                    connectionPossible = true;
                }

                if (connectionPossible)
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
            else
            {
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
            leftConnection = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y];
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
            if (nearbyReservoirCount > 0)
            {
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
                if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                {
                    connectionPossible = true;
                }
                else if ((int)aqueductPos.x - 1 > 0 && (int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y + 1] == null
                    || structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y + 1].GetComponent<RoadInformation>() != null)
                {
                    connectionPossible = true;
                }
                else if ((int)aqueductPos.x - 1 > 0 && (int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y - 1] == null
                    || structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y - 1].GetComponent<RoadInformation>() != null)
                {
                    connectionPossible = true;
                }

                if (connectionPossible)
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
            else
            {
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
            rightConnection = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y];
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
            if (nearbyReservoirCount > 0)
            {
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
                if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                {
                    connectionPossible = true;
                }
                else if ((int)aqueductPos.x + 1 < myWorld.mapSize && (int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y + 1] == null
                    || structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y + 1].GetComponent<RoadInformation>() != null)
                {
                    connectionPossible = true;
                }
                else if ((int)aqueductPos.x + 1 < myWorld.mapSize && (int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y - 1] == null
                    || structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y - 1].GetComponent<RoadInformation>() != null)
                {
                    connectionPossible = true;
                }

                if (connectionPossible)
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
            else
            {
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

        //topAq, topArch, topRes.... only thing that really matters here is the direction for the sprite.  the connection itself will need to get the arch and not the road, though
        //nearbyAqueductCount, nearbyArchCount, nearbyReservoirCount

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
        connections.Add(topConnection);
        connections.Add(botConnection);
        connections.Add(leftConnection);
        connections.Add(rightConnection);
        return connections;
    }

























    /*this bit went inside the 4 aqueduct check
     if (nearbyArchCount > 0)
            {
                bool northConnectionValid = false;
                bool southConnectionValid = false;
                bool westConnectionValid = false;
                bool eastConnectionValid = false;
                if (topArch)
                {
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    {
                        northConnectionValid = true;
                    }
                }
                if (botArch)
                {
                    if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    && ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    {
                        southConnectionValid = true;
                    }
                }
                if (leftArch)
                {
                    if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        westConnectionValid = true;
                    }
                }
                if (rightArch)
                {
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        eastConnectionValid = true;
                    }
                }
                if (northConnectionValid && southConnectionValid && westConnectionValid && eastConnectionValid)//4 arches and all are valid to connect to
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
                else if (northConnectionValid && southConnectionValid && westConnectionValid)//Have to check if there were 4 nearby arches, if so use the 3 sprite, otherwise use the 4
                {
                    if (nearbyArchCount == 4)
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
                    else
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
                }
                else if (northConnectionValid && southConnectionValid && eastConnectionValid)
                {
                    if (nearbyArchCount == 4)
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
                    else
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
                }
                else if (northConnectionValid && westConnectionValid && eastConnectionValid)
                {
                    if (nearbyArchCount == 4)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                        if (waterSourcesCount > 0)
                        {
                            gameObject.GetComponent<SpriteRenderer>().sprite = filled3NWE;
                        }
                        else
                        {
                            gameObject.GetComponent<SpriteRenderer>().sprite = empty3NWE;
                        }
                    }
                    else
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
                }
                else if (southConnectionValid && westConnectionValid && eastConnectionValid)
                {
                    if (nearbyArchCount == 4)
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
                    else
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
                }
                else if (northConnectionValid && southConnectionValid)//Have to check if there were more than 2 arches
                {
                    if (nearbyArchCount == 4)
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
                    else if (nearbyArchCount == 3)
                    {
                        //There is one valid aqueduct nearby that is not an arch
                        if (leftArch)
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
                        else if (rightArch)
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
                    }
                    else
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
                }
                else if (northConnectionValid && westConnectionValid)
                {
                    if (nearbyArchCount == 4)
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
                    else if (nearbyArchCount == 3)
                    {
                        //There is one valid aqueduct nearby that is not an arch
                        if (botArch)
                        {
                            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                            if (waterSourcesCount > 0)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = filled3NWE;
                            }
                            else
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = empty3NWE;
                            }
                        }
                        else if (rightArch)
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
                    }
                    else
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
                }
                else if (northConnectionValid && eastConnectionValid)
                {
                    if (nearbyArchCount == 4)
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
                    else if (nearbyArchCount == 3)
                    {
                        //There is one valid aqueduct nearby that is not an arch
                        if (leftArch)
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
                        else if (botArch)
                        {
                            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                            if (waterSourcesCount > 0)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = filled3NWE;
                            }
                            else
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = empty3NWE;
                            }
                        }
                    }
                    else
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
                }
                else if (southConnectionValid && westConnectionValid)
                {
                    if (nearbyArchCount == 4)
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
                    else if (nearbyArchCount == 3)
                    {
                        //There is one valid aqueduct nearby that is not an arch
                        if (topArch)
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
                        else if (rightArch)
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
                    }
                    else
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
                }
                else if (southConnectionValid && eastConnectionValid)
                {
                    if (nearbyArchCount == 4)
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
                    else if (nearbyArchCount == 3)
                    {
                        //There is one valid aqueduct nearby that is not an arch
                        if (leftArch)
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
                        else if (topArch)
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
                    }
                    else
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
                }
                else if (westConnectionValid && eastConnectionValid)
                {
                    if (nearbyArchCount == 4)
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
                    else if (nearbyArchCount == 3)
                    {
                        //There is one valid aqueduct nearby that is not an arch
                        if (topArch)
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
                        else if (botArch)
                        {
                            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                            if (waterSourcesCount > 0)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = filled3NWE;
                            }
                            else
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = empty3NWE;
                            }
                        }
                    }
                    else
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
                }
                else if (northConnectionValid)
                {
                    if (nearbyArchCount == 4)
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
                    else if (nearbyArchCount == 3)
                    {
                        //There is 1 valid aqueduct nearby that is not an arch
                        if (leftArch && rightArch)
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
                        else if (leftArch && botArch)
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
                        else if (rightArch && botArch)
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
                    }
                    else if (nearbyArchCount == 2)
                    {
                        //There are 2 valid aqueducts nearby that are not arches
                        if (leftArch)
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
                        else if (rightArch)
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
                        else if (botArch)
                        {
                            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                            if (waterSourcesCount > 0)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = filled3NWE;
                            }
                            else
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = empty3NWE;
                            }
                        }
                    }
                    else
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
                }
                else if (southConnectionValid)
                {
                    if (nearbyArchCount == 4)
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
                    else if (nearbyArchCount == 3)
                    {
                        //There is 1 valid aqueduct nearby that is not an arch
                        if (leftArch && rightArch)
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
                        else if (leftArch && topArch)
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
                        else if (rightArch && topArch)
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
                    }
                    else if (nearbyArchCount == 2)
                    {
                        //There are 2 valid aqueducts nearby that are not arches
                        if (leftArch)
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
                        else if (rightArch)
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
                        else if (topArch)
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
                    }
                    else
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
                }
                else if (westConnectionValid)
                {
                    if (nearbyArchCount == 4)
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
                    else if (nearbyArchCount == 3)
                    {
                        //There is 1 valid aqueduct nearby that is not an arch
                        if (topArch && botArch)
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
                        else if (rightArch && botArch)
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
                        else if (rightArch && topArch)
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
                    }
                    else if (nearbyArchCount == 2)
                    {
                        //There are 2 valid aqueducts nearby that are not arches
                        if (botArch)
                        {
                            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                            if (waterSourcesCount > 0)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = filled3NWE;
                            }
                            else
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = empty3NWE;
                            }
                        }
                        else if (rightArch)
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
                        else if (topArch)
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
                    }
                    else
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
                }
                else if (eastConnectionValid)
                {
                    if (nearbyArchCount == 4)
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
                    else if (nearbyArchCount == 3)
                    {
                        //There is 1 valid aqueduct nearby that is not an arch
                        if (topArch && botArch)
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
                        else if (leftArch && botArch)
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
                        else if (leftArch && topArch)
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
                    }
                    else if (nearbyArchCount == 2)
                    {
                        //There are 2 valid aqueducts nearby that are not arches
                        if (botArch)
                        {
                            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Buildings";
                            if (waterSourcesCount > 0)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = filled3NWE;
                            }
                            else
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = empty3NWE;
                            }
                        }
                        else if (leftArch)
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
                        else if (topArch)
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
                    }
                    else
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
                }
                else
                {
                    //None of the arch connections are valid
                    if (nearbyArchCount == 3)
                    {
                        if ((topArch && botArch && leftArch)
                            || (topArch && botArch && rightArch))
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
                        else if ((topArch && leftArch && rightArch)
                            || (botArch && leftArch && rightArch))
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
                    else if (nearbyArchCount == 2)
                    {
                        if (topArch && botArch)
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
                        else if (topArch && leftArch)
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
                        else if (topArch && rightArch)
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
                        else if (botArch && leftArch)
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
                        else if (botArch && rightArch)
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
                        else if (leftArch && rightArch)
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
                    else if (nearbyArchCount == 1)
                    {
                        if (topArch)
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
                        else if (botArch)
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
                        else if (leftArch)
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
                        else if (rightArch)
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
                    }
                }
            }
            else
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
     */
}
