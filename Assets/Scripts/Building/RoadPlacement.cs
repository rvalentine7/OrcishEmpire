using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Lets the player place roads in the world.
/// This class also gives visual feedback as to whether or not
/// road can be placed in a given location.It also updates
/// roads to connect visually when placed next to each other.
/// </summary>
public class RoadPlacement : BuildMode {
    private GameObject[,] structureArr;
    private GameObject[,] terrainArr;
    private Dictionary<Vector2, GameObject> tempRoads;
    private Vector2 buildStartLocation;
    private bool determinedBuildingDirection;
    private bool buildingInXDirection;
    private SpriteRenderer spriteRenderer;

    public Sprite possibleSprite;
    public Sprite possibleXRoadSprite;
    public Sprite possibleTRoadSprite;
    public Sprite possibleCornerRoadSprite;
    public Sprite impossibleSprite;
    public GameObject tempRoad;
    public GameObject road;
    public int buildingCost;


    /// <summary>
    /// Initializes the RoadPlacement class.
    /// </summary>
    void Start() {
        structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        tempRoads = new Dictionary<Vector2, GameObject>();
        buildStartLocation = new Vector2(-1, -1);
        determinedBuildingDirection = false;
        buildingInXDirection = false;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Moves the visual road icon around to tell whether a location is viable to build on.
    /// The player can build a road game object at the mouse location if it is in a viable build location.
    /// </summary>
    void Update() {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (tempRoads.Keys.Count > 0)
            {
                foreach (Vector2 tempRoadLocation in tempRoads.Keys)
                {
                    Destroy(tempRoads[tempRoadLocation]);
                }
                tempRoads = new Dictionary<Vector2, GameObject>();
                buildStartLocation.x = -1;
                buildStartLocation.y = -1;
                spriteRenderer.enabled = true;
            }
            else
            {
                //exits out of construction mode if the right mouse button or escape is clicked
                Destroy(gameObject);
            }
        }

        updateBuildMode();

        structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();

        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0) && buildStartLocation.x == -1 && buildStartLocation.y == -1)
        {
            buildStartLocation = mousePos;
            spriteRenderer.enabled = false;
        }
        
        if (buildStartLocation.x != -1) {
            //If the first is in the x direction, start x. at the end of x, go y
            //If the first is in the y direction, start y. at the end of y, go x
            int xSpaces = Mathf.RoundToInt(mousePos.x - buildStartLocation.x);
            int ySpaces = Mathf.RoundToInt(mousePos.y - buildStartLocation.y);
            if (determinedBuildingDirection
                && ((mousePos.x == buildStartLocation.x && mousePos.y == buildStartLocation.y)
                || (buildingInXDirection && mousePos.x == buildStartLocation.x)
                || (!buildingInXDirection && mousePos.y == buildStartLocation.y)))
            {
                determinedBuildingDirection = false;
            }
            if (!determinedBuildingDirection && (mousePos.x != buildStartLocation.x || mousePos.y != buildStartLocation.y))
            {
                determinedBuildingDirection = true;
                buildingInXDirection = Mathf.Abs(xSpaces) > Mathf.Abs(ySpaces);
            }

            int yAdjustment = 0;
            int xAdjustment = 0;
            if (!buildingInXDirection)
            {
                yAdjustment = ySpaces;
            }
            else
            {
                xAdjustment = xSpaces;
            }
            
            List<Vector2> tempRoadsToRemove = new List<Vector2>(tempRoads.Keys);
            //TODO: only update temp roads if something changes (optimization)
            int currentGold = myWorld.getCurrency();
            int totalRoadsToBuy = 0;
            int totalBuyableRoads = currentGold / buildingCost;
            //X direction
            for (int i = 0; i < Mathf.Abs(xSpaces); i++)
            {
                Vector2 tempRoadLocation = new Vector2(buildStartLocation.x + i * (xSpaces < 0 ? -1 : 1), buildStartLocation.y + yAdjustment);
                bool validCost = false;
                if ((buildingInXDirection && totalRoadsToBuy < totalBuyableRoads)
                    || (totalBuyableRoads > Mathf.Abs(ySpaces) + i))
                {
                    validCost = true;
                    totalRoadsToBuy++;
                }
                updateTempRoad(tempRoadLocation, validCost);
                tempRoadsToRemove.Remove(tempRoadLocation);
            }
            //Y direction
            for (int i = 0; i < Mathf.Abs(ySpaces); i++)
            {
                Vector2 tempRoadLocation = new Vector2(buildStartLocation.x + xAdjustment, buildStartLocation.y + i * (ySpaces < 0 ? -1 : 1));
                bool validCost = false;
                if (totalRoadsToBuy < totalBuyableRoads)
                {
                    validCost = true;
                    totalRoadsToBuy++;
                }
                updateTempRoad(tempRoadLocation, validCost);
                tempRoadsToRemove.Remove(tempRoadLocation);
            }
            bool valid = false;
            if (totalRoadsToBuy < totalBuyableRoads)
            {
                valid = true;
                totalRoadsToBuy += buildingCost;
            }
            updateTempRoad(mousePos, valid);
            tempRoadsToRemove.Remove(mousePos);
            foreach (Vector2 tempRoadLocation in tempRoadsToRemove)
            {
                GameObject tempRoadBeingRemoved = tempRoads[tempRoadLocation];
                tempRoads.Remove(tempRoadLocation);
                Destroy(tempRoadBeingRemoved);
            }
        }

        //Try to build any temp roads
        if (Input.GetMouseButtonUp(0) && tempRoads.Count > 0)
        {
            buildStartLocation.x = -1;
            buildStartLocation.y = -1;
            determinedBuildingDirection = false;
            spriteRenderer.enabled = true;

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //tempRoads.Add(gameObject.transform.position, gameObject);
                foreach (GameObject tempRoadObject in tempRoads.Values)
                {
                    Vector2 buildPos = tempRoadObject.transform.position;
                    //Sprites that do not work are named "ImpossibleRoad" whereas sprites that do work are "ViableRoad"
                    if (!tempRoadObject.GetComponent<SpriteRenderer>().sprite.name.Contains("Im"))
                    {
                        GameObject roadObj = Instantiate(road, buildPos, Quaternion.identity) as GameObject;
                        AudioSource buildAudioSource = GameObject.Find(World.BUILD_AUDIO).GetComponent<AudioSource>();
                        buildAudioSource.clip = buildAudioClip;
                        buildAudioSource.volume = myWorld.getSettingsMenu().getClickVolume();
                        buildAudioSource.Play();
                        myWorld.updateCurrency(-buildingCost);
                        structureArr = myWorld.constructNetwork.getConstructArr();
                        GameObject aqueductObj = null;
                        if (structureArr[(int)buildPos.x, (int)buildPos.y] != null
                            && structureArr[(int)buildPos.x, (int)buildPos.y].GetComponent<Aqueduct>() != null)
                        {
                            roadObj.GetComponent<RoadInformation>().setAqueduct(structureArr[(int)buildPos.x, (int)buildPos.y]);
                            aqueductObj = structureArr[(int)buildPos.x, (int)buildPos.y];
                        }
                        structureArr[(int)buildPos.x, (int)buildPos.y] = roadObj;
                        RoadInformation roadInfo = roadObj.GetComponent<RoadInformation>();
                        roadInfo.updateRoadConnection();
                        roadInfo.updateNeighbors();
                        //Update the aqueduct so it becomes an arch if needed
                        if (aqueductObj != null)
                        {
                            aqueductObj.GetComponent<Aqueduct>().updateConnections();
                        }
                    }
                }
            }

            foreach (Vector2 tempRoadLocation in tempRoads.Keys)
            {
                Destroy(tempRoads[tempRoadLocation]);
            }
            tempRoads = new Dictionary<Vector2, GameObject>();
        }
        if (mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1)
        {
            transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
            if (buildStartLocation.x == -1 && buildStartLocation.y == -1)
            {
                updateTempRoad(mousePos, buildingCost <= myWorld.getCurrency());
            }
        }
    }

    /// <summary>
    /// Updates the validity of a temp road object
    /// </summary>
    /// <param name="position">The position of the temp road object</param>
    /// <param name="validCost">Whether the building can be built with the current city gold count</param>
    private void updateTempRoad(Vector2 position, bool validCost)
    {
        GameObject tempRoadToUpdate;
        if (buildStartLocation.x == -1 && buildStartLocation.y == -1)
        {
            tempRoadToUpdate = gameObject;
        }
        else if (tempRoads.ContainsKey(position))
        {
            tempRoadToUpdate = tempRoads[position];
        }
        else
        {
            tempRoadToUpdate = Instantiate(tempRoad, position, Quaternion.identity) as GameObject;
            tempRoads.Add(position, tempRoadToUpdate);
        }

        bool valid = validCost;
        //Need enough currency to build the road
        if (myWorld.getCurrency() < buildingCost)
        {
            valid = false;
        }
        //Buildings cannot be placed outside of the map
        if (position.x < 0)
        {
            position.x = 0;
            valid = false;
        }
        if (position.x > myWorld.mapSize - 1)
        {
            position.x = myWorld.mapSize - 1;
            valid = false;
        }
        if (position.y < 0)
        {
            position.y = 0;
            valid = false;
        }
        if (position.y > myWorld.mapSize - 1)
        {
            position.y = myWorld.mapSize - 1;
            valid = false;
        }

        //Check if the structure is currently in a valid location
        //need to make sure there isn't a road/building already in the place where the road is currently located
        //check roadArr for the x/y coordinates of the object position to see if there is anything there...
        if ((position.x <= 0 && position.x >= myWorld.mapSize && position.y <= 0 && position.y >= myWorld.mapSize))
        {
            valid = false;
        }
        //can't place a road on other constructs
        if (valid && structureArr[(int)position.x, (int)position.y] != null
            && structureArr[(int)position.x, (int)position.y].GetComponent<Aqueduct>() == null)
        {
            valid = false;
        }
        if (valid && terrainArr[(int)position.x, (int)position.y] != null
            && !myWorld.buildableTerrain.Contains(terrainArr[(int)position.x, (int)position.y].tag))
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
        if (valid)
        {
            if (structureArr[(int)position.x, (int)position.y] == null)
            {
                //Border is green when it is possible to place a sprite in its current location
                updateRoadPreview(tempRoadToUpdate);
            }
            else
            {
                if (structureArr[(int)position.x, (int)position.y] != null
                    && structureArr[(int)position.x, (int)position.y].GetComponent<Aqueduct>() != null)
                {
                    Aqueduct aqueductClass = structureArr[(int)position.x, (int)position.y].GetComponent<Aqueduct>();
                    //If the aqueduct is connected to an aqueduct arch, it is not valid
                    //Checking to the left for an aqueduct arch
                    if (((int)position.x + 1 < myWorld.mapSize
                        && structureArr[(int)position.x + 1, (int)position.y] != null
                        && structureArr[(int)position.x + 1, (int)position.y].GetComponent<RoadInformation>() != null
                        && structureArr[(int)position.x + 1, (int)position.y].GetComponent<RoadInformation>().getAqueduct() != null)
                        || (structureArr[(int)position.x + 1, (int)position.y] != null
                        && structureArr[(int)position.x + 1, (int)position.y].GetComponent<Aqueduct>() != null
                        && tempRoads.ContainsKey(new Vector2((int)position.x + 1, (int)position.y))
                        && tempRoads[new Vector2((int)position.x + 1, (int)position.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Viable")))
                    {
                        valid = false;
                    }
                    //Checking to the right for an aqueduct arch
                    else if (((int)position.x - 1 > 0
                        && structureArr[(int)position.x - 1, (int)position.y] != null
                        && structureArr[(int)position.x - 1, (int)position.y].GetComponent<RoadInformation>() != null
                        && structureArr[(int)position.x - 1, (int)position.y].GetComponent<RoadInformation>().getAqueduct() != null)
                        || (structureArr[(int)position.x - 1, (int)position.y] != null
                        && structureArr[(int)position.x - 1, (int)position.y].GetComponent<Aqueduct>() != null
                        && tempRoads.ContainsKey(new Vector2((int)position.x - 1, (int)position.y))
                        && tempRoads[new Vector2((int)position.x - 1, (int)position.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Viable")))
                    {
                        valid = false;
                    }
                    //Checking to the top for an aqueduct arch
                    else if (((int)position.y + 1 < myWorld.mapSize
                        && structureArr[(int)position.x, (int)position.y + 1] != null
                        && structureArr[(int)position.x, (int)position.y + 1].GetComponent<RoadInformation>() != null
                        && structureArr[(int)position.x, (int)position.y + 1].GetComponent<RoadInformation>().getAqueduct() != null)
                        || (structureArr[(int)position.x, (int)position.y + 1] != null
                        && structureArr[(int)position.x, (int)position.y + 1].GetComponent<Aqueduct>() != null
                        && tempRoads.ContainsKey(new Vector2((int)position.x, (int)position.y + 1))
                        && tempRoads[new Vector2((int)position.x, (int)position.y + 1)].GetComponent<SpriteRenderer>().sprite.name.Contains("Viable")))
                    {
                        valid = false;
                    }
                    //Checking to the bot for an aqueduct arch
                    else if (((int)position.y - 1 > 0
                        && structureArr[(int)position.x, (int)position.y - 1] != null
                        && structureArr[(int)position.x, (int)position.y - 1].GetComponent<RoadInformation>() != null
                        && structureArr[(int)position.x, (int)position.y - 1].GetComponent<RoadInformation>().getAqueduct() != null)
                        || (structureArr[(int)position.x, (int)position.y - 1] != null
                        && structureArr[(int)position.x, (int)position.y - 1].GetComponent<Aqueduct>() != null
                        && tempRoads.ContainsKey(new Vector2((int)position.x, (int)position.y - 1))
                        && tempRoads[new Vector2((int)position.x, (int)position.y - 1)].GetComponent<SpriteRenderer>().sprite.name.Contains("Viable")))
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
                        //Border is green when it is possible to place a sprite in its current location
                        updateRoadPreview(tempRoadToUpdate);
                    }
                }
                else
                {
                    valid = false;
                }
            }
        }
        if (!valid)
        {
            //Border turns red when it is impossible to place a sprite in its current location
            tempRoadToUpdate.GetComponent<SpriteRenderer>().sprite = impossibleSprite;
        }
    }

    /// <summary>
    /// Changes the preview appearance of the road
    /// </summary>
    /// <param name="road">the road object being previewed</param>
    void updateRoadPreview(GameObject road)
    {
        Vector2 roadPos = road.transform.position;
        //Checking for roads
        int nearbyRoadCount = 0;
        bool top = false;
        if ((int)roadPos.y + 1 < myWorld.mapSize && (tempRoads.ContainsKey(new Vector2(roadPos.x, roadPos.y + 1))
            || (roadPos.x == mousePos.x && roadPos.y + 1 == mousePos.y)
            || (structureArr[(int)roadPos.x, (int)roadPos.y + 1] != null
            && (structureArr[(int)roadPos.x, (int)roadPos.y + 1].tag.Equals(World.ROAD)
            || structureArr[(int)roadPos.x, (int)roadPos.y + 1].tag.Equals(World.HIGH_BRIDGE)
            || structureArr[(int)roadPos.x, (int)roadPos.y + 1].tag.Equals(World.LOW_BRIDGE)))))
        {
            top = true;
            nearbyRoadCount++;
        }
        bool bot = false;
        if ((int)roadPos.y - 1 > 0 && (tempRoads.ContainsKey(new Vector2(roadPos.x, roadPos.y - 1))
            || (roadPos.x == mousePos.x && roadPos.y - 1 == mousePos.y)
            || (structureArr[(int)roadPos.x, (int)roadPos.y - 1] != null
            && (structureArr[(int)roadPos.x, (int)roadPos.y - 1].tag.Equals(World.ROAD)
            || structureArr[(int)roadPos.x, (int)roadPos.y - 1].tag.Equals(World.HIGH_BRIDGE)
            || structureArr[(int)roadPos.x, (int)roadPos.y - 1].tag.Equals(World.LOW_BRIDGE)))))
        {
            bot = true;
            nearbyRoadCount++;
        }
        bool left = false;
        if ((int)roadPos.x - 1 > 0 && (tempRoads.ContainsKey(new Vector2(roadPos.x - 1, roadPos.y))
            || (roadPos.x - 1 == mousePos.x && roadPos.y == mousePos.y)
            || (structureArr[(int)roadPos.x - 1, (int)roadPos.y] != null
            && (structureArr[(int)roadPos.x - 1, (int)roadPos.y].tag.Equals(World.ROAD)
            || structureArr[(int)roadPos.x - 1, (int)roadPos.y].tag.Equals(World.HIGH_BRIDGE)
            || structureArr[(int)roadPos.x - 1, (int)roadPos.y].tag.Equals(World.LOW_BRIDGE)))))
        {
            left = true;
            nearbyRoadCount++;
        }
        bool right = false;
        if ((int)roadPos.x + 1 < myWorld.mapSize && (tempRoads.ContainsKey(new Vector2(roadPos.x + 1, roadPos.y))
            || (roadPos.x + 1 == mousePos.x && roadPos.y == mousePos.y)
            || (structureArr[(int)roadPos.x + 1, (int)roadPos.y] != null
            && (structureArr[(int)roadPos.x + 1, (int)roadPos.y].tag.Equals(World.ROAD)
            || structureArr[(int)roadPos.x + 1, (int)roadPos.y].tag.Equals(World.HIGH_BRIDGE)
            || structureArr[(int)roadPos.x + 1, (int)roadPos.y].tag.Equals(World.LOW_BRIDGE)))))
        {
            right = true;
            nearbyRoadCount++;
        }
        //Checking for stairs
        if (top == false && (int)roadPos.y + 1 < myWorld.mapSize && terrainArr[(int)roadPos.x, (int)roadPos.y + 1] != null
            && terrainArr[(int)roadPos.x, (int)roadPos.y + 1].tag.Equals(World.STAIRS))
        {
            top = true;
            nearbyRoadCount++;
        }
        if (bot == false && (int)roadPos.y - 1 > 0 && terrainArr[(int)roadPos.x, (int)roadPos.y - 1] != null
            && terrainArr[(int)roadPos.x, (int)roadPos.y - 1].tag.Equals(World.STAIRS))
        {
            bot = true;
            nearbyRoadCount++;
        }
        if (left == false && (int)roadPos.x - 1 > 0 && terrainArr[(int)roadPos.x - 1, (int)roadPos.y] != null
            && terrainArr[(int)roadPos.x - 1, (int)roadPos.y].tag.Equals(World.STAIRS))
        {
            left = true;
            nearbyRoadCount++;
        }
        if (right == false && (int)roadPos.x + 1 < myWorld.mapSize && terrainArr[(int)roadPos.x + 1, (int)roadPos.y] != null
            && terrainArr[(int)roadPos.x + 1, (int)roadPos.y].tag.Equals(World.STAIRS))
        {
            right = true;
            nearbyRoadCount++;
        }

        //no nearby roads
        if (nearbyRoadCount == 0)
        {
            road.GetComponent<SpriteRenderer>().sprite = possibleSprite;
            road.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        
        //straight road
        if (nearbyRoadCount == 1)
        {
            road.GetComponent<SpriteRenderer>().sprite = possibleSprite;
            if (top || bot)
            {
                //change the z rotation
                road.transform.localEulerAngles = new Vector3(0, 0, 90);
            }
            else
            {
                road.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        //straight or corner; depends on if nearby roads are opposite or diagonal
        if (nearbyRoadCount == 2)
        {
            if (right && left)
            {
                road.GetComponent<SpriteRenderer>().sprite = possibleSprite;
                road.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else if (top && bot)
            {
                road.GetComponent<SpriteRenderer>().sprite = possibleSprite;
                road.transform.localEulerAngles = new Vector3(0, 0, 90);
            }
            else if (top)
            {
                road.GetComponent<SpriteRenderer>().sprite = possibleCornerRoadSprite;
                //only needs to be rotated if the second connecting road is on the right
                if (right)
                {
                    road.transform.localEulerAngles = new Vector3(0, 0, 270);
                }
                else
                {
                    road.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
            }
            else if (bot)
            {
                road.GetComponent<SpriteRenderer>().sprite = possibleCornerRoadSprite;
                if (right)
                {
                    road.transform.localEulerAngles = new Vector3(0, 0, 180);
                }
                else
                {
                    road.transform.localEulerAngles = new Vector3(0, 0, 90);
                }
            }
        }

        //T road
        if (nearbyRoadCount == 3)
        {
            if (top && bot)
            {
                road.GetComponent<SpriteRenderer>().sprite = possibleTRoadSprite;
                if (right)
                {
                    road.transform.localEulerAngles = new Vector3(0, 0, 270);
                }
                else
                {
                    road.transform.localEulerAngles = new Vector3(0, 0, 90);
                }
            }
            else if (right && left)
            {
                road.GetComponent<SpriteRenderer>().sprite = possibleTRoadSprite;
                //starting rotation is if the third connection is on top, therefore only need to rotation in this
                // case if last connection is bot
                if (bot)
                {
                    road.transform.localEulerAngles = new Vector3(0, 0, 180);
                }
                else
                {
                    road.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
            }
        }

        //crossroad
        if (nearbyRoadCount == 4)
        {
            road.GetComponent<SpriteRenderer>().sprite = possibleXRoadSprite;
        }
    }
}
