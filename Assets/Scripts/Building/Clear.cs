using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Used by the ClearLand button in the game UI to clear out obstacles
 * and player-constructed objects in the map.
 */
public class Clear : MonoBehaviour {
    public Sprite clearSection;
    public GameObject straightRoad;
    public GameObject cornerRoad;
    public GameObject tRoad;
    public GameObject crossRoad;
    private Dictionary<GameObject, float> delayDeletion;

    // Use this for initialization
    void Start() {
        delayDeletion = new Dictionary<GameObject, float>();
    }

    /**
     * Updates the visual location of the "DestroySection" sprite and deletes what is at the location
     * if the left mouse button is pressed.
     */
    void Update() {
        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Escape))
        {
            //exits out of destroy mode if the right mouse button or escape is clicked
            Destroy(gameObject);
        }
        //Stops me from deleting a road immediately with an aqueduct
        if (delayDeletion.Count > 0)
        {
            List<GameObject> keysToRemove = new List<GameObject>();
            foreach (KeyValuePair<GameObject, float> kvp in delayDeletion)
            {
                if (kvp.Value < Time.time)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
            foreach (GameObject key in keysToRemove)
            {
                delayDeletion.Remove(key);
            }
        }

        GameObject world = GameObject.Find("WorldInformation");
        World myWorld = world.GetComponent<World>();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = Mathf.RoundToInt(mousePos.x);
        mousePos.y = Mathf.RoundToInt(mousePos.y);
        mousePos.z = 0;

        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();

        if (mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1)
        {
            transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
        }

        if (Input.GetMouseButton(0)
            && mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1
            && structureArr[(int)mousePos.x, (int)mousePos.y] != null
            && (structureArr[(int)mousePos.x, (int)mousePos.y].tag == "Road"
            || structureArr[(int)mousePos.x, (int)mousePos.y].tag == "Building"
            || structureArr[(int)mousePos.x, (int)mousePos.y].tag == "House"))
        {
            GameObject structureToClear = structureArr[(int)mousePos.x, (int)mousePos.y];
            if (structureArr[(int)mousePos.x, (int)mousePos.y].tag == "House")
            {
                HouseInformation houseInfo = structureToClear.GetComponent<HouseInformation>();
                houseInfo.destroyHouse();
            }
            if (structureArr[(int)mousePos.x, (int)mousePos.y].tag == "Building")
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
                }

                Employment employment = structureToClear.GetComponent<Employment>();
                employment.destroyEmployment();

                //Clearing the array in that position because even though the object should be removed, it doesn't seem to update properly
                if (structureArr[(int)mousePos.x, (int)mousePos.y] != null)
                {
                    if (structureArr[(int)mousePos.x, (int)mousePos.y].GetComponent<RoadInformation>() == null)
                    {
                        structureArr[(int)mousePos.x, (int)mousePos.y] = null;
                    }
                }

                //Need to update the connections of any aqueducts or reservoirs that were connected if this was a reservoir/aqueduct
                if (clearingAnAqueduct || clearingAReservoir)
                {
                    updateWaterConnections(waterConnections);
                }
            }
            //If the deleted object is a road, the surrounding roads need to be updated to reflect
            // the fact there is no longer a road where the deleted one was
            if (structureArr[(int)mousePos.x, (int)mousePos.y] != null && structureArr[(int)mousePos.x, (int)mousePos.y].tag == "Road")
            {
                if (structureArr[(int)mousePos.x, (int)mousePos.y].GetComponent<RoadInformation>().getAqueduct() == null
                    && !delayDeletion.ContainsKey(structureArr[(int)mousePos.x, (int)mousePos.y]))
                {
                    structureArr[(int)mousePos.x, (int)mousePos.y] = null;
                    //updateRoadConnection(mousePos, structureArr);
                    //go through roads attached to the deleted road to update them visibly
                    //do not update the roads outside building limits
                    if ((int)mousePos.y + 1 < myWorld.mapSize - 1 && structureArr[(int)mousePos.x, (int)mousePos.y + 1] != null
                       && structureArr[(int)mousePos.x, (int)mousePos.y + 1].tag == "Road")
                    {
                        //update road above the one you are trying to build
                        structureArr[(int)mousePos.x, (int)mousePos.y + 1].GetComponent<RoadInformation>().updateRoadConnection();
                    }
                    if ((int)mousePos.y - 1 > 0 && structureArr[(int)mousePos.x, (int)mousePos.y - 1] != null
                        && structureArr[(int)mousePos.x, (int)mousePos.y - 1].tag == "Road")
                    {
                        //update road below the one you are trying to build
                        structureArr[(int)mousePos.x, (int)mousePos.y - 1].GetComponent<RoadInformation>().updateRoadConnection(); ;
                    }
                    if ((int)mousePos.x - 1 > 0 && structureArr[(int)mousePos.x - 1, (int)mousePos.y] != null
                        && structureArr[(int)mousePos.x - 1, (int)mousePos.y].tag == "Road")
                    {
                        //update road to the left of the one you are trying to build
                        structureArr[(int)mousePos.x - 1, (int)mousePos.y].GetComponent<RoadInformation>().updateRoadConnection();
                    }
                    if ((int)mousePos.x + 1 < myWorld.mapSize - 1 && structureArr[(int)mousePos.x + 1, (int)mousePos.y] != null
                        && structureArr[(int)mousePos.x + 1, (int)mousePos.y].tag == "Road")
                    {
                        //update road to the right of the one you are trying to build
                        structureArr[(int)mousePos.x + 1, (int)mousePos.y].GetComponent<RoadInformation>().updateRoadConnection();
                    }
                    Destroy(structureToClear);
                }
                else if (!delayDeletion.ContainsKey(structureArr[(int)mousePos.x, (int)mousePos.y]))
                {
                    bool clearingAnAqueduct = false;
                    List<GameObject> aqueductConnections = new List<GameObject>();
                    if (structureToClear.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>() != null)
                    {
                        clearingAnAqueduct = true;
                        //get a list of the current connections
                        aqueductConnections = structureToClear.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().getConnections();
                    }
                    
                    structureArr[(int)mousePos.x, (int)mousePos.y].GetComponent<RoadInformation>().destroyRoad();
                    GameObject roadWithDestroyedAqueduct = structureArr[(int)mousePos.x, (int)mousePos.y];
                    structureArr[(int)mousePos.x, (int)mousePos.y] = null;//connections won't update right away unless I explicity set this to null

                    if (clearingAnAqueduct)
                    {
                        //call update connections on each item in the list of the aqueduct's old connections
                        foreach (GameObject connection in aqueductConnections)
                        {
                            if (connection != null)
                            {
                                if (connection.GetComponent<Aqueduct>() != null)
                                {
                                    connection.GetComponent<Aqueduct>().updateConnections();
                                }
                                else if (connection.GetComponent<Reservoir>() != null)
                                {
                                    connection.GetComponent<Reservoir>().updateConnections();
                                }
                                else if (connection.GetComponent<RoadInformation>() != null)
                                {
                                    connection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().updateConnections();
                                }
                            }
                        }
                    }
                    //roadWithDestroyedAqueduct.GetComponent<RoadInformation>().setAqueduct(null);
                    structureArr[(int)mousePos.x, (int)mousePos.y] = roadWithDestroyedAqueduct;

                    //0.3f is an arbitrarily chosen number I feel is long enough to avoid losing the road from a click
                    delayDeletion.Add(structureArr[(int)mousePos.x, (int)mousePos.y], Time.time + 0.3f);
                }
            }

            //myControls.buildArr.removeFromBuildArr((int)mousePos.y, (int)mousePos.x);
            //Destroy(structureToClear);
        }
        //allow the user to destroy trees
        if (Input.GetMouseButton(0)
            && mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1
            && terrainArr[(int)mousePos.x, (int)mousePos.y] != null
            && (terrainArr[(int)mousePos.x, (int)mousePos.y].tag == "Trees"))
        {
            GameObject terrainToClear = terrainArr[(int)mousePos.x, (int)mousePos.y];
            TreeRemoval treeToClear = terrainToClear.GetComponent<TreeRemoval>();
            treeToClear.removeTree();
        }
    }

    /**
     * Updates the connections of a reservoir or aqueduct that is being deleted
     * @param waterConnections the connections to update
     */
    private void updateWaterConnections(List<GameObject> waterConnections)
    {
        //call update connections on each item in the list of the aqueduct's old connections
        foreach (GameObject connection in waterConnections)
        {
            if (connection != null)
            {
                if (connection.GetComponent<Aqueduct>() != null)
                {
                    connection.GetComponent<Aqueduct>().updateConnections();
                }
                else if (connection.GetComponent<Reservoir>() != null)
                {
                    connection.GetComponent<Reservoir>().updateConnections();
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
