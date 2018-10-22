using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/**
 * Lets the player place roads in the world.
 * This class also gives visual feedback as to whether or not
 * a road can be placed in a given location.  It also updates
 * roads to connect visually when placed next to each other.
 */
public class RoadPlacement : MonoBehaviour {
    private bool validPlacement;
    private GameObject world;
    private World myWorld;
    //private GameObject aqueduct;
    public Sprite possibleSprite;
    public Sprite possibleXRoadSprite;
    public Sprite possibleTRoadSprite;
    public Sprite possibleCornerRoadSprite;
    public Sprite impossibleSprite;
    public GameObject road;
    public int buildingCost;
    

	/**
     * Initializes the RoadPlacement class.
     */
	void Start () {
        validPlacement = true;
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        //aqueduct = null;
    }
	
	/**
     * Moves the visual road icon around to tell whether a location is viable to build on.
     * The player can build a road game object at the mouse location if it is in a viable build location.
     */
	void Update () {
        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Escape))
        {
            //exits out of construction mode if the right mouse button or escape is clicked
            Destroy(gameObject);
        }

        //GameObject world = GameObject.Find("WorldInformation");
        //World myWorld = world.GetComponent<World>();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = Mathf.RoundToInt(mousePos.x);
        mousePos.y = Mathf.RoundToInt(mousePos.y);
        mousePos.z = 0;

        bool valid = true;
        //Need enough currency to build the road
        if (myWorld.getCurrency() < buildingCost)
        {
            valid = false;
        }
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
        //need to make sure there isn't a road/building already in the place where the road is currently located
        //check roadArr for the x/y coordinates of the mousePos to see if there is anything there...
        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();

        //bool valid = true;
        if ((mousePos.x <= 0 && mousePos.x >= myWorld.mapSize && mousePos.y <= 0 && mousePos.y >= myWorld.mapSize))
        {
            valid = false;
        }
        //can't place a road on other constructs
        if (valid && structureArr[(int)mousePos.x, (int)mousePos.y] != null
            && (!myWorld.buildableTerrain.Contains(structureArr[(int) mousePos.x, (int) mousePos.y].tag))
            && structureArr[(int)mousePos.x, (int)mousePos.y].GetComponent<Aqueduct>() == null)
        {
            valid = false;
        }
        if (valid && terrainArr[(int)mousePos.x, (int)mousePos.y] != null
            && !myWorld.buildableTerrain.Contains(terrainArr[(int)mousePos.x, (int)mousePos.y].tag))
        {
            valid = false;
        }
        
        /*
         If the location is an aqueduct, check if there are any connected aqueduct arches (roads holding an aqueduct object (getAqueduct() != null)
         Checks to see if valid:
         If there is a connected aqueduct arch, it is not valid
         If the aqueduct has more than 2 connections, it is not valid
         If the aqueduct has 2 connections and they are not North-South or East-West, it is not valid

         When placing the road, it will need to add the aqueduct to the road and replace the aqueduct with a road in the construction network
         */
        if (valid && (structureArr[(int) mousePos.x, (int) mousePos.y] == null))
        {
            validPlacement = true;
            //Border is green when it is possible to place a sprite in its current location
            updateRoadPreview(gameObject, structureArr);
        }
        else
        {
            if (structureArr[(int)mousePos.x, (int)mousePos.y] != null
                && structureArr[(int)mousePos.x, (int)mousePos.y].GetComponent<Aqueduct>() != null)
            {
                Aqueduct aqueductClass = structureArr[(int)mousePos.x, (int)mousePos.y].GetComponent<Aqueduct>();
                //If the aqueduct is connected to an aqueduct arch, it is not valid
                //Checking to the left for an aqueduct arch
                if ((int)mousePos.x + 1 < myWorld.mapSize
                    && structureArr[(int)mousePos.x + 1, (int)mousePos.y] != null
                    && structureArr[(int)mousePos.x + 1, (int)mousePos.y].GetComponent<RoadInformation>() != null
                    && structureArr[(int)mousePos.x + 1, (int)mousePos.y].GetComponent<RoadInformation>().getAqueduct() != null)
                {
                    valid = false;
                }
                //Checking to the right for an aqueduct arch
                else if ((int)mousePos.x - 1 > 0
                    && structureArr[(int)mousePos.x - 1, (int)mousePos.y] != null
                    && structureArr[(int)mousePos.x - 1, (int)mousePos.y].GetComponent<RoadInformation>() != null
                    && structureArr[(int)mousePos.x - 1, (int)mousePos.y].GetComponent<RoadInformation>().getAqueduct() != null)
                {
                    valid = false;
                }
                //Checking to the top for an aqueduct arch
                else if ((int)mousePos.y + 1 < myWorld.mapSize
                    && structureArr[(int)mousePos.x, (int)mousePos.y + 1] != null
                    && structureArr[(int)mousePos.x, (int)mousePos.y + 1].GetComponent<RoadInformation>() != null
                    && structureArr[(int)mousePos.x, (int)mousePos.y + 1].GetComponent<RoadInformation>().getAqueduct() != null)
                {
                    valid = false;
                }
                //Checking to the bot for an aqueduct arch
                else if ((int)mousePos.y - 1 > 0
                    && structureArr[(int)mousePos.x, (int)mousePos.y - 1] != null
                    && structureArr[(int)mousePos.x, (int)mousePos.y - 1].GetComponent<RoadInformation>() != null
                    && structureArr[(int)mousePos.x, (int)mousePos.y - 1].GetComponent<RoadInformation>().getAqueduct() != null)
                {
                    valid = false;
                }
                //If the aqueduct has more than 2 connections, it is not valid
                if (aqueductClass.getConnections().Count > 2)
                {
                    valid = false;
                }
                //If the aqueduct has 2 connections and they are not North-South or East-West, it is not valid
                if (aqueductClass.getConnections().Count == 2)
                {
                    bool validArch = false;
                    if (aqueductClass.getTopConnection() != null && aqueductClass.getBotConnection() != null)
                    {
                        validArch = true;
                    }
                    if (aqueductClass.getLeftConnection() != null && aqueductClass.getRightConnection() != null)
                    {
                        validArch = true;
                    }
                    if (!validArch)
                    {
                        valid = false;
                    }
                }

                if (valid)
                {
                    validPlacement = true;
                    //Border is green when it is possible to place a sprite in its current location
                    updateRoadPreview(gameObject, structureArr);
                }
                else
                {
                    validPlacement = false;
                }
            }
            else
            {
                valid = false;
                validPlacement = false;
            }
            if (!valid)
            {
                //Border turns red when it is impossible to place a sprite in its current location
                GetComponent<SpriteRenderer>().sprite = impossibleSprite;
            }
        }

        //If the road is in a valid location and the left mouse is clicked, place it in the world
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(0) && validPlacement)
        {
            //when mouse down, can place road in any viable location the mouse moves to
            if (mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1)
            {
                transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
                GameObject roadObj = Instantiate(road, mousePos, Quaternion.identity) as GameObject;
                myWorld.updateCurrency(-buildingCost);
                structureArr = myWorld.constructNetwork.getConstructArr();
                GameObject aqueductObj = null;
                if (structureArr[(int)mousePos.x, (int)mousePos.y] != null
                    && structureArr[(int)mousePos.x, (int)mousePos.y].GetComponent<Aqueduct>() != null)
                {
                    roadObj.GetComponent<RoadInformation>().setAqueduct(structureArr[(int)mousePos.x, (int)mousePos.y]);
                    aqueductObj = structureArr[(int)mousePos.x, (int)mousePos.y];
                }
                structureArr[(int)mousePos.x, (int)mousePos.y] = roadObj;
                RoadInformation roadInfo = roadObj.GetComponent<RoadInformation>();
                roadInfo.updateRoadConnection();
                roadInfo.updateNeighbors();
                //Update the aqueduct so it becomes an arch if needed
                if (aqueductObj != null)
                {
                    aqueductObj.GetComponent<Aqueduct>().updateConnections();
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
        }
        if (mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1)
        {
            transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
        }
    }

    /**
     * Changes the preview appearance of the road attached to the player's mouse.
     * @param road is the road object being previewed
     * @param structureArr is a multidimensional array containing roads and buildings
     */
    void updateRoadPreview(GameObject road, GameObject[,] structureArr)
    {
        //GameObject myCamera = GameObject.Find("Main Camera");
        //Controls myControls = myCamera.GetComponent<Controls>();
        Vector2 roadPos = road.transform.position;
        //pretty much the same as the part up above
        //also add structureArr as something that needs to be passed in?

        //checking for what gameobject appearance I want and add it to the buildArr

        //one way to remove repetition would be to have a method for the following which returns
        // a data structure with a length of 5 or a data structure that resizes itself. the first
        // location in the structure would be how many nearby roads followed by all of the sides
        // my current road object is surrounded on
        int nearbyRoadCount = 0;
        bool top = false;
        if ((int)roadPos.y + 1 < myWorld.mapSize && structureArr[(int)roadPos.x, (int)roadPos.y + 1] != null
            && structureArr[(int)roadPos.x, (int)roadPos.y + 1].tag == "Road")
        {
            top = true;
            nearbyRoadCount++;
        }
        bool bot = false;
        if ((int)roadPos.y - 1 > 0 && structureArr[(int)roadPos.x, (int)roadPos.y - 1] != null
            && structureArr[(int)roadPos.x, (int)roadPos.y - 1].tag == "Road")
        {
            bot = true;
            nearbyRoadCount++;
        }
        bool left = false;
        if ((int)roadPos.x - 1 > 0 && structureArr[(int)roadPos.x - 1, (int)roadPos.y] != null
            && structureArr[(int)roadPos.x - 1, (int)roadPos.y].tag == "Road")
        {
            left = true;
            nearbyRoadCount++;
        }
        bool right = false;
        if ((int)roadPos.x + 1 < myWorld.mapSize && structureArr[(int)roadPos.x + 1, (int)roadPos.y] != null
            && structureArr[(int)roadPos.x + 1, (int)roadPos.y].tag == "Road")
        {
            right = true;
            nearbyRoadCount++;
        }


        //no nearby roads
        if (nearbyRoadCount == 0)
        {
            GetComponent<SpriteRenderer>().sprite = possibleSprite;
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        
        //straight road
        if (nearbyRoadCount == 1)
        {
            GetComponent<SpriteRenderer>().sprite = possibleSprite;
            if (top || bot)
            {
                //change the z rotation
                transform.localEulerAngles = new Vector3(0, 0, 90);
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        //straight or corner; depends on if nearby roads are opposite or diagonal
        if (nearbyRoadCount == 2)
        {
            if (right && left)
            {
                GetComponent<SpriteRenderer>().sprite = possibleSprite;
            }
            else if (top && bot)
            {
                GetComponent<SpriteRenderer>().sprite = possibleSprite;
                transform.localEulerAngles = new Vector3(0, 0, 90);
            }
            else if (top)
            {
                GetComponent<SpriteRenderer>().sprite = possibleCornerRoadSprite;
                //only needs to be rotated if the second connecting road is on the right
                if (right)
                {
                    transform.localEulerAngles = new Vector3(0, 0, 270);
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, 0, 0);
                }
            }
            else if (bot)
            {
                GetComponent<SpriteRenderer>().sprite = possibleCornerRoadSprite;
                if (right)
                {
                    transform.localEulerAngles = new Vector3(0, 0, 180);
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, 0, 90);
                }
            }
        }

        //T road
        if (nearbyRoadCount == 3)
        {
            if (top && bot)
            {
                GetComponent<SpriteRenderer>().sprite = possibleTRoadSprite;
                if (right)
                {
                    transform.localEulerAngles = new Vector3(0, 0, 270);
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, 0, 90);
                }
            }
            else if (right && left)
            {
                GetComponent<SpriteRenderer>().sprite = possibleTRoadSprite;
                //starting rotation is if the third connection is on top, therefore only need to rotation in this
                // case if last connection is bot
                if (bot)
                {
                    transform.localEulerAngles = new Vector3(0, 0, 180);
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, 0, 0);
                }
            }
        }

        //crossroad
        if (nearbyRoadCount == 4)
        {
            GetComponent<SpriteRenderer>().sprite = possibleXRoadSprite;
        }
    }

    /**
     * Gets the aqueduct over this road object
     * @return the aqueduct over this road object if there is one, null otherwise
     */
    public GameObject getAqueduct()
    {
        //return this.aqueduct;
        return null;
    }
}
