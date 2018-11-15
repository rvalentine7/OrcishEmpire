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
    private bool placedStartingLocation;
    private Dictionary<Vector2, GameObject> tempBridgeSegments;//The key is the position
    private Vector2 startingPosition;
    private Vector2 endingPosition;

    // Use this for initialization
    void Start () {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        validPlacement = true;
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        placedStartingLocation = false;
        tempBridgeSegments = new Dictionary<Vector2, GameObject>();
        startingPosition = new Vector2(0, 0);
        endingPosition = new Vector2(-1, -1);
    }
	
	// Update is called once per frame
	void Update () {
        /**
         1 Mouse click: place the starting piece
         2 Mouse click: place the last piece
            Fill everything inbetween
            At the time of the 2nd mouseclick, create the actual objects and make sure they have references to all the other pieces in the bridge
            Each piece of the bridge will cost gold
         */

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (placedStartingLocation)
            {
                placedStartingLocation = false;
                //Delete any temp segments that currently exist
                foreach (Vector2 tempSegmentKey in tempBridgeSegments.Keys)
                {
                    Destroy(tempBridgeSegments[tempSegmentKey]);
                }
                tempBridgeSegments = new Dictionary<Vector2, GameObject>();
                spriteRenderer.enabled = true;
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
        //can't place a bridge on other constructs.  if the starting location has been placed, don't worry about validity at mouse position
        if (valid && !placedStartingLocation && structureArr[(int)mousePos.x, (int)mousePos.y] != null
            && (!myWorld.buildableTerrain.Contains(structureArr[(int)mousePos.x, (int)mousePos.y].tag))
            && structureArr[(int)mousePos.x, (int)mousePos.y].GetComponent<Aqueduct>() == null)
        {
            valid = false;
        }
        if (valid && !placedStartingLocation && terrainArr[(int)mousePos.x, (int)mousePos.y] != null
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
        if (!placedStartingLocation && !nextToWater)
        {
            valid = false;
        }
        //If the first location has been placed, we now care more about if a valid bridge is being formed (must have a middle segment)
        if (valid && placedStartingLocation && tempBridgeSegments.Count <= 2)
        {
            valid = false;
        }
        //If endingPosition is not (-1, -1), there are multiple segments and they potentially create a bridge.  Need to see if the segments are in valid locations
        bool segmentsInValidPositions = true;
        if (valid && endingPosition != new Vector2(-1, -1))
        {
            //Check for the final segment's validity and everything inbetween (everything inbetween must be over water)
            foreach (Vector2 segmentPosition in tempBridgeSegments.Keys)
            {
                //Starting position was already determined to be valid
                //Checking if ending position is next to water
                if (segmentPosition == endingPosition)
                {
                    //Checking if the end segment is in a buildable location
                    if (terrainArr[(int)endingPosition.x, (int)endingPosition.y] == null
                        || !myWorld.buildableTerrain.Contains(terrainArr[(int)endingPosition.x, (int)endingPosition.y].tag)
                        || structureArr[(int)endingPosition.x, (int)endingPosition.y] != null)
                    {
                        valid = false;
                    }

                    bool endNextToWater = false;
                    if (valid && endingPosition.x + 1 < myWorld.mapSize && terrainArr[(int)endingPosition.x + 1, (int)endingPosition.y] != null
                        && myWorld.wateryTerrain.Contains(terrainArr[(int)endingPosition.x + 1, (int)endingPosition.y].tag))
                    {
                        endNextToWater = true;
                    }
                    if (valid && !endNextToWater && endingPosition.x - 1 > 0 && terrainArr[(int)endingPosition.x - 1, (int)endingPosition.y] != null
                        && myWorld.wateryTerrain.Contains(terrainArr[(int)endingPosition.x - 1, (int)endingPosition.y].tag))
                    {
                        endNextToWater = true;
                    }
                    if (valid && !endNextToWater && endingPosition.y + 1 < myWorld.mapSize && terrainArr[(int)endingPosition.x, (int)endingPosition.y + 1] != null
                        && myWorld.wateryTerrain.Contains(terrainArr[(int)endingPosition.x, (int)endingPosition.y + 1].tag))
                    {
                        endNextToWater = true;
                    }
                    if (valid && !endNextToWater && endingPosition.y - 1 > 0 && terrainArr[(int)endingPosition.x, (int)endingPosition.y - 1] != null
                        && myWorld.wateryTerrain.Contains(terrainArr[(int)endingPosition.x, (int)endingPosition.y - 1].tag))
                    {
                        endNextToWater = true;
                    }
                    if (!endNextToWater)
                    {
                        segmentsInValidPositions = false;
                    }
                }
                //Checking if middle segments are over water or over another structure (a bridge over the water) or next to another bridge
                else if (segmentPosition != startingPosition && (terrainArr[(int)segmentPosition.x, (int)segmentPosition.y] == null
                    || terrainArr[(int)segmentPosition.x, (int)segmentPosition.y].tag != World.WATER
                    || structureArr[(int)segmentPosition.x, (int)segmentPosition.y] != null
                    || (structureArr[(int)segmentPosition.x + 1, (int)segmentPosition.y] != null
                        && (structureArr[(int)segmentPosition.x + 1, (int)segmentPosition.y].tag == World.HIGH_BRIDGE
                        || structureArr[(int)segmentPosition.x + 1, (int)segmentPosition.y].tag == World.LOW_BRIDGE))
                    || (structureArr[(int)segmentPosition.x - 1, (int)segmentPosition.y] != null
                        && (structureArr[(int)segmentPosition.x - 1, (int)segmentPosition.y].tag == World.HIGH_BRIDGE
                        || structureArr[(int)segmentPosition.x - 1, (int)segmentPosition.y].tag == World.LOW_BRIDGE))
                    || (structureArr[(int)segmentPosition.x, (int)segmentPosition.y + 1] != null
                        && (structureArr[(int)segmentPosition.x, (int)segmentPosition.y + 1].tag == World.HIGH_BRIDGE
                        || structureArr[(int)segmentPosition.x, (int)segmentPosition.y + 1].tag == World.LOW_BRIDGE))
                    || (structureArr[(int)segmentPosition.x, (int)segmentPosition.y - 1] != null
                        && (structureArr[(int)segmentPosition.x, (int)segmentPosition.y - 1].tag == World.HIGH_BRIDGE
                        || structureArr[(int)segmentPosition.x, (int)segmentPosition.y - 1].tag == World.LOW_BRIDGE))))
                {
                    segmentsInValidPositions = false;
                }
            }
        }
        //If there is no ending segment and the segment that does exist is not next to water
        else if (valid && !nextToWater)
        {
            valid = false;
        }

        validPlacement = valid && segmentsInValidPositions && (structureArr[(int)mousePos.x, (int)mousePos.y] == null);
        //Update the preview (this will show segments inbetween the two endpoints and show possible/impossible sprites)
        updateBridgePreview(mousePos);

        //If the bridge is in a valid location and the left mouse is clicked, place an object
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0) && validPlacement)
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
                    //The end segment will be shown connected to the other segments (hiding the object attached to the mouse... re-enabled if the starting point is cancelled)
                    spriteRenderer.enabled = false;
                    tempBridgeSegments.Add(mousePos, startingBridgeSegment);
                }
                //Second click.  Place the end point
                else if (new Vector2(mousePos.x, mousePos.y) != startingPosition)
                {
                    //Update the city's currency for building the bridge
                    myWorld.updateCurrency(-(bridgeSegmentCost * tempBridgeSegments.Count));

                    //Go through all of the segments and have them build an actual bridge segment and the location and destroy themselves
                    float xDifference = startingPosition.x - endingPosition.x;
                    float yDifference = startingPosition.y - endingPosition.y;
                    List<GameObject> confirmedBridgeSegments = new List<GameObject>();
                    foreach (GameObject segment in tempBridgeSegments.Values)
                    {
                        Vector2 segmentPosition = segment.transform.position;
                        GameObject confirmedBridgeSegment = Instantiate(bridgeObject, segmentPosition, Quaternion.identity) as GameObject;
                        confirmedBridgeSegments.Add(confirmedBridgeSegment);
                        structureArr[(int)segmentPosition.x, (int)segmentPosition.y] = confirmedBridgeSegment;

                        //Set the sprite of the finalized bridge segment
                        string confirmedSegmentSprite = Bridge.HORIZONTAL;
                        if (xDifference < 0)
                        {
                            if (segmentPosition == startingPosition)
                            {
                                confirmedSegmentSprite = Bridge.WEST_END;
                            }
                            else if (segmentPosition == endingPosition)
                            {
                                confirmedSegmentSprite = Bridge.EAST_END;
                            }
                            else
                            {
                                confirmedSegmentSprite = Bridge.HORIZONTAL;
                            }
                        }
                        else if (xDifference > 0)
                        {
                            if (segmentPosition == startingPosition)
                            {
                                confirmedSegmentSprite = Bridge.EAST_END;
                            }
                            else if (segmentPosition == endingPosition)
                            {
                                confirmedSegmentSprite = Bridge.WEST_END;
                            }
                            else
                            {
                                confirmedSegmentSprite = Bridge.HORIZONTAL;
                            }
                        }
                        else if (yDifference < 0)
                        {
                            if (segmentPosition == startingPosition)
                            {
                                confirmedSegmentSprite = Bridge.SOUTH_END;
                            }
                            else if (segmentPosition == endingPosition)
                            {
                                confirmedSegmentSprite = Bridge.NORTH_END;
                            }
                            else
                            {
                                confirmedSegmentSprite = Bridge.VERTICAL;
                            }
                        }
                        else if (yDifference > 0)
                        {
                            if (segmentPosition == startingPosition)
                            {
                                confirmedSegmentSprite = Bridge.NORTH_END;
                            }
                            else if (segmentPosition == endingPosition)
                            {
                                confirmedSegmentSprite = Bridge.SOUTH_END;
                            }
                            else
                            {
                                confirmedSegmentSprite = Bridge.VERTICAL;
                            }
                        }
                        confirmedBridgeSegment.GetComponent<Bridge>().setSpriteAppearance(confirmedSegmentSprite);

                        Destroy(segment);
                    }

                    foreach (GameObject confirmedSegment in confirmedBridgeSegments)
                    {
                        //Giving each segment access to all of the other segments (for deleting purposes... if a segment is deleted, the others should be as well)
                        List<GameObject> otherSegments = new List<GameObject>(confirmedBridgeSegments);
                        otherSegments.Remove(confirmedSegment);
                        confirmedSegment.GetComponent<Bridge>().setConnectedBridgeObjects(otherSegments);
                    }

                    //Start over from square one now that a bridge has been created
                    placedStartingLocation = false;
                    tempBridgeSegments = new Dictionary<Vector2, GameObject>();
                    spriteRenderer.enabled = true;
                }
            }
        }
        if (!placedStartingLocation && mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1)
        {
            transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
        }
    }

    /**
     * Displays all of the segments of a bridge and shows whether it is possible
     * to create the bridge.
     * @param mousePos the position of the mouse
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
            endingPosition = new Vector2(-1, -1);
            GameObject firstSegment = tempBridgeSegments[startingPosition];
            tempBridgeSegments = new Dictionary<Vector2, GameObject>();
            tempBridgeSegments.Add(startingPosition, firstSegment);

            //Getting the difference in x and y to determine which direction the bridge is being built in
            Vector2 firstTempBridgeLocation = tempBridgeSegments[startingPosition].transform.position;
            float xDifference = mousePos.x - firstTempBridgeLocation.x;
            float yDifference = mousePos.y - firstTempBridgeLocation.y;
            bool buildVertical = Mathf.Abs(yDifference) > Mathf.Abs(xDifference);
            //Updating the preview of all of the gameobjects from the starting point to the x/y location of the mouse
            if (buildVertical)
            {
                //Starting from south going up
                if (yDifference > 0)
                {
                    tempBridgeSegments[startingPosition].GetComponent<SpriteRenderer>().sprite = validPlacement ? possibleSouthConnection : impossibleSouthConnection;
                    spriteRenderer.sprite = possibleNorthConnection;
                    for (int i = 1; i < Mathf.RoundToInt(yDifference); i++)
                    {
                        Vector2 segmentPosition = new Vector2(firstTempBridgeLocation.x, firstTempBridgeLocation.y + i);
                        if (!tempBridgeSegments.ContainsKey(segmentPosition))
                        {
                            GameObject middleBridgeSegment = Instantiate(tempBridgeObject, segmentPosition, Quaternion.identity) as GameObject;
                            middleBridgeSegment.GetComponent<SpriteRenderer>().sprite = validPlacement ? possibleVerticalMiddle : impossibleVerticalMiddle;
                            tempBridgeSegments.Add(segmentPosition, middleBridgeSegment);
                        }
                    }
                    Vector2 endPosition = new Vector2(firstTempBridgeLocation.x, firstTempBridgeLocation.y + Mathf.RoundToInt(yDifference));
                    if (!tempBridgeSegments.ContainsKey(endPosition))
                    {
                        GameObject endBridgeSegment = Instantiate(tempBridgeObject, endPosition, Quaternion.identity) as GameObject;
                        endBridgeSegment.GetComponent<SpriteRenderer>().sprite = validPlacement ? possibleNorthConnection : impossibleNorthConnection;
                        tempBridgeSegments.Add(endPosition, endBridgeSegment);
                        endingPosition = endPosition;
                    }
                }
                //Starting from north going down
                else
                {
                    tempBridgeSegments[startingPosition].GetComponent<SpriteRenderer>().sprite = validPlacement ? possibleNorthConnection : impossibleNorthConnection;
                    spriteRenderer.sprite = possibleNorthConnection;
                    for (int i = 1; i < Mathf.Abs(Mathf.RoundToInt(yDifference)); i++)
                    {
                        Vector2 segmentPosition = new Vector2(firstTempBridgeLocation.x, firstTempBridgeLocation.y - i);
                        if (!tempBridgeSegments.ContainsKey(segmentPosition))
                        {
                            GameObject middleBridgeSegment = Instantiate(tempBridgeObject, segmentPosition, Quaternion.identity) as GameObject;
                            middleBridgeSegment.GetComponent<SpriteRenderer>().sprite = validPlacement ? possibleVerticalMiddle : impossibleVerticalMiddle;
                            tempBridgeSegments.Add(segmentPosition, middleBridgeSegment);
                        }
                    }
                    Vector2 endPosition = new Vector2(firstTempBridgeLocation.x, firstTempBridgeLocation.y + Mathf.RoundToInt(yDifference));
                    if (!tempBridgeSegments.ContainsKey(endPosition))
                    {
                        GameObject endBridgeSegment = Instantiate(tempBridgeObject, endPosition, Quaternion.identity) as GameObject;
                        endBridgeSegment.GetComponent<SpriteRenderer>().sprite = validPlacement ? possibleSouthConnection : impossibleSouthConnection;
                        tempBridgeSegments.Add(endPosition, endBridgeSegment);
                        endingPosition = endPosition;
                    }
                }
            }
            else
            {
                //Starting from left going right
                if (xDifference > 0)
                {
                    tempBridgeSegments[startingPosition].GetComponent<SpriteRenderer>().sprite = validPlacement ? possibleWestConnection : impossibleWestConnection;
                    spriteRenderer.sprite = possibleNorthConnection;
                    for (int i = 1; i < Mathf.RoundToInt(xDifference); i++)
                    {
                        Vector2 segmentPosition = new Vector2(firstTempBridgeLocation.x + i, firstTempBridgeLocation.y);
                        if (!tempBridgeSegments.ContainsKey(segmentPosition))
                        {
                            GameObject middleBridgeSegment = Instantiate(tempBridgeObject, segmentPosition, Quaternion.identity) as GameObject;
                            middleBridgeSegment.GetComponent<SpriteRenderer>().sprite = validPlacement ? possibleHorizontalMiddle : impossibleHorizontalMiddle;
                            tempBridgeSegments.Add(segmentPosition, middleBridgeSegment);
                        }
                    }
                    Vector2 endPosition = new Vector2(firstTempBridgeLocation.x + Mathf.RoundToInt(xDifference), firstTempBridgeLocation.y);
                    if (!tempBridgeSegments.ContainsKey(endPosition))
                    {
                        GameObject endBridgeSegment = Instantiate(tempBridgeObject, endPosition, Quaternion.identity) as GameObject;
                        endBridgeSegment.GetComponent<SpriteRenderer>().sprite = validPlacement ? possibleEastConnection : impossibleEastConnection;
                        tempBridgeSegments.Add(endPosition, endBridgeSegment);
                        endingPosition = endPosition;
                    }
                }
                //Starting from right going left
                else
                {
                    tempBridgeSegments[startingPosition].GetComponent<SpriteRenderer>().sprite = validPlacement ? possibleEastConnection : impossibleEastConnection;
                    spriteRenderer.sprite = possibleNorthConnection;
                    for (int i = 1; i < Mathf.Abs(Mathf.RoundToInt(xDifference)); i++)
                    {
                        Vector2 segmentPosition = new Vector2(firstTempBridgeLocation.x - i, firstTempBridgeLocation.y);
                        if (!tempBridgeSegments.ContainsKey(segmentPosition))
                        {
                            GameObject middleBridgeSegment = Instantiate(tempBridgeObject, segmentPosition, Quaternion.identity) as GameObject;
                            middleBridgeSegment.GetComponent<SpriteRenderer>().sprite = validPlacement ? possibleHorizontalMiddle : impossibleHorizontalMiddle;
                            tempBridgeSegments.Add(segmentPosition, middleBridgeSegment);
                        }
                    }
                    Vector2 endPosition = new Vector2(firstTempBridgeLocation.x + Mathf.RoundToInt(xDifference), firstTempBridgeLocation.y);
                    if (!tempBridgeSegments.ContainsKey(endPosition))
                    {
                        GameObject endBridgeSegment = Instantiate(tempBridgeObject, endPosition, Quaternion.identity) as GameObject;
                        endBridgeSegment.GetComponent<SpriteRenderer>().sprite = validPlacement ? possibleWestConnection : impossibleWestConnection;
                        tempBridgeSegments.Add(endPosition, endBridgeSegment);
                        endingPosition = endPosition;
                    }
                }
            }
        }
    }
}
