using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private GameObject world;
    private World myWorld;
    private SpriteRenderer spriteRenderer;
    private direction validDirection;

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
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        validDirection = direction.up;
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
        //Rotate the building
        if (Input.GetKey(KeyCode.R))
        {
            int tempWidth = this.width;
            width = height;
            height = tempWidth;
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
        
        //can't place a building on other constructs or water
        int waterUnderPlacementCount = 0;
        int r = 0;
        while (validPlacement && r < height)
        {
            int c = 0;
            while (validPlacement && c < width)
            {
                //Water should be under the far top or bottom (exclusive or)
                if (height > width && (r == height - 1 || r == 0)
                    && terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] != null
                    && myWorld.wateryTerrain.Contains(terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r].tag)
                    && structureArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] == null)
                {
                    waterUnderPlacementCount++;
                    if (waterUnderPlacementCount == 2)
                    {
                        if (r == height - 1)
                        {
                            spriteRenderer.sprite = possibleSpriteNS;
                            validDirection = direction.up;
                        }
                        else
                        {
                            spriteRenderer.sprite = possibleSpriteSN;
                            validDirection = direction.down;
                        }
                    }
                }
                //Water should be under the far right or left (exclusive or)
                else if (width > height && (c == width - 1 || c == 0)
                    && terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] != null
                    && myWorld.wateryTerrain.Contains(terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r].tag)
                    && structureArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] == null)
                {
                    waterUnderPlacementCount++;
                    if (waterUnderPlacementCount == 2)
                    {
                        if (c == width - 1)
                        {
                            spriteRenderer.sprite = possibleSpriteWE;
                            validDirection = direction.left;
                        }
                        else
                        {
                            spriteRenderer.sprite = possibleSpriteEW;
                            validDirection = direction.right;
                        }
                    }
                }
                //Should be normal, buildable terrain
                else
                {
                    //Mathf.CeilToInt(width / 2.0f - 1) finds the middle square and then that is subtracted from x to get to the edge to start checking the structure array
                    if (structureArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] != null
                        && (!myWorld.buildableTerrain.Contains(structureArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r].tag)))
                    {
                        validPlacement = false;
                    }
                    if (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] != null
                        && (!myWorld.buildableTerrain.Contains(terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r].tag)))
                    {
                        validPlacement = false;
                    }
                }
                c++;
            }
            r++;
        }

        //Need to make sure this doesn't cut off a water route (needs to go 1 row/column further than normal buildings and check if that spot is land or a building)
        if (validPlacement)
        {
            if (height > width)
            {
                int c = 0;
                while (validPlacement && c < width)
                {
                    //Check 1 beyond the valid end (r = -1 && r = height)
                    r = -1;
                    if (validDirection == direction.up
                        && (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] != null
                        && (!myWorld.wateryTerrain.Contains(terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r].tag)))
                        || structureArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] != null)
                    {
                        validPlacement = false;
                    }
                    r = height;
                    if (validDirection == direction.down
                        && (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] != null
                        && (!myWorld.wateryTerrain.Contains(terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r].tag)))
                        || structureArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] != null)
                    {
                        validPlacement = false;
                    }
                    c++;
                }
            }
            else
            {
                r = 0;
                while (validPlacement && r < height)
                {
                    //Check 1 beyond the valid end (c = -1 && c = width)
                    int c = -1;
                    if (validDirection == direction.right
                        && (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] != null
                        && (!myWorld.wateryTerrain.Contains(terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r].tag)))
                        || structureArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] != null)
                    {
                        validPlacement = false;
                    }
                    c = width;
                    if (validDirection == direction.left
                        && (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] != null
                        && (!myWorld.wateryTerrain.Contains(terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r].tag)))
                        || structureArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] != null)
                    {
                        validPlacement = false;
                    }
                    r++;
                }
            }
        }

        if (waterUnderPlacementCount != 2)
        {
            validPlacement = false;
        }
    }
}
