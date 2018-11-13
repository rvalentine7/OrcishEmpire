using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BridgePlacement : MonoBehaviour {
    public GameObject bridgeObject;
    public int bridgeSegmentCost;
    public GameObject tempBridgeObject;
    public Sprite possibleSouthConnection;
    public Sprite possibleNorthConnection;
    public Sprite possibleWestConnection;
    public Sprite possibleEastConnection;
    public Sprite possibleVerticalMiddle;
    public Sprite possibleHorizontalMiddle;
    public Sprite impossibleSouthConnection;
    public Sprite impossibleNorthConnection;
    public Sprite impossibleWestConnection;
    public Sprite impossibleEastConnection;
    public Sprite impossibleVerticalMiddle;
    public Sprite impossibleHorizontalMiddle;

    private SpriteRenderer spriteRenderer;
    private bool validPlacement;
    private GameObject world;
    private World myWorld;
    private GameObject[,] terrainArr;
    private bool placedStartingLocation;
    private Dictionary<Vector2, GameObject> tempBridgeSegments;//The key is the position
    private Vector2 startingPosition;

    // Use this for initialization
    void Start () {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        validPlacement = true;
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        placedStartingLocation = false;
        tempBridgeSegments = new Dictionary<Vector2, GameObject>();
        startingPosition = new Vector2(0, 0);
    }
	
	// Update is called once per frame
	void Update () {
        /**
         1 Mouse click: place the starting piece
         2 Mouse click: place the last piece
            Fill everything inbetween
            At the time of the 2nd mouseclick, create the actual objects and make sure they have references to all the other pieces in the bridge
            Each piece of the bridge will cost gold (5g for each piece?)

        Will need to make roads connect to bridges.  When bridges are destroyed, the end points will need to update any connected roads

        If a bridge piece is destroyed, destroy the entire bridge (each piece has references to the other pieces)

        Right clicks/esc -> If no mouse clicks, cancel building.  If 1 mouse click, undo the starting place
        If clicking a UI element, should cancel this entirely
         */

        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Escape))
        {
            if (placedStartingLocation)
            {
                placedStartingLocation = false;
                //TODO: delete any temp segments that currently exist
                foreach (Vector2 tempSegmentKey in tempBridgeSegments.Keys)
                {
                    Destroy(tempBridgeSegments[tempSegmentKey]);
                }
                tempBridgeSegments = new Dictionary<Vector2, GameObject>();
            }
            else
            {
                //exits out of construction mode if the right mouse button or escape is clicked
                Destroy(gameObject);
            }
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = Mathf.RoundToInt(mousePos.x);
        mousePos.y = Mathf.RoundToInt(mousePos.y);
        mousePos.z = 0;

        validPlacement = false;
        bool valid = true;
        //Need enough currency to build the road
        if (myWorld.getCurrency() < bridgeSegmentCost * tempBridgeSegments.Count)
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
        //need to make sure there isn't a road/building already in the place where the bridge is currently located
        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();

        //bool valid = true;
        if ((mousePos.x <= 0 && mousePos.x >= myWorld.mapSize && mousePos.y <= 0 && mousePos.y >= myWorld.mapSize))
        {
            valid = false;
        }
        //can't place a bridge on other constructs
        if (valid && structureArr[(int)mousePos.x, (int)mousePos.y] != null
            && (!myWorld.buildableTerrain.Contains(structureArr[(int)mousePos.x, (int)mousePos.y].tag))
            && structureArr[(int)mousePos.x, (int)mousePos.y].GetComponent<Aqueduct>() == null)
        {
            valid = false;
        }
        if (valid && terrainArr[(int)mousePos.x, (int)mousePos.y] != null
            && !myWorld.buildableTerrain.Contains(terrainArr[(int)mousePos.x, (int)mousePos.y].tag))
        {
            valid = false;
        }
        //a bridge must be placed next to water
        bool nextToWater = false;
        if (valid && mousePos.x + 1 < myWorld.mapSize && terrainArr[(int)mousePos.x + 1, (int)mousePos.y] != null
            && myWorld.wateryTerrain.Contains(terrainArr[(int)mousePos.x + 1, (int)mousePos.y].tag))
        {
            nextToWater = true;
        }
        if (valid && !nextToWater && mousePos.x - 1 > 0 && terrainArr[(int)mousePos.x - 1, (int)mousePos.y] != null
            && myWorld.wateryTerrain.Contains(terrainArr[(int)mousePos.x - 1, (int)mousePos.y].tag))
        {
            nextToWater = true;
        }
        if (valid && !nextToWater && mousePos.y + 1 < myWorld.mapSize && terrainArr[(int)mousePos.x, (int)mousePos.y + 1] != null
            && myWorld.wateryTerrain.Contains(terrainArr[(int)mousePos.x, (int)mousePos.y + 1].tag))
        {
            nextToWater = true;
        }
        if (valid && !nextToWater && mousePos.y - 1 > 0 && terrainArr[(int)mousePos.x, (int)mousePos.y - 1] != null
            && myWorld.wateryTerrain.Contains(terrainArr[(int)mousePos.x, (int)mousePos.y - 1].tag))
        {
            nextToWater = true;
        }
        //TODO: need a check for the final segment's validity and everything inbetween (everything inbetween must be over water)

        validPlacement = valid && nextToWater && (structureArr[(int)mousePos.x, (int)mousePos.y] == null);
        //Update the preview (this will show segments inbetween the two endpoints and show possible/impossible sprites)
        updateBridgePreview(mousePos);

        //If the bridge is in a valid location and the left mouse is clicked, place an object
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(0) && validPlacement)
        {
            //when mouse down, can place a bridge object
            if (mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1)
            {
                transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
                //First click.  Place the starting point
                if (!placedStartingLocation)
                {
                    placedStartingLocation = true;

                    GameObject startingBridgeSegment = Instantiate(tempBridgeObject, mousePos, Quaternion.identity) as GameObject;
                    startingPosition = mousePos;
                    tempBridgeSegments.Add(mousePos, startingBridgeSegment);
                }
                //Second click.  Place the end point
                else
                {
                    //TODO: go through all of the segments and have them build an actual bridge segment and the location and destroy themselves
                    List<GameObject> confirmedBridgeSegments = new List<GameObject>();
                    foreach (GameObject segment in tempBridgeSegments.Values)
                    {
                        Vector2 segmentPosition = segment.transform.position;
                        GameObject confirmedBridgeSegment = Instantiate(bridgeObject, mousePos, Quaternion.identity) as GameObject;
                        confirmedBridgeSegments.Add(confirmedBridgeSegment);
                    }
                    //TODO: delete the temp segments

                    GameObject endingBridgeSegment = Instantiate(bridgeObject, mousePos, Quaternion.identity) as GameObject;
                    confirmedBridgeSegments.Add(endingBridgeSegment);
                    foreach (GameObject confirmedSegment in confirmedBridgeSegments)
                    {
                        //Giving each segment access to all of the other segments (for deleting purposes... if a segment is deleted, the others should be as well)
                        List<GameObject> otherSegments = new List<GameObject>(confirmedBridgeSegments);
                        otherSegments.Remove(confirmedSegment);
                        confirmedSegment.GetComponent<Bridge>().setConnectedBridgeObjects(otherSegments);
                    }
                    //TODO: delete this gameObject
                }
            }
        }
        if (mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1)
        {
            transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
        }
    }

    /**
     * 
     */
    private void updateBridgePreview(Vector3 mousePos)
    {
        //If there is only one sprite to worry about (the starting sprite)
        if (!placedStartingLocation)
        {
            if (validPlacement)
            {
                spriteRenderer.sprite = possibleSouthConnection;
            }
            else
            {
                spriteRenderer.sprite = impossibleSouthConnection;
            }
        }
        //If there are more sprites to worry about
        else
        {
            //Removing the old segments
            foreach (Vector2 tempSegmentKey in tempBridgeSegments.Keys)
            {
                if (startingPosition != tempSegmentKey)
                {
                    Destroy(tempBridgeSegments[tempSegmentKey]);
                }
            }
            GameObject firstSegment = tempBridgeSegments[startingPosition];
            tempBridgeSegments = new Dictionary<Vector2, GameObject>();
            tempBridgeSegments.Add(startingPosition, firstSegment);

            //Getting the difference in x and y to determine which direction the bridge is being built in
            Vector2 firstTempBridgeLocation = tempBridgeSegments[startingPosition].transform.position;
            float xDifference = mousePos.x - firstTempBridgeLocation.x;
            float yDifference = mousePos.y - firstTempBridgeLocation.y;
            bool buildVertical = Mathf.Abs(yDifference) > Mathf.Abs(xDifference);
            //TODO: update the preview of all of the gameobjects from the starting point to where the mouse is now (sprite at mouse icon is always a start/end point... everything inbetween is a middle segment)
            if (validPlacement)//TODO: these should be the exact same... change it so I just check at the end if it's possible or impossible
            {
                if (buildVertical)
                {
                    //Starting from south going up
                    if (yDifference > 0)
                    {
                        tempBridgeSegments[startingPosition].GetComponent<SpriteRenderer>().sprite = possibleSouthConnection;
                        spriteRenderer.sprite = possibleNorthConnection;
                        for (int i = 1; i < Mathf.RoundToInt(yDifference); i++)
                        {
                            Vector2 segmentPosition = new Vector2(firstTempBridgeLocation.x, firstTempBridgeLocation.y + i);
                            if (!tempBridgeSegments.ContainsKey(segmentPosition))
                            {
                                GameObject middleBridgeSegment = Instantiate(tempBridgeObject, segmentPosition, Quaternion.identity) as GameObject;
                                middleBridgeSegment.GetComponent<SpriteRenderer>().sprite = possibleVerticalMiddle;
                                tempBridgeSegments.Add(segmentPosition, middleBridgeSegment);
                            }
                        }
                    }
                    //Starting from north going down
                    else
                    {

                    }
                }
                else
                {
                    //Starting from left going right
                    if (xDifference > 0)
                    {

                    }
                    //Starting from right going left
                    else
                    {

                    }
                }
            }
            else
            {
                if (buildVertical)
                {
                    //Starting from south going up
                    if (yDifference > 0)
                    {

                    }
                    //Starting from north going down
                    else
                    {

                    }
                }
                else
                {
                    //Starting from left going right
                    if (xDifference > 0)
                    {

                    }
                    //Starting from right going left
                    else
                    {

                    }
                }
            }
        }
    }
}
