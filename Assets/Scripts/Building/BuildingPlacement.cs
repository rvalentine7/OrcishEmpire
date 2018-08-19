﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/**
 * Lets the player place buildings in the world.
 */
public class BuildingPlacement : MonoBehaviour {
    public int width;
    public int height;
    public Sprite possibleSprite;
    public Sprite impossibleSprite;
    public GameObject building;
    private bool validPlacement;
    private GameObject world;
    private World myWorld;

    /**
     * Initializes the BuildingPlacement class
     */
    void Start () {
        //validPlacement = true;
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
    }
	
	/**
     * Allows the player to place a building in a viable location.
     */
	void Update () {
        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Escape))
        {
            //exits out of construction mode if the right mouse button or escape is clicked
            Destroy(gameObject);
        }
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = Mathf.RoundToInt(mousePos.x);
        mousePos.y = Mathf.RoundToInt(mousePos.y);
        mousePos.z = 0;

        validPlacement = true;
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
        //houses can only be built within a distance of 2 from a road in horizontal and vertical directions
        if (validPlacement && gameObject.name == "BuildHouse(Clone)")
        {
            if ((mousePos.x >= myWorld.mapSize - 1 || (mousePos.x + 1 > 0 && mousePos.x + 1 < myWorld.mapSize && mousePos.y > 0 && mousePos.y < myWorld.mapSize
                && (structureArr[(int)mousePos.x + 1, (int)mousePos.y] == null || structureArr[(int)mousePos.x + 1, (int)mousePos.y].tag != "Road")))
                && (mousePos.x >= myWorld.mapSize - 2 || (mousePos.x + 2 > 0 && mousePos.x + 2 < myWorld.mapSize && mousePos.y > 0 && mousePos.y < myWorld.mapSize
                && (structureArr[(int)mousePos.x + 2, (int)mousePos.y] == null || structureArr[(int)mousePos.x + 2, (int)mousePos.y].tag != "Road")))
                && (mousePos.x <= 1 || (mousePos.x - 1 > 0 && mousePos.x - 1 < myWorld.mapSize && mousePos.y > 0 && mousePos.y < myWorld.mapSize
                && (structureArr[(int)mousePos.x - 1, (int)mousePos.y] == null || structureArr[(int)mousePos.x - 1, (int)mousePos.y].tag != "Road")))
                && (mousePos.x <= 2 || (mousePos.x - 2 > 0 && mousePos.x - 2 < myWorld.mapSize && mousePos.y > 0 && mousePos.y < myWorld.mapSize
                && (structureArr[(int)mousePos.x - 2, (int)mousePos.y] == null || structureArr[(int)mousePos.x - 2, (int)mousePos.y].tag != "Road")))
                && (mousePos.y >= myWorld.mapSize - 1 || (mousePos.x > 0 && mousePos.x < myWorld.mapSize && mousePos.y + 1 > 0 && mousePos.y + 1 < myWorld.mapSize
                && (structureArr[(int)mousePos.x, (int)mousePos.y + 1] == null || structureArr[(int)mousePos.x, (int)mousePos.y + 1].tag != "Road")))
                && (mousePos.y >= myWorld.mapSize - 2 || (mousePos.x > 0 && mousePos.x < myWorld.mapSize && mousePos.y + 2 > 0 && mousePos.y + 2 < myWorld.mapSize
                && (structureArr[(int)mousePos.x, (int)mousePos.y + 2] == null || structureArr[(int)mousePos.x, (int)mousePos.y + 2].tag != "Road")))
                && (mousePos.y <= 1 || (mousePos.x > 0 && mousePos.x < myWorld.mapSize && mousePos.y - 1 > 0 && mousePos.y - 1 < myWorld.mapSize
                && (structureArr[(int)mousePos.x, (int)mousePos.y - 1] == null || structureArr[(int)mousePos.x, (int)mousePos.y - 1].tag != "Road")))
                && (mousePos.y <= 2 || (mousePos.x > 0 && mousePos.x < myWorld.mapSize && mousePos.y - 2 > 0 && mousePos.y - 2 < myWorld.mapSize
                && (structureArr[(int)mousePos.x, (int)mousePos.y - 2] == null || structureArr[(int)mousePos.x, (int)mousePos.y - 2].tag != "Road"))))
            {
                validPlacement = false;
            }
        }
        //iron mines need to be next to "Rocks" terrain
        if (validPlacement && gameObject.name == "BuildIronMine(Clone)")
        {
            if (!myWorld.mountainousTerrain.Contains(terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1)),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2))].tag)//below bottom left corner
                && !myWorld.mountainousTerrain.Contains(terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1)),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2) + 1)].tag)//above top left corner
                && !myWorld.mountainousTerrain.Contains(terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2)),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2))].tag)//below bottom right corner
                && !myWorld.mountainousTerrain.Contains(terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2)),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2) + 1)].tag)//above top right corner
                && !myWorld.mountainousTerrain.Contains(terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2) + 1),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2) + 1)].tag)//to the right of bottom right corner
                && !myWorld.mountainousTerrain.Contains(terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2) + 1),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2))].tag)//to the right of the top right corner
                && !myWorld.mountainousTerrain.Contains(terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2) + 1)].tag)//to the left of the bottom left corner
                && !myWorld.mountainousTerrain.Contains(terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2))].tag))//to the left of the top left corner
            {
                validPlacement = false;
            }
        }
        //gem mines need to be built next to "GemVein" terrain
        if (validPlacement && gameObject.name == "BuildGemMine(Clone)")
        {
            if (terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1)),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2))].tag != "GemVein"//below bottom left corner
                && terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1)),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2) + 1)].tag != "GemVein"//above top left corner
                && terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2)),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2))].tag != "GemVein"//below bottom right corner
                && terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2)),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2) + 1)].tag != "GemVein"//above top right corner
                && terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2) + 1),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2) + 1)].tag != "GemVein"//to the right of bottom right corner
                && terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2) + 1),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2))].tag != "GemVein"//to the right of the top right corner
                && terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2) + 1)].tag != "GemVein"//to the left of the bottom left corner
                && terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2))].tag != "GemVein")//to the left of the top left corner
            {
                validPlacement = false;
            }
        }
        //ochre pits need to be built next to "OchreHills" terrain
        if (validPlacement && gameObject.name == "BuildOchrePit(Clone)")
        {
            if (terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1)),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2))].tag != "OchreHills"//below bottom left corner
                && terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1)),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2) + 1)].tag != "OchreHills"//above top left corner
                && terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2)),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2))].tag != "OchreHills"//below bottom right corner
                && terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2)),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2) + 1)].tag != "OchreHills"//above top right corner
                && terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2) + 1),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2) + 1)].tag != "OchreHills"//to the right of bottom right corner
                && terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2) + 1),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2))].tag != "OchreHills"//to the right of the top right corner
                && terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2) + 1)].tag != "OchreHills"//to the left of the bottom left corner
                && terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2))].tag != "OchreHills")//to the left of the top left corner
            {
                validPlacement = false;
            }
        }
        //lumberyards need to be built next to "Trees" terrain
        if (validPlacement && gameObject.name == "BuildLumberMill(Clone)")
        {
            if (terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1)),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2))].tag != "Trees"//below bottom left corner
                && terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1)),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2) + 1)].tag != "Trees"//above top left corner
                && terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2)),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2))].tag != "Trees"//below bottom right corner
                && terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2)),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2) + 1)].tag != "Trees"//above top right corner
                && terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2) + 1),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2) + 1)].tag != "Trees"//to the right of bottom right corner
                && terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2) + 1),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2))].tag != "Trees"//to the right of the top right corner
                && terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (int)(mousePos.y - Mathf.FloorToInt(height / 2) + 1)].tag != "Trees"//to the left of the bottom left corner
                && terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (int)(mousePos.y + Mathf.FloorToInt(height / 2))].tag != "Trees")//to the left of the top left corner
            {
                validPlacement = false;
            }
        }
        //docks need to be built next to water (Tag: "Water")

        //can't place a building on other constructs or water
        int r = 0;
        while (validPlacement && r < height)
        {
            int c = 0;
            while (validPlacement && c < width)
            {
                //Mathf.CeilToInt(width / 2.0f - 1) finds the middle square and then that is subtracted from x to get to the edge to start checking the structure array
                //TODO: change the following to be areas that can be built on because the areas that can't be built on are more numerous
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
        if (validPlacement)
        {
            GetComponent<SpriteRenderer>().sprite = possibleSprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = impossibleSprite;
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
}
