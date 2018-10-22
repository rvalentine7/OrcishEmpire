using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReservoirPlacement : MonoBehaviour {
    public int width;
    public int height;

    public Sprite impossibleEmptySprite;
    public Sprite impossibleFilledSprite;
    public Sprite possibleEmptySprite;
    public Sprite possibleEmptySprite1N;
    public Sprite possibleEmptySprite1S;
    public Sprite possibleEmptySprite1W;
    public Sprite possibleEmptySprite1E;
    public Sprite possibleEmptySprite2NS;
    public Sprite possibleEmptySprite2NW;
    public Sprite possibleEmptySprite2NE;
    public Sprite possibleEmptySprite2SW;
    public Sprite possibleEmptySprite2SE;
    public Sprite possibleEmptySprite2WE;
    public Sprite possibleEmptySprite3NSW;
    public Sprite possibleEmptySprite3NSE;
    public Sprite possibleEmptySprite3NWE;
    public Sprite possibleEmptySprite3SWE;
    public Sprite possibleEmptySprite4;
    public Sprite possibleFilledSprite;
    public Sprite possibleFilledSprite1N;
    public Sprite possibleFilledSprite1S;
    public Sprite possibleFilledSprite1W;
    public Sprite possibleFilledSprite1E;
    public Sprite possibleFilledSprite2NS;
    public Sprite possibleFilledSprite2NW;
    public Sprite possibleFilledSprite2NE;
    public Sprite possibleFilledSprite2SW;
    public Sprite possibleFilledSprite2SE;
    public Sprite possibleFilledSprite2WE;
    public Sprite possibleFilledSprite3NSW;
    public Sprite possibleFilledSprite3NSE;
    public Sprite possibleFilledSprite3NWE;
    public Sprite possibleFilledSprite3SWE;
    public Sprite possibleFilledSprite4;

    public GameObject building;
    public int buildingCost;
    private bool validPlacement;
    private GameObject world;
    private World myWorld;
    private SpriteRenderer spriteRenderer;

    /**
     * Initializes the BuildingPlacement class
     */
    void Start()
    {
        //validPlacement = true;
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    /**
     * Allows the player to place a building in a viable location.
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

        //Need enough currency to build the reservoir
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

        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
        //distance to location just outside object? floor(width / 2 + 1) floor(height / 2 + 1)

        if (validPlacement && (mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) < 1 || mousePos.x + Mathf.FloorToInt(width / 2) >= myWorld.mapSize - 1
            || mousePos.y - Mathf.FloorToInt(height / 2) < 1 || mousePos.y + Mathf.FloorToInt(height / 2) >= myWorld.mapSize - 1))
        {
            validPlacement = false;
        }

        //can't place a building on other constructs or water
        int r = 0;
        while (validPlacement && r < height)
        {
            int c = 0;
            while (validPlacement && c < width)
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
                //farms need to be built on fertile land
                if (validPlacement && gameObject.GetComponent<Farm>() != null)
                {
                    if (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r] != null
                        && terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r].tag != "FertileLand")
                    {
                        validPlacement = false;
                    }
                }
                c++;
            }
            r++;
        }

        //make sure sprite is correct based on if the location is possible or not to build on
        updateAppearance(mousePos, terrainArr, validPlacement);

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
                for (r = 0; r < height; r++)
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
     * Updates the appearance of the reservoir based on its surroundings
     * @param mousePos the current position of the mouse
     * @param terrainArr the array holding terrain information from World
     * @param validPlacement whether the placement is possible
     */
    private void updateAppearance(Vector2 mousePos, GameObject[,] terrainArr, bool validPlacement)
    {
        //reservoirs appear filled when next to water
        bool nextToWater = checkForRawResources("Water", mousePos, terrainArr);

        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        Vector2 reservoirCenter = new Vector2(Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.y));
        bool northConnection = false;
        bool southConnection = false;
        bool westConnection = false;
        bool eastConnection = false;
        if ((int)reservoirCenter.y + 2 < myWorld.mapSize && structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y + 2] != null
            && ((structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y + 2].tag == "Road"
                && structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y + 2].GetComponent<RoadInformation>().getAqueduct() != null)
            || structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y + 2].GetComponent<Aqueduct>() != null
            || (structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y + 2].GetComponent<Reservoir>() != null
                && Mathf.RoundToInt(structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y + 2].transform.position.x) == Mathf.RoundToInt(transform.position.x))))
        {
            northConnection = true;
        }
        if ((int)reservoirCenter.y - 2 > 0 && structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y - 2] != null
            && ((structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y - 2].tag == "Road"
                && structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y - 2].GetComponent<RoadInformation>().getAqueduct() != null)
            || structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y - 2].GetComponent<Aqueduct>() != null
            || (structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y - 2].GetComponent<Reservoir>() != null
                && Mathf.RoundToInt(structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y - 2].transform.position.x) == Mathf.RoundToInt(transform.position.x))))
        {
            southConnection = true;
        }
        if ((int)reservoirCenter.x - 2 > 0 && structureArr[(int)reservoirCenter.x - 2, (int)reservoirCenter.y] != null
            && ((structureArr[(int)reservoirCenter.x - 2, (int)reservoirCenter.y].tag == "Road"
                && structureArr[(int)reservoirCenter.x - 2, (int)reservoirCenter.y].GetComponent<RoadInformation>().getAqueduct() != null)
            || structureArr[(int)reservoirCenter.x - 2, (int)reservoirCenter.y].GetComponent<Aqueduct>() != null
            || (structureArr[(int)reservoirCenter.x - 2, (int)reservoirCenter.y].GetComponent<Reservoir>() != null
                && Mathf.RoundToInt(structureArr[(int)reservoirCenter.x - 2, (int)reservoirCenter.y].transform.position.y) == Mathf.RoundToInt(transform.position.y))))
        {
            westConnection = true;
        }
        if ((int)reservoirCenter.x + 2 < myWorld.mapSize && structureArr[(int)reservoirCenter.x + 2, (int)reservoirCenter.y] != null
            && ((structureArr[(int)reservoirCenter.x + 2, (int)reservoirCenter.y].tag == "Road"
                && structureArr[(int)reservoirCenter.x + 2, (int)reservoirCenter.y].GetComponent<RoadInformation>().getAqueduct() != null)
            || structureArr[(int)reservoirCenter.x + 2, (int)reservoirCenter.y].GetComponent<Aqueduct>() != null
            || (structureArr[(int)reservoirCenter.x + 2, (int)reservoirCenter.y].GetComponent<Reservoir>() != null
                && Mathf.RoundToInt(structureArr[(int)reservoirCenter.x + 2, (int)reservoirCenter.y].transform.position.y) == Mathf.RoundToInt(transform.position.y))))
        {
            eastConnection = true;
        }
        //Update sprite based on connections
        if (!validPlacement)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = impossibleFilledSprite;
            }
            else
            {
                spriteRenderer.sprite = impossibleEmptySprite;
            }
        }
        else if (northConnection && southConnection && westConnection && eastConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite4;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite4;
            }
        }
        else if (northConnection && southConnection && westConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite3NSW;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite3NSW;
            }
        }
        else if (northConnection && southConnection && eastConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite3NSE;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite3NSE;
            }
        }
        else if (northConnection && westConnection && eastConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite3NWE;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite3NWE;
            }
        }
        else if (southConnection && westConnection && eastConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite3SWE;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite3SWE;
            }
        }
        else if (northConnection && southConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite2NS;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite2NS;
            }
        }
        else if (northConnection && westConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite2NW;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite2NW;
            }
        }
        else if (northConnection && eastConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite2NE;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite2NE;
            }
        }
        else if (southConnection && westConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite2SW;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite2SW;
            }
        }
        else if (southConnection && eastConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite2SE;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite2SE;
            }
        }
        else if (westConnection && eastConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite2WE;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite2WE;
            }
        }
        else if (northConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite1N;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite1N;
            }
        }
        else if (southConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite1S;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite1S;
            }
        }
        else if (westConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite1W;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite1W;
            }
        }
        else if (eastConnection)
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite1E;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite1E;
            }
        }
        else
        {
            if (nextToWater)
            {
                spriteRenderer.sprite = possibleFilledSprite;
            }
            else
            {
                spriteRenderer.sprite = possibleEmptySprite;
            }
        }
    }

    /**
     * Checks the areas around a building for if a raw resource is next to it
     * @param tagName the name of the resource to check for
     * @param mousePos the position of the mouse
     * @param terrainArr the array of terrain elements
     * @return nextToResources whether the building has any of the raw resource next to it
     */
    private bool checkForRawResources(string tagName, Vector2 mousePos, GameObject[,] terrainArr)
    {
        bool nextToResource = false;
        for (int i = 0; i < width; i++)
        {
            //Mathf.CeilToInt(width / 2.0f - 1) finds the middle square and then that is subtracted from x to get to the edge to start checking the structure array
            //Left side
            if ((int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1 > 0
                && (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + i < myWorld.mapSize - 1
                && (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + i > 0
                && terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + i] != null
                && (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + i].tag == tagName))
            {
                nextToResource = true;
            }
            //Right side
            else if ((int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + width < myWorld.mapSize - 1
                && (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + i < myWorld.mapSize - 1
                && (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + i > 0
                && terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + width, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + i] != null
                && (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + width, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + i].tag == tagName))
            {
                nextToResource = true;
            }
            //Top side
            else if ((int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i < myWorld.mapSize - 1
                && (int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i > 0
                && (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + height < myWorld.mapSize - 1
                && terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + height] != null
                && (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + height].tag == tagName))
            {
                nextToResource = true;
            }
            //Bottom side
            else if ((int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i < myWorld.mapSize - 1
                && (int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i > 0
                && (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) - 1 > 0
                && terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) - 1] != null
                && (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) - 1].tag == tagName))
            {
                nextToResource = true;
            }
        }
        return nextToResource;
    }
}
