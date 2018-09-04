﻿using UnityEngine;
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
    private float timeMouseButton;
    private Dictionary<GameObject, float> delayDeletion;

    // Use this for initialization
    void Start() {
        timeMouseButton = 0.0f;
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
                Employment employment = structureToClear.GetComponent<Employment>();
                employment.destroyEmployment();
            }
            //If the deleted object is a road, the surrounding roads need to be updated to reflect
            // the fact there is no longer a road where the deleted one was
            if (structureArr[(int)mousePos.x, (int)mousePos.y].tag == "Road")
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
                        updateRoadConnection(structureArr[(int)mousePos.x, (int)mousePos.y + 1], structureArr);
                    }
                    if ((int)mousePos.y - 1 > 0 && structureArr[(int)mousePos.x, (int)mousePos.y - 1] != null
                        && structureArr[(int)mousePos.x, (int)mousePos.y - 1].tag == "Road")
                    {
                        //update road below the one you are trying to build
                        updateRoadConnection(structureArr[(int)mousePos.x, (int)mousePos.y - 1], structureArr);
                    }
                    if ((int)mousePos.x - 1 > 0 && structureArr[(int)mousePos.x - 1, (int)mousePos.y] != null
                        && structureArr[(int)mousePos.x - 1, (int)mousePos.y].tag == "Road")
                    {
                        //update road to the left of the one you are trying to build
                        updateRoadConnection(structureArr[(int)mousePos.x - 1, (int)mousePos.y], structureArr);
                    }
                    if ((int)mousePos.x + 1 < myWorld.mapSize - 1 && structureArr[(int)mousePos.x + 1, (int)mousePos.y] != null
                        && structureArr[(int)mousePos.x + 1, (int)mousePos.y].tag == "Road")
                    {
                        //update road to the right of the one you are trying to build
                        updateRoadConnection(structureArr[(int)mousePos.x + 1, (int)mousePos.y], structureArr);
                    }
                    Destroy(structureToClear);
                }
                else if (!delayDeletion.ContainsKey(structureArr[(int)mousePos.x, (int)mousePos.y]))
                {
                    structureArr[(int)mousePos.x, (int)mousePos.y].GetComponent<RoadInformation>().destroyRoad();
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
     * Updates the appearance of the roads as they are added so that they line up with one another
     * @param road is the road being updated
     * @param structureArr is a multidimensional array containing all of the roads and buildings
     */
    void updateRoadConnection(GameObject road, GameObject[,] structureArr)
    {
        GameObject world = GameObject.Find("WorldInformation");
        World myWorld = world.GetComponent<World>();
        Vector2 roadPos = road.transform.position;

        //check here for what gameobject I want and add it to the buildArr
        int nearbyRoadCount = 0;
        //booleans allow me to where the other roads are
        bool top = false;
        if (structureArr[(int)roadPos.x, (int)roadPos.y + 1] != null
            && structureArr[(int)roadPos.x, (int)roadPos.y + 1].tag == "Road")
        {
            top = true;
            nearbyRoadCount++;
        }
        bool bot = false;
        if (structureArr[(int)roadPos.x, (int)roadPos.y - 1] != null
            && structureArr[(int)roadPos.x, (int)roadPos.y - 1].tag == "Road")
        {
            bot = true;
            nearbyRoadCount++;
        }
        bool left = false;
        if (structureArr[(int)roadPos.x - 1, (int)roadPos.y] != null
            && structureArr[(int)roadPos.x - 1, (int)roadPos.y].tag == "Road")
        {
            left = true;
            nearbyRoadCount++;
        }
        bool right = false;
        if (structureArr[(int)roadPos.x + 1, (int)roadPos.y] != null
            && structureArr[(int)roadPos.x + 1, (int)roadPos.y].tag == "Road")
        {
            right = true;
            nearbyRoadCount++;
        }


        //New game object to reflect the road change
        GameObject roadObj;
        //straight road
        if (nearbyRoadCount == 1)
        {
            roadObj = Instantiate(straightRoad, roadPos, Quaternion.identity) as GameObject;
            if (top || bot)
            {
                //change the z rotation
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.z = 90;
                roadObj.transform.rotation = Quaternion.Euler(rotationVector);
            }
            myWorld.constructNetwork.setConstructArr((int)roadPos.x, (int)roadPos.y, roadObj);
            //Need to set the aqueduct if there is one
            roadObj.GetComponent<RoadInformation>().setAqueduct(road.GetComponent<RoadInformation>().getAqueduct());
        }

        //straight or corner; depends on if nearby roads are opposite or diagonal
        if (nearbyRoadCount == 2)
        {
            if (right && left)
            {
                roadObj = Instantiate(straightRoad, roadPos, Quaternion.identity) as GameObject;
                myWorld.constructNetwork.setConstructArr((int)roadPos.x, (int)roadPos.y, roadObj);
                //Need to set the aqueduct if there is one
                roadObj.GetComponent<RoadInformation>().setAqueduct(road.GetComponent<RoadInformation>().getAqueduct());
            }
            else if (top && bot)
            {
                roadObj = Instantiate(straightRoad, roadPos, Quaternion.identity) as GameObject;
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.z = 90;
                roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                myWorld.constructNetwork.setConstructArr((int)roadPos.x, (int)roadPos.y, roadObj);
                //Need to set the aqueduct if there is one
                roadObj.GetComponent<RoadInformation>().setAqueduct(road.GetComponent<RoadInformation>().getAqueduct());
            }
            else if (top)
            {
                roadObj = Instantiate(cornerRoad, roadPos, Quaternion.identity) as GameObject;
                //only needs to be rotated if the second connecting road is on the right
                if (right)
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 270;
                    roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                }
                myWorld.constructNetwork.setConstructArr((int)roadPos.x, (int)roadPos.y, roadObj);
                //Need to set the aqueduct if there is one
                roadObj.GetComponent<RoadInformation>().setAqueduct(road.GetComponent<RoadInformation>().getAqueduct());
            }
            else if (bot)
            {
                roadObj = Instantiate(cornerRoad, roadPos, Quaternion.identity) as GameObject;
                if (right)
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 180;
                    roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                }
                else
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 90;
                    roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                }
                myWorld.constructNetwork.setConstructArr((int)roadPos.x, (int)roadPos.y, roadObj);
                //Need to set the aqueduct if there is one
                roadObj.GetComponent<RoadInformation>().setAqueduct(road.GetComponent<RoadInformation>().getAqueduct());
            }
        }

        //T road
        if (nearbyRoadCount == 3)
        {
            if (top && bot)
            {
                roadObj = Instantiate(tRoad, roadPos, Quaternion.identity) as GameObject;
                if (right)
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 270;
                    roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                }
                else
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 90;
                    roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                }
                myWorld.constructNetwork.setConstructArr((int)roadPos.x, (int)roadPos.y, roadObj);
                //Need to set the aqueduct if there is one
                roadObj.GetComponent<RoadInformation>().setAqueduct(road.GetComponent<RoadInformation>().getAqueduct());
            }
            else if (right && left)
            {
                roadObj = Instantiate(tRoad, roadPos, Quaternion.identity) as GameObject;
                //starting rotation is if the third connection is on top, therefore only need to rotation in this
                // case if last connection is bot
                if (bot)
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 180;
                    roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                }
                myWorld.constructNetwork.setConstructArr((int)roadPos.x, (int)roadPos.y, roadObj);
                //Need to set the aqueduct if there is one
                roadObj.GetComponent<RoadInformation>().setAqueduct(road.GetComponent<RoadInformation>().getAqueduct());
            }
        }

        //crossroad
        if (nearbyRoadCount == 4)
        {
            roadObj = Instantiate(crossRoad, roadPos, Quaternion.identity) as GameObject;
            myWorld.constructNetwork.setConstructArr((int)roadPos.x, (int)roadPos.y, roadObj);
            //Need to set the aqueduct if there is one
            roadObj.GetComponent<RoadInformation>().setAqueduct(road.GetComponent<RoadInformation>().getAqueduct());
        }

        //delete the old road so there aren't two roads in one spot
        //if the new nearbyRoadCount is 0, there is nothing to be deleted
        // because the straightRoad is still a straightRoad and it should keep
        // its rotation from before
        if (nearbyRoadCount > 0)
        {
            //road.GetComponent<RoadInformation>().destroyRoad();
            Destroy(road);
        }
    }
}
