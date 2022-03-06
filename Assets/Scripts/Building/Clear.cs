using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Used by the ClearLand button in the game UI to clear out obstacles
/// and player-constructed objects in the map.
/// </summary>
public class Clear : MonoBehaviour {
    public Sprite clearSection;
    public GameObject straightRoad;
    public GameObject tempClearObject;

    private World myWorld;
    private Vector2 startClearPosition;
    private Color color = Color.red;
    private LineRenderer lineRenderer;
    private SpriteRenderer spriteRenderer;
    private Dictionary<Vector2, GameObject> clearObjects;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start() {
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        startClearPosition = new Vector2(-1, -1);

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.sortingLayerName = "CreationGrid";
        lineRenderer.widthMultiplier = 0.03f;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.enabled = false;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        clearObjects = new Dictionary<Vector2, GameObject>();
    }

    /// <summary>
    /// Updates the visual location of the "DestroySection" sprite and deletes what is at the location
    /// if the left mouse button is pressed.
    /// </summary>
    void Update() {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (startClearPosition.x != -1 && startClearPosition.y != -1)
            {
                startClearPosition.x = -1;
                startClearPosition.y = -1;
                lineRenderer.enabled = false;
                spriteRenderer.enabled = true;
                foreach (GameObject clearObject in clearObjects.Values)
                {
                    Destroy(clearObject);
                }
                clearObjects = new Dictionary<Vector2, GameObject>();
            }
            else
            {
                //exits out of destroy mode if the right mouse button or escape is clicked
                Destroy(gameObject);
            }
        }
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = Mathf.RoundToInt(mousePos.x);
        mousePos.y = Mathf.RoundToInt(mousePos.y);
        mousePos.z = 0;

        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();

        if (mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1)
        {
            transform.position = Vector2.Lerp(transform.position, mousePos, 1f);

            if (Input.GetMouseButtonDown(0) && startClearPosition.x == -1 && startClearPosition.y == -1)
            {
                startClearPosition = mousePos;
                spriteRenderer.enabled = false;
            }
        }

        int xSpaces = Mathf.RoundToInt(mousePos.x - startClearPosition.x);
        int ySpaces = Mathf.RoundToInt(mousePos.y - startClearPosition.y);
        //Draw the area in which stuff will be destroyed
        if (startClearPosition.x != -1 && startClearPosition.y != -1)
        {
            float xStartAdjustment = xSpaces >= 0 ? -0.5f : 0.5f;
            float yStartAdjustment = ySpaces >= 0 ? -0.5f : 0.5f;
            //Start
            lineRenderer.SetPosition(0, new Vector3(startClearPosition.x + xStartAdjustment, startClearPosition.y + yStartAdjustment, 0));
            //Bottom right
            lineRenderer.SetPosition(1, new Vector3(mousePos.x - xStartAdjustment, startClearPosition.y + yStartAdjustment, 0));
            //Mouse Position
            lineRenderer.SetPosition(2, new Vector3(mousePos.x - xStartAdjustment, mousePos.y - yStartAdjustment, 0));
            //Top Left
            lineRenderer.SetPosition(3, new Vector3(startClearPosition.x + xStartAdjustment, mousePos.y - yStartAdjustment, 0));
            //Return to start
            lineRenderer.SetPosition(4, new Vector3(startClearPosition.x + xStartAdjustment, startClearPosition.y + yStartAdjustment, 0));
            lineRenderer.enabled = true;

            //Go through all the locations in the selected area to display if something in it is being cleared
            List<Vector2> clearLocationsToRemove = new List<Vector2>(clearObjects.Keys);
            int xAbsSpaces = Mathf.Abs(xSpaces);
            int yAbsSpaces = Mathf.Abs(ySpaces);
            //Go through all the structure and terrain locations in the dragged area
            for (int i = 0; i <= xAbsSpaces; i++)
            {
                int xSign = xSpaces == 0 ? 1 : xSpaces / xAbsSpaces;
                for (int j = 0; j <= yAbsSpaces; j++)
                {
                    int ySign = ySpaces == 0 ? 1 : ySpaces / yAbsSpaces;
                    //Check if there is a structure or tree in the location
                    Vector2 tempClearObjPosition = new Vector2((int)startClearPosition.x + i * xSign, (int)startClearPosition.y + j * ySign);
                    if (tempClearObjPosition.x > 0 && tempClearObjPosition.x < myWorld.mapSize
                        && tempClearObjPosition.y > 0 && tempClearObjPosition.y < myWorld.mapSize
                        && (structureArr[(int)tempClearObjPosition.x, (int)tempClearObjPosition.y] != null
                        || (terrainArr[(int)tempClearObjPosition.x, (int)tempClearObjPosition.y] != null
                        && terrainArr[(int)tempClearObjPosition.x, (int)tempClearObjPosition.y].tag.Equals(World.TREES))))
                    {
                        if (clearLocationsToRemove.Contains(tempClearObjPosition))
                        {
                            clearLocationsToRemove.Remove(tempClearObjPosition);
                        }
                        else
                        {
                            GameObject tempClearObj = Instantiate(tempClearObject, tempClearObjPosition, Quaternion.identity) as GameObject;
                            clearObjects.Add(tempClearObjPosition, tempClearObj);
                        }
                    }
                }
            }
            //Remove locations that are no longer being considered
            foreach (Vector2 clearLocationToRemove in clearLocationsToRemove)
            {
                Destroy(clearObjects[clearLocationToRemove]);
                clearObjects.Remove(clearLocationToRemove);
            }
        }

        if (Input.GetMouseButtonUp(0) && startClearPosition.x != -1)
        {
            int xAbsSpaces = Mathf.Abs(xSpaces);
            int yAbsSpaces = Mathf.Abs(ySpaces);
            //Go through all the structure and terrain locations in the dragged area
            for (int i = 0; i <= xAbsSpaces; i++)
            {
                int xSign = xSpaces == 0 ? 1 : xSpaces / xAbsSpaces;
                for (int j = 0; j <= yAbsSpaces; j++)
                {
                    int ySign = ySpaces == 0 ? 1 : ySpaces / yAbsSpaces;
                    //If the user is clearing a section of space, this will clear every possible thing in that section.  If it's one grid space, it will clear the top item
                    attemptClearOnPosition(new Vector2(startClearPosition.x + i * xSign, startClearPosition.y + j * ySign), structureArr, terrainArr, xAbsSpaces > 0 || yAbsSpaces > 0);
                }
            }
            foreach (GameObject clearObject in clearObjects.Values)
            {
                Destroy(clearObject);
            }
            clearObjects = new Dictionary<Vector2, GameObject>();
            startClearPosition.x = -1;
            startClearPosition.y = -1;
            lineRenderer.enabled = false;
            spriteRenderer.enabled = true;
        }
    }

    /// <summary>
    /// Attempts to clear a grid space on the map
    /// </summary>
    /// <param name="locationToClear">The grid space to clear</param>
    /// <param name="structureArr">The structure array to update</param>
    /// <param name="terrainArr">The terrain array to update</param>
    /// <param name="clearRoadAndAqueduct">Whether to clear both road and aqueduct if they're in the same grid space</param>
    private void attemptClearOnPosition(Vector2 locationToClear, GameObject[,] structureArr, GameObject[,] terrainArr, bool clearRoadAndAqueduct)
    {
        if (locationToClear.x > 0 && locationToClear.x < myWorld.mapSize - 1 && locationToClear.y > 0 && locationToClear.y < myWorld.mapSize - 1
            && structureArr[(int)locationToClear.x, (int)locationToClear.y] != null
            && (structureArr[(int)locationToClear.x, (int)locationToClear.y].tag.Equals(World.ROAD)
            || structureArr[(int)locationToClear.x, (int)locationToClear.y].tag.Equals(World.BUILDING)
            || structureArr[(int)locationToClear.x, (int)locationToClear.y].tag.Equals(World.HOUSE)
            || structureArr[(int)locationToClear.x, (int)locationToClear.y].tag.Equals(World.HIGH_BRIDGE)
            || structureArr[(int)locationToClear.x, (int)locationToClear.y].tag.Equals(World.LOW_BRIDGE)))
        {
            GameObject structureToClear = structureArr[(int)locationToClear.x, (int)locationToClear.y];
            if (structureArr[(int)locationToClear.x, (int)locationToClear.y].tag.Equals(World.HOUSE))
            {
                HouseInformation houseInfo = structureToClear.GetComponent<HouseInformation>();
                houseInfo.destroyHouse();
            }
            if (structureArr[(int)locationToClear.x, (int)locationToClear.y].tag.Equals(World.BUILDING))
            {
                bool clearingAnAqueduct = false;
                bool clearingAReservoir = false;
                List<GameObject> waterConnections = new List<GameObject>();
                if (structureToClear.GetComponent<Aqueduct>() != null)
                {
                    clearingAnAqueduct = true;
                    //get a list of the current connections
                    waterConnections = structureToClear.GetComponent<Aqueduct>().getConnections();
                }
                else if (structureToClear.GetComponent<Reservoir>() != null)
                {
                    clearingAReservoir = true;
                    //get a list of the current connections
                    waterConnections = structureToClear.GetComponent<Reservoir>().getConnections();
                    //Need to set all of the reservoir's locations in the structureArr to null
                    Vector2 reservoirPos = new Vector2(Mathf.RoundToInt(structureToClear.transform.position.x),
                        Mathf.RoundToInt(structureToClear.transform.position.y));
                    BoxCollider2D boxCollider2D = structureToClear.GetComponent<BoxCollider2D>();
                    for (int i = 0; i < boxCollider2D.size.x; i++)
                    {
                        for (int j = 0; j < boxCollider2D.size.y; j++)
                        {
                            //The -1 is to get to the bottom left corner of the reservoir
                            structureArr[(int)reservoirPos.x - 1 + i, (int)reservoirPos.y - 1 + j] = null;
                        }
                    }
                    int waterSourcesCount = structureToClear.GetComponent<Reservoir>().getWaterSources().Count;
                    if (waterSourcesCount > 0)
                    {
                        structureToClear.GetComponent<Reservoir>().updatePipes(false);
                    }
                }

                Employment employment = structureToClear.GetComponent<Employment>();
                employment.destroyEmployment();

                //Clearing the array in that position because even though the object should be removed, it doesn't seem to update properly
                if (structureArr[(int)locationToClear.x, (int)locationToClear.y] != null)
                {
                    if (structureArr[(int)locationToClear.x, (int)locationToClear.y].GetComponent<RoadInformation>() == null)
                    {
                        structureArr[(int)locationToClear.x, (int)locationToClear.y] = null;
                    }
                }

                //Need to update the connections of any aqueducts or reservoirs that were connected if this was a reservoir/aqueduct
                if (clearingAnAqueduct || clearingAReservoir)
                {
                    updateWaterConnections(waterConnections);
                }
            }
            //Deleting bridges
            if (structureArr[(int)locationToClear.x, (int)locationToClear.y] != null && structureArr[(int)locationToClear.x, (int)locationToClear.y].tag.Equals(World.HIGH_BRIDGE)
                || structureArr[(int)locationToClear.x, (int)locationToClear.y] != null && structureArr[(int)locationToClear.x, (int)locationToClear.y].tag.Equals(World.LOW_BRIDGE))
            {
                structureArr[(int)locationToClear.x, (int)locationToClear.y].GetComponent<Bridge>().destroyBridge();
            }
            //If the deleted object is a road, the surrounding roads need to be updated to reflect
            // the fact there is no longer a road where the deleted one was
            if (structureArr[(int)locationToClear.x, (int)locationToClear.y] != null && structureArr[(int)locationToClear.x, (int)locationToClear.y].tag.Equals(World.ROAD))
            {
                if (structureArr[(int)locationToClear.x, (int)locationToClear.y].GetComponent<RoadInformation>().getAqueduct() == null)
                {
                    clearRoad(structureToClear, locationToClear, structureArr);
                }
                else
                {
                    bool clearingAnAqueduct = false;
                    List<GameObject> aqueductConnections = new List<GameObject>();
                    if (structureToClear.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>() != null)
                    {
                        clearingAnAqueduct = true;
                        //get a list of the current connections
                        aqueductConnections = structureToClear.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().getConnections();
                    }

                    structureArr[(int)locationToClear.x, (int)locationToClear.y].GetComponent<RoadInformation>().destroyRoad();
                    GameObject roadWithDestroyedAqueduct = structureArr[(int)locationToClear.x, (int)locationToClear.y];
                    structureArr[(int)locationToClear.x, (int)locationToClear.y] = null;//connections won't update right away unless I explicity set this to null

                    if (clearingAnAqueduct)
                    {
                        updateWaterConnections(aqueductConnections);
                    }
                    if (clearRoadAndAqueduct)
                    {
                        clearRoad(structureToClear, locationToClear, structureArr);
                    }
                    else
                    {
                        structureArr[(int)locationToClear.x, (int)locationToClear.y] = roadWithDestroyedAqueduct;
                    }
                }
            }
        }
        //allow the user to destroy trees
        if (locationToClear.x > 0 && locationToClear.x < myWorld.mapSize - 1 && locationToClear.y > 0 && locationToClear.y < myWorld.mapSize - 1
            && terrainArr[(int)locationToClear.x, (int)locationToClear.y] != null
            && (terrainArr[(int)locationToClear.x, (int)locationToClear.y].tag.Equals(World.TREES)))
        {
            GameObject terrainToClear = terrainArr[(int)locationToClear.x, (int)locationToClear.y];
            TreeObject treeToClear = terrainToClear.GetComponent<TreeObject>();
            treeToClear.removeTree();
        }
    }

    /// <summary>
    /// Destroys a road segment
    /// </summary>
    /// <param name="roadToClear">The road to clear</param>
    /// <param name="locationToClear">The grid space to clear the road from</param>
    /// <param name="structureArr">The world's structure array</param>
    private void clearRoad(GameObject roadToClear, Vector2 locationToClear, GameObject[,] structureArr)
    {
        structureArr[(int)locationToClear.x, (int)locationToClear.y] = null;
        //updateRoadConnection(mousePos, structureArr);
        //go through roads attached to the deleted road to update them visibly
        //do not update the roads outside building limits
        if ((int)locationToClear.y + 1 < myWorld.mapSize - 1 && structureArr[(int)locationToClear.x, (int)locationToClear.y + 1] != null
           && structureArr[(int)locationToClear.x, (int)locationToClear.y + 1].tag.Equals(World.ROAD))
        {
            //update road above the one you are trying to build
            structureArr[(int)locationToClear.x, (int)locationToClear.y + 1].GetComponent<RoadInformation>().updateRoadConnection();
        }
        if ((int)locationToClear.y - 1 > 0 && structureArr[(int)locationToClear.x, (int)locationToClear.y - 1] != null
            && structureArr[(int)locationToClear.x, (int)locationToClear.y - 1].tag.Equals(World.ROAD))
        {
            //update road below the one you are trying to build
            structureArr[(int)locationToClear.x, (int)locationToClear.y - 1].GetComponent<RoadInformation>().updateRoadConnection(); ;
        }
        if ((int)locationToClear.x - 1 > 0 && structureArr[(int)locationToClear.x - 1, (int)locationToClear.y] != null
            && structureArr[(int)locationToClear.x - 1, (int)locationToClear.y].tag.Equals(World.ROAD))
        {
            //update road to the left of the one you are trying to build
            structureArr[(int)locationToClear.x - 1, (int)locationToClear.y].GetComponent<RoadInformation>().updateRoadConnection();
        }
        if ((int)locationToClear.x + 1 < myWorld.mapSize - 1 && structureArr[(int)locationToClear.x + 1, (int)locationToClear.y] != null
            && structureArr[(int)locationToClear.x + 1, (int)locationToClear.y].tag.Equals(World.ROAD))
        {
            //update road to the right of the one you are trying to build
            structureArr[(int)locationToClear.x + 1, (int)locationToClear.y].GetComponent<RoadInformation>().updateRoadConnection();
        }
        Destroy(roadToClear);
    }

    /// <summary>
    /// Updates the connections of a reservoir or aqueduct that is being deleted
    /// </summary>
    /// <param name="waterConnections">the connections to update</param>
    private void updateWaterConnections(List<GameObject> waterConnections)
    {
        //call update connections on each item in the list of the aqueduct's old connections
        foreach (GameObject connection in waterConnections)
        {
            if (connection != null)
            {
                if (connection.GetComponent<Aqueduct>() != null)
                {
                    Aqueduct aqueduct = connection.GetComponent<Aqueduct>();
                    aqueduct.updateConnections();
                    aqueduct.setTimeToUpdateWaterSources(false);
                    aqueduct.setNextToClearedWaterStructure(true);
                }
                else if (connection.GetComponent<Reservoir>() != null)
                {
                    Reservoir reservoir = connection.GetComponent<Reservoir>();
                    reservoir.updateConnections();
                    reservoir.setTimeToUpdateWaterSources(false);
                    reservoir.setNextToClearedWaterStructure(true);
                }
                else if (connection.GetComponent<RoadInformation>() != null)
                {
                    if (connection.GetComponent<RoadInformation>().getAqueduct() != null)
                    {
                        //If an arch loses its connections, it should also be destroyed
                        Aqueduct connectingAqueductArch = connection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>();
                        if (connectingAqueductArch.getTopConnection() != null || connectingAqueductArch.getBotConnection() != null
                            || connectingAqueductArch.getLeftConnection() != null || connectingAqueductArch.getRightConnection() != null)
                        {
                            connectingAqueductArch.updateConnections();
                            connectingAqueductArch.setTimeToUpdateWaterSources(false);
                            connectingAqueductArch.setNextToClearedWaterStructure(true);
                        }
                        else if (connectingAqueductArch.getTopConnection() == null && connectingAqueductArch.getBotConnection() == null
                            && connectingAqueductArch.getLeftConnection() == null && connectingAqueductArch.getRightConnection() == null)
                        {
                            connection.GetComponent<RoadInformation>().destroyRoad();
                        }
                    }
                }
            }
        }
    }
}
