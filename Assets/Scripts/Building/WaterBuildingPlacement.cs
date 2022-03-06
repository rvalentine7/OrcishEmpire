using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Lets the player place buildings that attach to water in the world.
 */
public class WaterBuildingPlacement : MonoBehaviour
{
    public int width;
    public int height;
    public Sprite possibleSpriteNS;
    public Sprite possibleSpriteSN;
    public Sprite possibleSpriteWE;
    public Sprite possibleSpriteEW;
    public Sprite impossibleSpriteNS;
    public Sprite impossibleSpriteSN;
    public Sprite impossibleSpriteWE;
    public Sprite impossibleSpriteEW;
    public GameObject building;
    public int buildingCost;
    private bool validPlacement;
    private World myWorld;
    private SpriteRenderer spriteRenderer;
    //private direction validDirection;

    private enum direction
    {
        up,
        down,
        left,
        right
    }

    /**
     * Initializes the BuildingPlacement class
     */
    void Start()
    {
        //validPlacement = true;
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //validDirection = direction.up;
    }

    /**
     * Allows the player to place a water building in a viable location.
     */
    void Update()
    {
        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Escape))
        {
            //exits out of construction mode if the right mouse button or escape is clicked
            Destroy(gameObject);
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = Mathf.RoundToInt(mousePos.x);
        mousePos.y = Mathf.RoundToInt(mousePos.y);
        mousePos.z = 0;

        //Need enough currency to build the building
        if (myWorld.getCurrency() < buildingCost)
        {
            validPlacement = false;
        }
        else
        {
            validPlacement = true;
        }
        //Buildings cannot be placed outside of the map
        if (mousePos.x < 0)
        {
            mousePos.x = 0;
            validPlacement = false;
        }
        if (mousePos.x > myWorld.mapSize - 1)
        {
            mousePos.x = myWorld.mapSize - 1;
            validPlacement = false;
        }
        if (mousePos.y < 0)
        {
            mousePos.y = 0;
            validPlacement = false;
        }
        if (mousePos.y > myWorld.mapSize - 1)
        {
            mousePos.y = myWorld.mapSize - 1;
            validPlacement = false;
        }
        
        //distance to location just outside object? floor(width / 2 + 1) floor(height / 2 + 1)

        //TODO: width and height can flip with water buildings (the part in the water)
        if (validPlacement && (mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) < 1 || mousePos.x + Mathf.FloorToInt(width / 2) >= myWorld.mapSize - 1
            || mousePos.y - Mathf.FloorToInt(height / 2) < 1 || mousePos.y + Mathf.FloorToInt(height / 2) >= myWorld.mapSize - 1))
        {
            validPlacement = false;
        }

        //Check if the building can be built
        checkPlacementOnTerrain(mousePos);
        
        if (!validPlacement)
        {
            if (width > height)
            {
                spriteRenderer.sprite = impossibleSpriteWE;
            }
            else
            {
                spriteRenderer.sprite = impossibleSpriteNS;
            }
        }

        //Place the object in the world upon left mouse click
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(0) && validPlacement)
        {
            if (mousePos.x > 0 && mousePos.x < myWorld.mapSize - 1 && mousePos.y > 0 && mousePos.y < myWorld.mapSize - 1)
            {
                transform.position = Vector2.Lerp(transform.position, mousePos, 1f);

                float adjustedX = 0f;
                float adjustedY = 0f;
                if (width % 2 == 0)
                {
                    adjustedX += 0.5f;
                }
                if (height % 2 == 0)
                {
                    adjustedY += 0.5f;
                }
                Vector2 buildingVec = new Vector2(mousePos.x + adjustedX, mousePos.y + adjustedY);
                //create the new building in the game world
                GameObject buildingObj = Instantiate(building, buildingVec, Quaternion.identity) as GameObject;
                myWorld.updateCurrency(-buildingCost);
                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < width; c++)
                    {
                        myWorld.constructNetwork.setConstructArr((int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c,
                            (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r, buildingObj);
                    }
                }
            }
        }

        if (mousePos.x - Mathf.FloorToInt(width / 2) + 1 > 0 && mousePos.x + Mathf.FloorToInt(width / 2) - 1 < myWorld.mapSize - 1
            && mousePos.y - Mathf.FloorToInt(height / 2) + 1 > 0 && mousePos.y + Mathf.FloorToInt(height / 2) - 1 < myWorld.mapSize - 1)
        {
            float adjustedX = 0f;
            float adjustedY = 0f;
            if (width % 2 == 0)
            {
                adjustedX += 0.5f;
            }
            if (height % 2 == 0)
            {
                adjustedY += 0.5f;
            }
            Vector2 buildingVec = new Vector2(mousePos.x + adjustedX, mousePos.y + adjustedY);
            transform.position = Vector2.Lerp(transform.position, buildingVec, 1f);
        }
    }

    /**
     * Checks if the building has valid placement on the terrain in the world
     * @param mousePos the position of the mouse
     */
    private void checkPlacementOnTerrain(Vector3 mousePos)
    {
        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();

        int tempHeight = 2;
        int tempWidth = 3;

        direction validWidthDirection = direction.down;

        //can't place a building on other constructs or water
        bool validPlacementWidth = true;
        int waterUnderRightCount = 0;
        int waterUnderLeftCount = 0;
        int r = 0;
        while (validPlacementWidth && r < tempHeight)
        {
            int c = 0;
            while (validPlacementWidth && c < tempWidth)
            {
                //Water should be under the far top xor bottom
                if ((c == tempWidth - 1 || c == 0)
                    && terrainArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r] != null
                    && myWorld.wateryTerrain.Contains(terrainArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r].tag)
                    && structureArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r] == null)
                {
                    if (c == tempWidth - 1)
                    {
                        waterUnderRightCount++;
                        if (waterUnderRightCount == 2)
                        {
                            spriteRenderer.sprite = possibleSpriteWE;
                            validWidthDirection = direction.left;
                        }
                    }
                    else
                    {
                        waterUnderLeftCount++;
                        if (waterUnderLeftCount == 2)
                        {
                            spriteRenderer.sprite = possibleSpriteEW;
                            validWidthDirection = direction.right;
                        }
                    }
                }
                //Should be normal, buildable terrain (the other 4 grid spaces)
                else
                {
                    //Mathf.CeilToInt(width / 2.0f - 1) finds the middle square and then that is subtracted from x to get to the edge to start checking the structure array
                    if (structureArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r] != null
                        && (!myWorld.buildableTerrain.Contains(structureArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r].tag)))
                    {
                        validPlacementWidth = false;
                    }
                    if (terrainArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r] != null
                        && (!myWorld.buildableTerrain.Contains(terrainArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r].tag)))
                    {
                        validPlacementWidth = false;
                    }
                }
                c++;
            }
            r++;
        }
        if (validWidthDirection == direction.down
                || (waterUnderRightCount > 0 && waterUnderLeftCount > 0)
                || (waterUnderRightCount == 1)
                || (waterUnderRightCount > 2)
                || (waterUnderLeftCount == 1)
                || (waterUnderLeftCount > 2))
        {
            validPlacementWidth = false;
        }
        
        tempHeight = 3;
        tempWidth = 2;
        bool validPlacementHeight = true;
        direction validHeightDirection = direction.left;
        int waterUnderBotCount = 0;
        int waterUnderTopCount = 0;
        r = 0;
        while (validPlacementHeight && r < tempHeight)
        {
            int c = 0;
            while (validPlacementHeight && c < tempWidth)
            {
                //Water should be under the far top xor bottom
                if ((r == tempHeight - 1 || r == 0)
                    && terrainArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r] != null
                    && myWorld.wateryTerrain.Contains(terrainArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r].tag)
                    && structureArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r] == null)
                {
                    if (r == tempHeight - 1)
                    {
                        waterUnderTopCount++;
                        if (waterUnderTopCount == 2)
                        {
                            spriteRenderer.sprite = possibleSpriteSN;
                            validHeightDirection = direction.up;
                        }
                    }
                    else
                    {
                        waterUnderBotCount++;
                        if (waterUnderBotCount == 2)
                        {
                            spriteRenderer.sprite = possibleSpriteNS;
                            validHeightDirection = direction.down;
                        }
                    }
                }
                //Should be normal, buildable terrain (the other 4 grid spaces)
                else
                {
                    //Mathf.CeilToInt(width / 2.0f - 1) finds the middle square and then that is subtracted from x to get to the edge to start checking the structure array
                    if (structureArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r] != null
                        && (!myWorld.buildableTerrain.Contains(structureArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r].tag)))
                    {
                        validPlacementHeight = false;
                    }
                    if (terrainArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r] != null
                        && (!myWorld.buildableTerrain.Contains(terrainArr[(int)mousePos.x - Mathf.CeilToInt(tempWidth / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(tempHeight / 2.0f - 1) + r].tag)))
                    {
                        validPlacementHeight = false;
                    }
                }
                c++;
            }
            r++;
        }
        if (validHeightDirection == direction.left
                || (waterUnderBotCount > 0 && waterUnderTopCount > 0)
                || (waterUnderBotCount == 1)
                || (waterUnderBotCount > 2)
                || (waterUnderTopCount == 1)
                || (waterUnderTopCount > 2))
        {
            validPlacementHeight = false;
        }

        if (validPlacementHeight && validPlacementWidth)
        {
            //Determine which one the mouse is closer to
            float closestDistanceToWater = float.MaxValue;
            Vector2 floatMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //"2" and "3" are for width and height.  when checking top/bot, 2 is width.  when checking left/right, 2 is height
            if (waterUnderBotCount == 2)
            {
                float distanceToWater = distance(floatMousePos, new Vector2(mousePos.x - (2 / 2.0f - 1), mousePos.y - (3 / 2.0f - 1)));
                if (distanceToWater < closestDistanceToWater)
                {
                    closestDistanceToWater = distanceToWater;
                    spriteRenderer.sprite = possibleSpriteNS;
                    width = 2;
                    height = 3;
                }
            }
            if (waterUnderRightCount == 2)
            {
                float distanceToWater = distance(floatMousePos, new Vector2(mousePos.x - (3 / 2.0f - 1) + 3 - 1, mousePos.y - (2 / 2.0f - 1)));
                if (distanceToWater < closestDistanceToWater)
                {
                    closestDistanceToWater = distanceToWater;
                    spriteRenderer.sprite = possibleSpriteWE;
                    width = 3;
                    height = 2;
                }
            }
            if (waterUnderLeftCount == 2)
            {
                float distanceToWater = distance(floatMousePos, new Vector2(mousePos.x - (3 / 2.0f - 1), mousePos.y - (2 / 2.0f - 1)));
                if (distanceToWater < closestDistanceToWater)
                {
                    closestDistanceToWater = distanceToWater;
                    spriteRenderer.sprite = possibleSpriteEW;
                    width = 3;
                    height = 2;
                }
            }
            if (waterUnderTopCount == 2)
            {
                float distanceToWater = distance(floatMousePos, new Vector2(mousePos.x - (2 / 2.0f - 1), mousePos.y - (3 / 2.0f - 1) + 3 - 1));
                if (distanceToWater < closestDistanceToWater)
                {
                    closestDistanceToWater = distanceToWater;
                    spriteRenderer.sprite = possibleSpriteSN;
                    width = 2;
                    height = 3;
                }
            }
        }
        else if (validPlacementHeight)
        {
            height = 3;
            width = 2;
        }
        else if (validPlacementWidth)
        {
            height = 2;
            width = 3;
        }
        else
        {
            validPlacement = false;
        }
    }

    /**
     * Checks the distance between two points.
     * @param point1 is the first point
     * @param point2 is the second point
     * @return the distance between the two points
     */
    float distance(Vector2 point1, Vector2 point2)
    {
        return Mathf.Sqrt((point2.x - point1.x)
            * (point2.x - point1.x) + (point2.y - point1.y)
            * (point2.y - point1.y));
    }
}
