using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//What if a road could hold an aqueduct object?  If I call clear on it, and it has an aqueduct stored, delete the aqueduct instead..
public class AqueductPlacement : MonoBehaviour {
    public Sprite possibleSprite2WE;
    public Sprite possibleSprite2WEArch;
    public Sprite possibleSprite2NS;
    public Sprite possibleSprite2NE;
    public Sprite possibleSprite2NW;
    public Sprite possibleSprite2SE;
    public Sprite possibleSprite2SW;
    public Sprite possibleSprite3NSE;
    public Sprite possibleSprite3NSW;
    public Sprite possibleSprite3NWE;
    public Sprite possibleSprite3SWE;
    public Sprite possibleSprite4;
    public Sprite impossibleSprite;
    public GameObject building;

    private GameObject world;
    private World myWorld;
    private GameObject roadAqueductIsOn;
    private bool validPlacement;

    private void Awake()
    {
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        roadAqueductIsOn = null;
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Escape))
        {
            //exits out of construction mode if the right mouse button or escape is clicked
            Destroy(gameObject);
        }
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = Mathf.RoundToInt(mousePos.x);
        mousePos.y = Mathf.RoundToInt(mousePos.y);
        mousePos.z = 0;

        bool valid = true;
        //Buildings cannot be placed outside of the map
        if (mousePos.x < 0)
        {
            mousePos.x = 0;
            valid = false;
        }
        if (mousePos.x > myWorld.mapSize - 1)
        {
            mousePos.x = myWorld.mapSize - 1;
            valid = false;
        }
        if (mousePos.y < 0)
        {
            mousePos.y = 0;
            valid = false;
        }
        if (mousePos.y > myWorld.mapSize - 1)
        {
            mousePos.y = myWorld.mapSize - 1;
            valid = false;
        }

        //Check if the structure is currently in a valid location
        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();

        //bool valid = true;
        if ((mousePos.x <= 0 && mousePos.x >= myWorld.mapSize && mousePos.y <= 0 && mousePos.y >= myWorld.mapSize))
        {
            valid = false;
        }
        //can't place an aqueduct on non-road constructs
        if (valid && structureArr[(int)mousePos.x, (int)mousePos.y] != null
            && (!myWorld.aqueductTerrain.Contains(structureArr[(int)mousePos.x, (int)mousePos.y].tag)))
        {
            valid = false;
        }
        if (valid && structureArr[(int)mousePos.x, (int)mousePos.y] != null
            && ((structureArr[(int)mousePos.x, (int)mousePos.y].tag == "Road")))
        {
            //There are a bunch of special checks in the case of building on top of a road
            valid = specialRoadCaseValidity(gameObject, structureArr);
            if (valid)
            {
                roadAqueductIsOn = structureArr[(int)mousePos.x, (int)mousePos.y];
            }
        }
        //can't place an aqueduct on non-clear land
        if (valid && terrainArr[(int)mousePos.x, (int)mousePos.y] != null
            && !myWorld.aqueductTerrain.Contains(terrainArr[(int)mousePos.x, (int)mousePos.y].tag))
        {
            valid = false;
        }
        if (valid && (structureArr[(int)mousePos.x, (int)mousePos.y] == null || structureArr[(int)mousePos.x, (int)mousePos.y].tag == "Road"))
        {
            validPlacement = true;
            //Border is green when it is possible to place a sprite in its current location
            updateAqueductPreview(gameObject, structureArr);
        }
        else
        {
            validPlacement = false;
            //Border turns red when it is impossible to place a sprite in its current location
            GetComponent<SpriteRenderer>().sprite = impossibleSprite;
        }
        
        //If the aqueduct is in a valid location and the left mouse is clicked, place it in the world
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(0) && validPlacement)
        {
            //when mouse down, can place road in any viable location the mouse moves to
            if (mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1)
            {
                //transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
                if (roadAqueductIsOn == null || checkNearbyArchCount(mousePos) == 0)
                {
                    GameObject aqueductObj = Instantiate(building, mousePos, Quaternion.identity) as GameObject;
                    if (roadAqueductIsOn != null)
                    {
                        aqueductObj.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
                        roadAqueductIsOn.GetComponent<RoadInformation>().setAqueduct(aqueductObj);
                    }
                    else
                    {
                        myWorld.constructNetwork.setConstructArr((int)mousePos.x, (int)mousePos.y, aqueductObj);
                    }
                    //aqueductObj.GetComponent<Aqueduct>().updateConnections();
                    //aqueductObj.GetComponent<Aqueduct>().updateNeighbors();
                    //TODO?: update neighbors for reservoirs
                }
            }

            if (mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1)
            {
                transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
            }
        }

        if (mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1)
        {
            transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
        }
        roadAqueductIsOn = null;
    }

    /**
     * Checks if an aqueduct can be built on a road
     * @param aqueduct the aqueduct we are checking validity of
     * @param structureArr the array of structures currently in the world
     */
    private bool specialRoadCaseValidity(GameObject aqueduct, GameObject[,] structureArr)
    {
        Vector2 aqueductPos = aqueduct.transform.position;
        if (structureArr[(int)aqueductPos.x, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y].tag == "Road"
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct() != null)
        {
            return false;
        }
        //checking for what gameobject appearance I want and add it to the structureArrArr
        int nearbyAqueductCount = 0;
        bool topAq = false;
        if ((int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<Aqueduct>() != null)
        {
            topAq = true;
            nearbyAqueductCount++;
        }
        bool botAq = false;
        if ((int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<Aqueduct>() != null)
        {
            botAq = true;
            nearbyAqueductCount++;
        }
        bool leftAq = false;
        if ((int)aqueductPos.x - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<Aqueduct>() != null)
        {
            leftAq = true;
            nearbyAqueductCount++;
        }
        bool rightAq = false;
        if ((int)aqueductPos.x + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<Aqueduct>() != null)
        {
            rightAq = true;
            nearbyAqueductCount++;
        }
        //If there are more than 2 adjacent aqueducts or 0 adjacent aqueducts, an aqueduct cannot be built over a road
        if (nearbyAqueductCount > 2 || nearbyAqueductCount == 0)
        {
            return false;
        }
        //Adjacent aqueducts need to be on opposite sides when going over a road
        if ((topAq && leftAq) || (topAq && rightAq) || (botAq && leftAq) || (botAq && rightAq))
        {
            return false;
        }

        int nearbyRoadCount = 0;
        int nearbyArchCount = 0;
        bool topRoad = false;
        if ((int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].tag == "Road")
        {
            topRoad = true;
            nearbyRoadCount++;
            if (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>() != null
                && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>().getAqueduct() != null)
            {
                nearbyArchCount++;
            }
        }
        bool botRoad = false;
        if ((int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].tag == "Road")
        {
            botRoad = true;
            nearbyRoadCount++;
            if (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>() != null
                && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>().getAqueduct() != null)
            {
                nearbyArchCount++;
            }
        }
        bool leftRoad = false;
        if ((int)aqueductPos.x - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].tag == "Road")
        {
            leftRoad = true;
            nearbyRoadCount++;
            if (structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>() != null
                && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct() != null)
            {
                nearbyArchCount++;
            }
        }
        bool rightRoad = false;
        if ((int)aqueductPos.x + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].tag == "Road")
        {
            rightRoad = true;
            nearbyRoadCount++;
            if (structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>() != null
                && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct() != null)
            {
                nearbyArchCount++;
            }
        }
        //There would be nowhere for the aqueduct to go
        if (nearbyRoadCount > 2 || nearbyArchCount > 0)
        {
            return false;
        }
        //If the nearby roads are not conflicting with where the aqueduct would continue, this is good
        if (nearbyAqueductCount == 1)
        {
            //check to see if the roads would be in the way of where the aqueduct can go
            if (leftAq && rightRoad)
            {
                return false;
            }
            else if (rightAq && leftRoad)
            {
                return false;
            }
            else if (topAq && botRoad)
            {
                return false;
            }
            else if (botAq && topRoad)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //Only thing left is when there are 2 nearby aqueducts on opposite sides of the road object and there are a max of 2 road objects around
        //There can be 0, 1, or 2 adjacent roads here.
        //Based on all of my other checks, the aqueducts have to be on opposite sides already so this should be valid
        return true;
    }

    /**
     * Gets the number of aqueduct arches adjacent to a particular position
     * @param aqueductPos the position to check the neighbors of
     */
    public int checkNearbyArchCount(Vector2 aqueductPos)
    {
        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        int nearbyArchCount = 0;
        if ((int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].tag == "Road"
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>() != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>().getAqueduct() != null)
        {
            nearbyArchCount++;
        }
        if ((int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].tag == "Road"
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>() != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>().getAqueduct() != null)
        {
            nearbyArchCount++;
        }
        if ((int)aqueductPos.x - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].tag == "Road"
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>() != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct() != null)
        {
            nearbyArchCount++;
        }
        if ((int)aqueductPos.x + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].tag == "Road"
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>() != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct() != null)
        {
            nearbyArchCount++;
        }
        return nearbyArchCount;
    }

    /**
     * Changes the preview appearance of the road attached to the player's mouse.
     * @param road is the road object being previewed
     * @param structureArr is a multidimensional array containing roads and buildings
     */
    private void updateAqueductPreview(GameObject road, GameObject[,] structureArr)
    {
        Vector2 aqueductPos = road.transform.position;
        
        GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;

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
        if (structureArr[(int)aqueductPos.x, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y].tag == "Road")
        {
            overRoad = true;
        }

        
        //Four connections
        if (north && south && west && east)
        {
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
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    {
                        northArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (botArch)
                {
                    if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    && ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    {
                        southArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (leftArch)
                {
                    if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        westArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (rightArch)
                {
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        eastArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            if (northConnectionValid && southConnectionValid && westConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite4;
            }
            else if (northConnectionValid && westConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite3NWE;
            }
            else if (southConnectionValid && westConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite3SWE;
            }
            else if (northConnectionValid && southConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite3NSE;
            }
            else if (northConnectionValid && southConnectionValid && westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite3NSW;
            }
            else if (northConnectionValid && southConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (northConnectionValid && westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NW;
            }
            else if (southConnectionValid && westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2SW;
            }
            else if (northConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (southConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
        }
        //Three connections
        else if (north && south && west)
        {
            int numValidConnections = 3;
            bool northArchConnectionValid = false;
            bool southArchConnectionValid = false;
            bool westArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    {
                        northArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (botArch)
                {
                    if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    && ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    {
                        southArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (leftArch)
                {
                    if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        westArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            if (northConnectionValid && southConnectionValid && westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite3NSW;
            }
            else if (northConnectionValid && southConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (northConnectionValid && westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NW;
            }
            else if (southConnectionValid && westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2SW;
            }
            else if (northConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (southConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
        }
        else if (north && south && east)
        {
            int numValidConnections = 3;
            bool northArchConnectionValid = false;
            bool southArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    {
                        northArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (botArch)
                {
                    if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    && ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    {
                        southArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (rightArch)
                {
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        eastArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool eastConnectionValid = (leftArch ? eastArchConnectionValid : true);
            if (northConnectionValid && southConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite3NSE;
            }
            else if (northConnectionValid && southConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (northConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NE;
            }
            else if (southConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2SE;
            }
            else if (northConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (southConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
        }
        else if (north && west && east)
        {
            int numValidConnections = 3;
            bool northArchConnectionValid = false;
            bool westArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    {
                        northArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (leftArch)
                {
                    if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        westArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (rightArch)
                {
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        eastArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool westConnectionValid = (botArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (leftArch ? eastArchConnectionValid : true);
            if (northConnectionValid && westConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite3NWE;
            }
            else if (northConnectionValid && westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NW;
            }
            else if (northConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NE;
            }
            else if (westConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
            else if (northConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
            else if (eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
        }
        else if (south && west && east)
        {
            int numValidConnections = 3;
            bool southArchConnectionValid = false;
            bool westArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                        && ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                        || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    {
                        southArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (leftArch)
                {
                    if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        westArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (rightArch)
                {
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        eastArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool southConnectionValid = (topArch ? southArchConnectionValid : true);
            bool westConnectionValid = (botArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (leftArch ? eastArchConnectionValid : true);
            if (southConnectionValid && westConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite3SWE;
            }
            else if (southConnectionValid && westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2SW;
            }
            else if (southConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2SE;
            }
            else if (westConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
            else if (southConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
            else if (eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
        }
        //Two connections
        else if (north && south)
        {
            int numValidConnections = 2;
            bool northArchConnectionValid = false;
            bool southArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    {
                        northArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (botArch)
                {
                    if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    && ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    {
                        southArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            if (northConnectionValid && southConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (northConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (southConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
        }
        else if (north && west)
        {
            int numValidConnections = 2;
            bool northArchConnectionValid = false;
            bool westArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    {
                        northArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (leftArch)
                {
                    if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        westArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool westConnectionValid = (botArch ? westArchConnectionValid : true);
            if (northConnectionValid && westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NW;
            }
            else if (northConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }

        }
        else if (north && east)
        {
            int numValidConnections = 2;
            bool northArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && ((int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || (int)aqueductPos.y + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    {
                        northArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (rightArch)
                {
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        eastArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool northConnectionValid = (topArch ? northArchConnectionValid : true);
            bool eastConnectionValid = (leftArch ? eastArchConnectionValid : true);
            if (northConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NE;
            }
            else if (northConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
        }
        else if (south && west)
        {
            int numValidConnections = 2;
            bool southArchConnectionValid = false;
            bool westArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (topArch)
                {
                    if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                        && ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                        || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    {
                        southArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (leftArch)
                {
                    if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        westArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool southConnectionValid = (topArch ? southArchConnectionValid : true);
            bool westConnectionValid = (botArch ? westArchConnectionValid : true);
            if (southConnectionValid && westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2SW;
            }
            else if (southConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
        }
        else if (south && east)
        {
            int numValidConnections = 2;
            bool southArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (botArch)
                {
                    if ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    && ((int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.y - 2 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    {
                        southArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (rightArch)
                {
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        eastArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool eastConnectionValid = (leftArch ? eastArchConnectionValid : true);
            if (southConnectionValid && eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2SE;
            }
            else if (southConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
        }
        else if (west && east)
        {
            int numValidConnections = 2;
            bool westArchConnectionValid = false;
            bool eastArchConnectionValid = false;
            if (nearbyArchCount > 0)
            {
                int numInvalidConnections = 0;
                if (leftArch)
                {
                    if ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x - 2 > 0 && structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        westArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                if (rightArch)
                {
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && ((int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || (int)aqueductPos.x + 2 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    {
                        eastArchConnectionValid = true;
                    }
                    else
                    {
                        numInvalidConnections++;
                    }
                }
                numValidConnections -= numInvalidConnections;
            }
            //have the new number of valid connections and have which aqueducts are invalid, if any
            bool westConnectionValid = (botArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (leftArch ? eastArchConnectionValid : true);
            if (westConnectionValid && eastConnectionValid)
            {
                if (overRoad)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WEArch;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
                }
            }
            else if (westConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
            else if (eastConnectionValid)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
        }
        //One connection
        else if (north)
        {
            if (nearbyReservoirCount > 0)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
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

                if (connectionPossible)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
                }
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
        }
        else if (south)
        {
            if (nearbyReservoirCount > 0)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
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

                if (connectionPossible)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
                }
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
        }
        else if (west)
        {
            if (nearbyReservoirCount > 0)
            {
                if (overRoad)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WEArch;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
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

                if (connectionPossible)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
                }
            }
            else
            {
                if (overRoad)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WEArch;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
                }
            }
        }
        else if (east)
        {
            if (nearbyReservoirCount > 0)
            {
                if (overRoad)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WEArch;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
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

                if (connectionPossible)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
                }
            }
            else
            {
                if (overRoad)
                {
                        gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WEArch;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
                }
            }
        }
    }
}
