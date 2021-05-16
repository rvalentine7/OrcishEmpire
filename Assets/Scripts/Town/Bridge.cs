using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bridge is one segment of a larger bridge.  It keeps track of other bridge segments
/// for the purposes of destroying the overall bridge.
/// </summary>
public class Bridge : MonoBehaviour {
    public Sprite northConnection;
    public Sprite southConnection;
    public Sprite westConnection;
    public Sprite eastConnection;
    public Sprite verticalMiddle;
    public Sprite horizontalMiddle;

    public const string VERTICAL = "Vertical";
    public const string HORIZONTAL = "Horizontal";
    public const string NORTH_END = "North End";
    public const string SOUTH_END = "South End";
    public const string WEST_END = "West End";
    public const string EAST_END = "East End";

    private List<GameObject> connectedBridgeObjects;
    private GameObject world;
    private World myWorld;
    GameObject[,] structureArr;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start () {
        world = GameObject.Find(World.WORLD_INFORMATION);
        myWorld = world.GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();

        updateNearbyRoads(gameObject);
    }

    /// <summary>
    /// Sets references to the other segments of the bridge that this bridge is a segment of.
    /// This is used for destroying all of the bridge segments when a bridge segment is destroyed.
    /// </summary>
    /// <param name="connectedBridgeObjects">a list of the other bridge segments</param>
    public void setConnectedBridgeObjects(List<GameObject> connectedBridgeObjects)
    {
        this.connectedBridgeObjects = connectedBridgeObjects;
    }

    /**
     * Sets the sprite used by the gameobject
     * @param appearance the string description of what sprite to set
     */
    public void setSpriteAppearance(string appearance)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (appearance.Equals(VERTICAL))
        {
            spriteRenderer.sprite = verticalMiddle;
        }
        else if (appearance.Equals(HORIZONTAL))
        {
            spriteRenderer.sprite = horizontalMiddle;
        }
        else if (appearance.Equals(NORTH_END))
        {
            spriteRenderer.sprite = northConnection;
        }
        else if (appearance.Equals(SOUTH_END))
        {
            spriteRenderer.sprite = southConnection;
        }
        else if (appearance.Equals(WEST_END))
        {
            spriteRenderer.sprite = westConnection;
        }
        else if (appearance.Equals(EAST_END))
        {
            spriteRenderer.sprite = eastConnection;
        }
    }

    /// <summary>
    /// Updates the connecting roads
    /// </summary>
    /// <param name="bridgeSegment">the bridge segment we are updating the roads around</param>
    public void updateNearbyRoads(GameObject bridgeSegment)
    {
        if ((int)bridgeSegment.transform.position.y + 1 < myWorld.mapSize
            && structureArr[(int)bridgeSegment.transform.position.x, (int)bridgeSegment.transform.position.y + 1] != null
            && structureArr[(int)bridgeSegment.transform.position.x, (int)bridgeSegment.transform.position.y + 1].tag == World.ROAD)
        {
            structureArr[(int)bridgeSegment.transform.position.x, (int)bridgeSegment.transform.position.y + 1].GetComponent<RoadInformation>().updateRoadConnection();
        }
        if ((int)bridgeSegment.transform.position.y - 1 > 0
            && structureArr[(int)bridgeSegment.transform.position.x, (int)bridgeSegment.transform.position.y - 1] != null
            && structureArr[(int)bridgeSegment.transform.position.x, (int)bridgeSegment.transform.position.y - 1].tag == World.ROAD)
        {
            structureArr[(int)bridgeSegment.transform.position.x, (int)bridgeSegment.transform.position.y - 1].GetComponent<RoadInformation>().updateRoadConnection();
        }
        if ((int)bridgeSegment.transform.position.x - 1 > 0
            && structureArr[(int)bridgeSegment.transform.position.x - 1, (int)bridgeSegment.transform.position.y] != null
            && structureArr[(int)bridgeSegment.transform.position.x - 1, (int)bridgeSegment.transform.position.y].tag == World.ROAD)
        {
            structureArr[(int)bridgeSegment.transform.position.x - 1, (int)bridgeSegment.transform.position.y].GetComponent<RoadInformation>().updateRoadConnection();
        }
        if ((int)bridgeSegment.transform.position.x + 1 < myWorld.mapSize
            && structureArr[(int)bridgeSegment.transform.position.x + 1, (int)bridgeSegment.transform.position.y] != null
            && structureArr[(int)bridgeSegment.transform.position.x + 1, (int)bridgeSegment.transform.position.y].tag == World.ROAD)
        {
            structureArr[(int)bridgeSegment.transform.position.x + 1, (int)bridgeSegment.transform.position.y].GetComponent<RoadInformation>().updateRoadConnection();
        }
    }

    /// <summary>
    /// Destroys this bridge segment and all of the other bridge segments from the same bridge
    /// </summary>
    public void destroyBridge()
    {
        structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();

        List<WaterTile> waterTilesUnderBridge = new List<WaterTile>();
        foreach (GameObject connectedBridge in connectedBridgeObjects)
        {
            //Getting the water under the bridge so that the section numbers can be updated
            if (terrainArr[(int)connectedBridge.transform.position.x, (int)connectedBridge.transform.position.y].tag.Equals(World.WATER))
            {
                waterTilesUnderBridge.Add(terrainArr[(int)connectedBridge.transform.position.x, (int)connectedBridge.transform.position.y].GetComponent<WaterTile>());
            }

            structureArr[(int)connectedBridge.transform.position.x, (int)connectedBridge.transform.position.y] = null;
            updateNearbyRoads(connectedBridge);
            Destroy(connectedBridge);
        }

        structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y] = null;
        updateNearbyRoads(gameObject);

        //Updating the water sections under and around the bridge
        updateWaterSections(terrainArr, waterTilesUnderBridge);

        Destroy(gameObject);
    }

    /// <summary>
    /// Updates the water sections so that only connected bodies of water have the same section number
    /// </summary>
    /// <param name="terrainArr">The grid for the world that has the terrain saved in it</param>
    /// <param name="startingWaterTiles">The water tiles under the bridge</param>
    private void updateWaterSections(GameObject[,] terrainArr, List<WaterTile> startingWaterTiles)
    {
        //updatedTiles is all of the tiles that get their section number updated due to the new bridge
        List<WaterTile> updatedTiles = new List<WaterTile>();
        int lowestWaterSectionNum = int.MaxValue;
        //Updating the sections for the water tiles around the bridge
        foreach (WaterTile startingWaterTile in startingWaterTiles)
        {
            //If the tile hasn't already been accounted for in the floodfill
            if (!updatedTiles.Contains(startingWaterTile))
            {
                List<WaterTile> floodFillList = new List<WaterTile>();
                floodFillList.Add(startingWaterTile);
                //Run floodfill with the tile if it hasn't already been updated
                //fillTiles is the tiles from the current fill
                List<WaterTile> fillTiles = new List<WaterTile>();
                while (floodFillList.Count > 0)
                {
                    WaterTile nextTile = floodFillList[0];
                    floodFillList.Remove(nextTile);
                    //Don't want to repeat tiles that have already been checked
                    if (!updatedTiles.Contains(nextTile))
                    {
                        updatedTiles.Add(nextTile);
                        fillTiles.Add(nextTile);

                        //Adding adjacent water tiles
                        Vector3 waterTilePos = nextTile.transform.position;
                        if (waterTilePos.x + 1 < myWorld.mapSize - 1)
                        {
                            GameObject secondTerrainObj = terrainArr[(int)waterTilePos.x + 1, (int)waterTilePos.y];
                            if (myWorld.wateryTerrain.Contains(secondTerrainObj.tag))
                            {
                                WaterTile potentialTile = secondTerrainObj.GetComponent<WaterTile>();
                                if (potentialTile.getWaterSectionNum() != -1)
                                {
                                    floodFillList.Add(potentialTile);
                                    int waterTileSectionNum = potentialTile.getWaterSectionNum();
                                    if (waterTileSectionNum < lowestWaterSectionNum)
                                    {
                                        lowestWaterSectionNum = waterTileSectionNum;
                                    }
                                }
                            }
                        }
                        if (waterTilePos.x - 1 > 0)
                        {
                            GameObject thirdTerrainObj = terrainArr[(int)waterTilePos.x - 1, (int)waterTilePos.y];
                            if (myWorld.wateryTerrain.Contains(thirdTerrainObj.tag))
                            {
                                WaterTile potentialTile = thirdTerrainObj.GetComponent<WaterTile>();
                                if (potentialTile.getWaterSectionNum() != -1)
                                {
                                    floodFillList.Add(potentialTile);
                                    int waterTileSectionNum = potentialTile.getWaterSectionNum();
                                    if (waterTileSectionNum < lowestWaterSectionNum)
                                    {
                                        lowestWaterSectionNum = waterTileSectionNum;
                                    }
                                }
                            }
                        }
                        if (waterTilePos.y + 1 < myWorld.mapSize - 1)
                        {
                            GameObject fourthTerrainObj = terrainArr[(int)waterTilePos.x, (int)waterTilePos.y + 1];
                            if (myWorld.wateryTerrain.Contains(fourthTerrainObj.tag))
                            {
                                WaterTile potentialTile = fourthTerrainObj.GetComponent<WaterTile>();
                                if (potentialTile.getWaterSectionNum() != -1)
                                {
                                    floodFillList.Add(potentialTile);
                                    int waterTileSectionNum = potentialTile.getWaterSectionNum();
                                    if (waterTileSectionNum < lowestWaterSectionNum)
                                    {
                                        lowestWaterSectionNum = waterTileSectionNum;
                                    }
                                }
                            }
                        }
                        if (waterTilePos.y - 1 > 0)
                        {
                            GameObject fifthTerrainObj = terrainArr[(int)waterTilePos.x, (int)waterTilePos.y - 1];
                            if (myWorld.wateryTerrain.Contains(fifthTerrainObj.tag))
                            {
                                WaterTile potentialTile = fifthTerrainObj.GetComponent<WaterTile>();
                                if (potentialTile.getWaterSectionNum() != -1)
                                {
                                    floodFillList.Add(potentialTile);
                                    int waterTileSectionNum = potentialTile.getWaterSectionNum();
                                    if (waterTileSectionNum < lowestWaterSectionNum)
                                    {
                                        lowestWaterSectionNum = waterTileSectionNum;
                                    }
                                }
                            }
                        }
                    }
                }

                //Updating water section numbers for the fill tiles
                int newFillCount = fillTiles.Count;
                if (newFillCount > 0)
                {
                    //Updating all of the water tiles that are now part of the same section to be the lowest non-negative water section number of those sections
                    foreach (WaterTile fillTile in fillTiles)
                    {
                        fillTile.setWaterSectionNum(lowestWaterSectionNum);
                    }
                }
            }
        }
    }
}
