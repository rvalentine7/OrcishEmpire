using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Places an aqueduct in the world
/// </summary>
public class AqueductPlacement : BuildMode {
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
    public int buildingCost;
    public GameObject tempAqueduct;
    
    private Dictionary<Vector2, GameObject> roadsUnderTempAqueducts;
    private bool validPlacement;
    private Dictionary<Vector2, GameObject> tempAqueducts;
    private Vector2 buildStartLocation;
    private SpriteRenderer spriteRenderer;
    private bool determinedBuildingDirection;
    private bool buildingInXDirection;
    private GameObject[,] structureArr;
    private GameObject[,] terrainArr;

    /// <summary>
    /// Initialization
    /// </summary>
    private void Start()
    {
        tempAqueducts = new Dictionary<Vector2, GameObject>();
        roadsUnderTempAqueducts = new Dictionary<Vector2, GameObject>();
        buildStartLocation = new Vector2(-1, -1);
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        determinedBuildingDirection = false;
        buildingInXDirection = false;
        structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (tempAqueducts.Keys.Count > 0)
            {
                foreach (Vector2 tempAqueductLocation in tempAqueducts.Keys)
                {
                    Destroy(tempAqueducts[tempAqueductLocation]);
                }
                tempAqueducts = new Dictionary<Vector2, GameObject>();
                roadsUnderTempAqueducts = new Dictionary<Vector2, GameObject>();
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

        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0) && buildStartLocation.x == -1
            && mousePos.x >= 0 && mousePos.x <= myWorld.mapSize && mousePos.y >= 0 && mousePos.y <= myWorld.mapSize)
        {
            spriteRenderer.enabled = false;
            buildStartLocation = mousePos;
        }

        if (buildStartLocation.x != -1)
        {
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

            //Need to remove temp aqueducts before checking validities because they can affect the validity
            List<Vector2> tempAqueductsToRemove = new List<Vector2>(tempAqueducts.Keys);
            for (int i = 0; i < Mathf.Abs(xSpaces); i++)
            {
                Vector2 tempAqueductLocation = new Vector2(buildStartLocation.x + i * (xSpaces < 0 ? -1 : 1), buildStartLocation.y + yAdjustment);
                tempAqueductsToRemove.Remove(tempAqueductLocation);
            }
            for (int i = 0; i < Mathf.Abs(ySpaces); i++)
            {
                Vector2 tempAqueductLocation = new Vector2(buildStartLocation.x + xAdjustment, buildStartLocation.y + i * (ySpaces < 0 ? -1 : 1));
                tempAqueductsToRemove.Remove(tempAqueductLocation);
            }
            tempAqueductsToRemove.Remove(mousePos);
            foreach (Vector2 tempAqueductLocation in tempAqueductsToRemove)
            {
                GameObject tempAqueductBeingRemoved = tempAqueducts[tempAqueductLocation];
                tempAqueducts.Remove(tempAqueductLocation);
                if (roadsUnderTempAqueducts.ContainsKey(tempAqueductLocation))
                {
                    roadsUnderTempAqueducts.Remove(tempAqueductLocation);
                }
                Destroy(tempAqueductBeingRemoved);
            }
            roadsUnderTempAqueducts = new Dictionary<Vector2, GameObject>();

            //TODO: only update temp aqueducts if something changes (optimization)
            int currentGold = myWorld.getCurrency();
            int totalAqueductsToBuy = 0;
            int totalBuyableAqueducts = currentGold / buildingCost;
            structureArr = myWorld.constructNetwork.getConstructArr();
            terrainArr = myWorld.terrainNetwork.getTerrainArr();
            List<GameObject> validTempAqueducts = new List<GameObject>();
            //X direction
            for (int i = 0; i < Mathf.Abs(xSpaces); i++)
            {
                Vector2 tempAqueductLocation = new Vector2(buildStartLocation.x + i * (xSpaces < 0 ? -1 : 1), buildStartLocation.y + yAdjustment);
                bool validCost = false;
                if ((buildingInXDirection && totalAqueductsToBuy < totalBuyableAqueducts)
                    || (totalBuyableAqueducts > Mathf.Abs(ySpaces) + i))
                {
                    validCost = true;
                    totalAqueductsToBuy++;
                }
                if (checkValidity(tempAqueductLocation, validCost))
                {
                    validTempAqueducts.Add(tempAqueducts[tempAqueductLocation]);
                }
            }
            //Y direction
            for (int i = 0; i < Mathf.Abs(ySpaces); i++)
            {
                Vector2 tempAqueductLocation = new Vector2(buildStartLocation.x + xAdjustment, buildStartLocation.y + i * (ySpaces < 0 ? -1 : 1));
                bool validCost = false;
                if (totalAqueductsToBuy < totalBuyableAqueducts)
                {
                    validCost = true;
                    totalAqueductsToBuy++;
                }
                if (checkValidity(tempAqueductLocation, validCost))
                {
                    validTempAqueducts.Add(tempAqueducts[tempAqueductLocation]);
                }
            }
            bool valid = false;
            if (totalAqueductsToBuy < totalBuyableAqueducts)
            {
                valid = true;
                totalAqueductsToBuy += buildingCost;
            }
            if (checkValidity(mousePos, valid))
            {
                validTempAqueducts.Add(tempAqueducts[mousePos]);
            }

            foreach (GameObject tempAqueductToUpdate in tempAqueducts.Values)
            {
                if (validTempAqueducts.Contains(tempAqueductToUpdate))
                {
                    //Border is green when it is possible to place a sprite in its current location
                    updateAqueductPreview(tempAqueductToUpdate, structureArr);
                }
                else
                {
                    //Border turns red when it is impossible to place a sprite in its current location
                    tempAqueductToUpdate.GetComponent<SpriteRenderer>().sprite = impossibleSprite;
                }
            }
        }

        //If the aqueduct is in a valid location and the left mouse is clicked, place it in the world
        if (Input.GetMouseButtonUp(0))
        {
            spriteRenderer.enabled = true;
            buildStartLocation.x = -1;
            buildStartLocation.y = -1;
            determinedBuildingDirection = false;

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                foreach (GameObject tempAqueductToPlace in tempAqueducts.Values)
                {
                    if (!tempAqueductToPlace.GetComponent<SpriteRenderer>().sprite.name.Contains("Im"))
                    {
                        Vector2 positionToBuild = tempAqueductToPlace.transform.position;
                        GameObject roadAqueductIsOn = roadsUnderTempAqueducts.ContainsKey(positionToBuild) ? roadsUnderTempAqueducts[positionToBuild] : null;
                        GameObject aqueductObj = Instantiate(building, positionToBuild, Quaternion.identity) as GameObject;
                        myWorld.updateCurrency(-buildingCost);
                        if (roadAqueductIsOn != null)
                        {
                            aqueductObj.GetComponent<SpriteRenderer>().sortingLayerName = "TallBuildings";
                            roadAqueductIsOn.GetComponent<RoadInformation>().setAqueduct(aqueductObj);
                        }
                        else
                        {
                            myWorld.constructNetwork.setConstructArr((int)positionToBuild.x, (int)positionToBuild.y, aqueductObj);
                        }
                    }
                }
            }
            foreach (GameObject tempAqueductToDestroy in tempAqueducts.Values)
            {
                Destroy(tempAqueductToDestroy);
            }
            tempAqueducts = new Dictionary<Vector2, GameObject>();
            roadsUnderTempAqueducts = new Dictionary<Vector2, GameObject>();
        }

        if (mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1)
        {
            transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
            if (checkValidity(mousePos, buildingCost <= myWorld.getCurrency()))
            {
                //Border is green when it is possible to place a sprite in its current location
                updateAqueductPreview(gameObject, structureArr);
            }
            else
            {
                //Border turns red when it is impossible to place a sprite in its current location
                spriteRenderer.sprite = impossibleSprite;
            }
        }
    }

    /// <summary>
    /// Checks the validity of placing an aqueduct in a position in the world
    /// </summary>
    /// <param name="position">The position to check the validity of</param>
    /// <param name="validCost">Whether the player has the gold to place the aqueduct</param>
    /// <returns>Whether the aqueduct is valid</returns>
    private bool checkValidity(Vector2 position, bool validCost)
    {
        GameObject tempAqueductToUpdate;
        if (buildStartLocation.x == -1 && buildStartLocation.y == -1)
        {
            tempAqueductToUpdate = gameObject;
        }
        else if (tempAqueducts.ContainsKey(position))
        {
            tempAqueductToUpdate = tempAqueducts[position];
        }
        else
        {
            tempAqueductToUpdate = Instantiate(tempAqueduct, position, Quaternion.identity) as GameObject;
            tempAqueducts.Add(position, tempAqueductToUpdate);
        }

        bool valid = validCost;

        //Buildings cannot be placed outside of the map
        if (position.x <= 0)
        {
            position.x = 0;
            valid = false;
        }
        if (position.x > myWorld.mapSize - 1)
        {
            position.x = myWorld.mapSize - 1;
            valid = false;
        }
        if (position.y <= 0)
        {
            position.y = 0;
            valid = false;
        }
        if (position.y > myWorld.mapSize - 1)
        {
            position.y = myWorld.mapSize - 1;
            valid = false;
        }

        //bool valid = true;
        if (valid && (position.x <= 0 && position.x >= myWorld.mapSize && position.y <= 0 && position.y >= myWorld.mapSize))
        {
            valid = false;
        }
        //can't place an aqueduct on non-road constructs
        if (valid && structureArr[(int)position.x, (int)position.y] != null
            && (!myWorld.aqueductTerrain.Contains(structureArr[(int)position.x, (int)position.y].tag)))
        {
            valid = false;
        }
        if (valid && !roadsUnderTempAqueducts.ContainsKey(position) && structureArr[(int)position.x, (int)position.y] != null
            && (structureArr[(int)position.x, (int)position.y].tag.Equals(World.ROAD)))
        {
            //There are a bunch of special checks in the case of building on top of a road
            valid = specialRoadCaseValidity(tempAqueductToUpdate, structureArr);
            if (valid)
            {
                roadsUnderTempAqueducts.Add(position, structureArr[(int)position.x, (int)position.y]);
            }
        }
        //can't place an aqueduct on non-clear land
        if (valid && terrainArr[(int)position.x, (int)position.y] != null
            && !myWorld.aqueductTerrain.Contains(terrainArr[(int)position.x, (int)position.y].tag))
        {
            valid = false;
        }
        if (valid && (structureArr[(int)position.x, (int)position.y] == null || structureArr[(int)position.x, (int)position.y].tag.Equals(World.ROAD)))
        {
            validPlacement = true;
        }
        else
        {
            validPlacement = false;
        }
        return validPlacement;
    }

    /// <summary>
    /// Checks if an aqueduct can be built on a road
    /// </summary>
    /// <param name="aqueduct">the aqueduct we are checking validity of</param>
    /// <param name="structureArr">the array of structures currently in the world</param>
    /// <returns></returns>
    private bool specialRoadCaseValidity(GameObject aqueduct, GameObject[,] structureArr)
    {
        Vector2 aqueductPos = aqueduct.transform.position;
        if (structureArr[(int)aqueductPos.x, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y].tag.Equals(World.ROAD)
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct() != null)
        {
            return false;
        }
        //checking for what gameobject appearance I want and add it to the structureArrArr
        int nearbyAqueductCount = 0;
        bool topAq = false;
        if (((int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<Aqueduct>() != null)
            || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 1))
            && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 1)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
        {
            topAq = true;
            nearbyAqueductCount++;
        }
        bool botAq = false;
        if (((int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<Aqueduct>() != null)
            || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 1))
            && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 1)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
        {
            botAq = true;
            nearbyAqueductCount++;
        }
        bool leftAq = false;
        if (((int)aqueductPos.x - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<Aqueduct>() != null)
            || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x - 1, (int)aqueductPos.y))
            && !tempAqueducts[new Vector2((int)aqueductPos.x - 1, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
        {
            leftAq = true;
            nearbyAqueductCount++;
        }
        bool rightAq = false;
        if (((int)aqueductPos.x + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<Aqueduct>() != null)
            || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x + 1, (int)aqueductPos.y))
            && !tempAqueducts[new Vector2((int)aqueductPos.x + 1, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
        {
            rightAq = true;
            nearbyAqueductCount++;
        }
        //There must be a connection on either side of the road for an aqueduct to be valid over a road
        if (nearbyAqueductCount != 2)
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
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].tag.Equals(World.ROAD))
        {
            topRoad = true;
            nearbyRoadCount++;
            if (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>() != null
                && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>().getAqueduct() != null
                || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 1))
                && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 1)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im"))))
            {
                nearbyArchCount++;
            }
        }
        bool botRoad = false;
        if ((int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] != null
            && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].tag.Equals(World.ROAD)))
        {
            botRoad = true;
            nearbyRoadCount++;
            if (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>() != null
                && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>().getAqueduct() != null
                || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 1))
                && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 1)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im"))))
            {
                nearbyArchCount++;
            }
        }
        bool leftRoad = false;
        if ((int)aqueductPos.x - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].tag.Equals(World.ROAD))
        {
            leftRoad = true;
            nearbyRoadCount++;
            if (structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>() != null
                && (structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct() != null
                || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x - 1, (int)aqueductPos.y))
                && !tempAqueducts[new Vector2((int)aqueductPos.x - 1, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im"))))
            {
                nearbyArchCount++;
            }
        }
        bool rightRoad = false;
        if ((int)aqueductPos.x + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].tag.Equals(World.ROAD))
        {
            rightRoad = true;
            nearbyRoadCount++;
            if (structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>() != null
                && (structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct() != null
                || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x + 1, (int)aqueductPos.y))
                && !tempAqueducts[new Vector2((int)aqueductPos.x + 1, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im"))))
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

    /// <summary>
    /// Changes the preview appearance of the aqueduct attached to the player's mouse.
    /// </summary>
    /// <param name="aqueduct">the aqueduct object being previewed</param>
    /// <param name="structureArr">a multidimensional array containing roads and buildings</param>
    private void updateAqueductPreview(GameObject aqueduct, GameObject[,] structureArr)
    {
        Vector2 aqueductPos = aqueduct.transform.position;
        
        GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;

        bool north = false;
        bool south = false;
        bool west = false;
        bool east = false;
        //Checking the adjacent aqueducts to determine what connections are available
        int nearbyAqueductCount = 0;
        if ((int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<Aqueduct>() != null
            || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 1))
                && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 1)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
        {
            nearbyAqueductCount++;
            north = true;
        }
        if ((int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<Aqueduct>() != null
            || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 1))
                && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 1)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
        {
            nearbyAqueductCount++;
            south = true;
        }
        if ((int)aqueductPos.x - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<Aqueduct>() != null
            || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x - 1, (int)aqueductPos.y))
                && !tempAqueducts[new Vector2((int)aqueductPos.x - 1, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
        {
            nearbyAqueductCount++;
            west = true;
        }
        if ((int)aqueductPos.x + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<Aqueduct>() != null
            || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x + 1, (int)aqueductPos.y))
                && !tempAqueducts[new Vector2((int)aqueductPos.x + 1, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
        {
            nearbyAqueductCount++;
            east = true;
        }

        int nearbyArchCount = 0;
        bool topArch = false;
        if ((int)aqueductPos.y + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].tag.Equals(World.ROAD)
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>() != null
            && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 1].GetComponent<RoadInformation>().getAqueduct() != null
            || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 1))
                && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 1)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im"))))
        {
            topArch = true;
            nearbyArchCount++;
            north = true;
        }
        bool botArch = false;
        if ((int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].tag.Equals(World.ROAD)
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>() != null
            && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<RoadInformation>().getAqueduct() != null
            || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 1))
                && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 1)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im"))))
        {
            botArch = true;
            nearbyArchCount++;
            south = true;
        }
        bool leftArch = false;
        if ((int)aqueductPos.x - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].tag.Equals(World.ROAD)
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>() != null
            && (structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct() != null
            || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x - 1, (int)aqueductPos.y))
                && !tempAqueducts[new Vector2((int)aqueductPos.x - 1, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im"))))
        {
            leftArch = true;
            nearbyArchCount++;
            west = true;
        }
        bool rightArch = false;
        if ((int)aqueductPos.x + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].tag.Equals(World.ROAD)
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>() != null
            && (structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<RoadInformation>().getAqueduct() != null
            || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x + 1, (int)aqueductPos.y))
                && !tempAqueducts[new Vector2((int)aqueductPos.x + 1, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im"))))
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
            if (aqueduct.transform.position.x == Mathf.RoundToInt(reservoirObj.transform.position.x))
            {
                nearbyReservoirCount++;
                north = true;
            }
        }
        if ((int)aqueductPos.y - 1 > 0 && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1].GetComponent<Reservoir>() != null)
        {
            GameObject reservoirObj = structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 1];
            if (aqueduct.transform.position.x == Mathf.RoundToInt(reservoirObj.transform.position.x))
            {
                nearbyReservoirCount++;
                south = true;
            }
        }
        if ((int)aqueductPos.x - 1 > 0 && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y].GetComponent<Reservoir>() != null)
        {
            GameObject reservoirObj = structureArr[(int)aqueductPos.x - 1, (int)aqueductPos.y];
            if (aqueduct.transform.position.y == Mathf.RoundToInt(reservoirObj.transform.position.y))
            {
                nearbyReservoirCount++;
                west = true;
            }
        }
        if ((int)aqueductPos.x + 1 < myWorld.mapSize && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y].GetComponent<Reservoir>() != null)
        {
            GameObject reservoirObj = structureArr[(int)aqueductPos.x + 1, (int)aqueductPos.y];
            if (aqueduct.transform.position.y == Mathf.RoundToInt(reservoirObj.transform.position.y))
            {
                nearbyReservoirCount++;
                east = true;
            }
        }

        bool overRoad = false;
        if (structureArr[(int)aqueductPos.x, (int)aqueductPos.y] != null
            && structureArr[(int)aqueductPos.x, (int)aqueductPos.y].tag.Equals(World.ROAD))
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
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                        || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2))
                        && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.y - 2 > 0 && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.x - 2 > 0 && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite4;
            }
            else if (northConnectionValid && westConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite3NWE;
            }
            else if (southConnectionValid && westConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite3SWE;
            }
            else if (northConnectionValid && southConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite3NSE;
            }
            else if (northConnectionValid && southConnectionValid && westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite3NSW;
            }
            else if (northConnectionValid && southConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (northConnectionValid && westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NW;
            }
            else if (southConnectionValid && westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2SW;
            }
            else if (northConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (southConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
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
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                        || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2))
                        && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.y - 2 > 0 && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.x - 2 > 0 && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite3NSW;
            }
            else if (northConnectionValid && southConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (northConnectionValid && westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NW;
            }
            else if (southConnectionValid && westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2SW;
            }
            else if (northConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (southConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
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
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                        || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2))
                        && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.y - 2 > 0 && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            if (northConnectionValid && southConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite3NSE;
            }
            else if (northConnectionValid && southConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (northConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NE;
            }
            else if (southConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2SE;
            }
            else if (northConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (southConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
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
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                        || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2))
                        && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.x - 2 > 0 && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            if (northConnectionValid && westConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite3NWE;
            }
            else if (northConnectionValid && westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NW;
            }
            else if (northConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NE;
            }
            else if (westConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
            else if (northConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
            else if (eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
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
                    if ((int)aqueductPos.y - 2 > 0 && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                        && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                        || structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                        || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2))
                        && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.x - 2 > 0 && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            if (southConnectionValid && westConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite3SWE;
            }
            else if (southConnectionValid && westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2SW;
            }
            else if (southConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2SE;
            }
            else if (westConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
            else if (southConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
            else if (eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
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
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                        || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2))
                        && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.y - 2 > 0 && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (northConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (southConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
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
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                        || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2))
                        && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.x - 2 > 0 && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            if (northConnectionValid && westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NW;
            }
            else if (northConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
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
                    if ((int)aqueductPos.y + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                        && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                        || structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                        || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2))
                        && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            if (northConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NE;
            }
            else if (northConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
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
                if (botArch)
                {
                    if ((int)aqueductPos.y - 2 > 0 && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                        && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                        || structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                        || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2))
                        && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.x - 2 > 0 && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
            bool southConnectionValid = (botArch ? southArchConnectionValid : true);
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            if (southConnectionValid && westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2SW;
            }
            else if (southConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
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
                    if ((int)aqueductPos.y - 2 > 0 && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            if (southConnectionValid && eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2SE;
            }
            else if (southConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
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
                    if ((int)aqueductPos.x - 2 > 0 && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
                    if ((int)aqueductPos.x + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
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
            bool westConnectionValid = (leftArch ? westArchConnectionValid : true);
            bool eastConnectionValid = (rightArch ? eastArchConnectionValid : true);
            if (westConnectionValid && eastConnectionValid)
            {
                if (overRoad)
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WEArch;
                }
                else
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
                }
            }
            else if (westConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
            else if (eastConnectionValid)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
            }
        }
        //One connection
        else if (north)
        {
            if (nearbyReservoirCount > 0)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (nearbyArchCount > 0)
            {
                //Check if it can connect
                bool connectionPossible = false;
                if ((int)aqueductPos.y + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2] != null
                    && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x, (int)aqueductPos.y + 2].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y + 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
                {
                    connectionPossible = true;
                }

                if (connectionPossible)
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
                }
            }
            else
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
        }
        else if (south)
        {
            if (nearbyReservoirCount > 0)
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
            else if (nearbyArchCount > 0)
            {
                //Check if it can connect
                bool connectionPossible = false;
                if ((int)aqueductPos.y - 2 > 0 && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2] != null
                    && (structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x, (int)aqueductPos.y - 2].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x, (int)aqueductPos.y - 2)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
                {
                    connectionPossible = true;
                }
                if (connectionPossible)
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
                }
            }
            else
            {
                aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2NS;
            }
        }
        else if (west)
        {
            if (nearbyReservoirCount > 0)
            {
                if (overRoad)
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WEArch;
                }
                else
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
                }
            }
            else if (nearbyArchCount > 0)
            {
                //Check if it can connect
                bool connectionPossible = false;
                if ((int)aqueductPos.x - 2 > 0 && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x - 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x - 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
                {
                    connectionPossible = true;
                }

                if (connectionPossible)
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
                }
            }
            else
            {
                if (overRoad)
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WEArch;
                }
                else
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
                }
            }
        }
        else if (east)
        {
            if (nearbyReservoirCount > 0)
            {
                if (overRoad)
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WEArch;
                }
                else
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
                }
            }
            else if (nearbyArchCount > 0)
            {
                //Check if it can connect
                bool connectionPossible = false;
                if ((int)aqueductPos.x + 2 < myWorld.mapSize && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y] != null
                    && (structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Reservoir>() != null
                    || structureArr[(int)aqueductPos.x + 2, (int)aqueductPos.y].GetComponent<Aqueduct>() != null))
                    || (tempAqueducts.ContainsKey(new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y))
                    && !tempAqueducts[new Vector2((int)aqueductPos.x + 2, (int)aqueductPos.y)].GetComponent<SpriteRenderer>().sprite.name.Contains("Im")))
                {
                    connectionPossible = true;
                }

                if (connectionPossible)
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
                }
            }
            else
            {
                if (overRoad)
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WEArch;
                }
                else
                {
                    aqueduct.GetComponent<SpriteRenderer>().sprite = possibleSprite2WE;
                }
            }
        }
    }
}
