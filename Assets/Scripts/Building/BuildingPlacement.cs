﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Lets the player place buildings in the world.
/// </summary>
public class BuildingPlacement : BuildMode
{
    public Sprite possibleSprite;
    public Sprite possibleSprite2;
    public Sprite impossibleSprite;
    public Sprite impossibleSprite2;
    public GameObject building;
    private bool validPlacement;

    /// <summary>
    /// Initializes the BuildingPlacement class
    /// </summary>
    void Start () {
        
    }

    /// <summary>
    /// Allows the player to place a building in a viable location.
    /// </summary>
    void Update () {
        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Escape))
        {
            //exits out of construction mode if the right mouse button or escape is clicked
            Destroy(gameObject);
        }

        updateBuildMode();

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

        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();

        //distance to location just outside object? floor(width / 2 + 1) floor(height / 2 + 1)
        if (validPlacement && (mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) < 1 || mousePos.x + Mathf.FloorToInt(width / 2) >= myWorld.mapSize - 1
            || mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) < 1 || mousePos.y + Mathf.FloorToInt(height / 2) >= myWorld.mapSize - 1))
        {
            validPlacement = false;
        }

        //iron mines need to be next to "Rocks" terrain
        if (validPlacement && gameObject.name.Equals("BuildIronMine(Clone)"))
        {
            int i = 0;
            bool nearbyMountains = false;
            while (!nearbyMountains && i < 2)
            {
                //Left Side
                if ((int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1) > 0
                    && (int)(mousePos.y - Mathf.FloorToInt(height / 2 - 1) + i) > 0
                    && (int)(mousePos.y - Mathf.FloorToInt(height / 2 - 1) + i) < myWorld.mapSize - 1
                    && myWorld.mountainousTerrain.Contains(terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                    (int)(mousePos.y - Mathf.FloorToInt(height / 2 - 1) + i)].tag))
                {
                    nearbyMountains = true;
                }
                //Right Side
                else if ((int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + 1) < myWorld.mapSize - 1
                    && (int)(mousePos.y - Mathf.FloorToInt(height / 2 - 1) + i) > 0
                    && (int)(mousePos.y - Mathf.FloorToInt(height / 2 - 1) + i) < myWorld.mapSize - 1
                    && myWorld.mountainousTerrain.Contains(terrainArr[(int)(mousePos.x + Mathf.FloorToInt(width / 2 + 1)),
                    (int)(mousePos.y - Mathf.FloorToInt(height / 2 - 1) + i)].tag))
                {
                    nearbyMountains = true;
                }
                //Top Side
                else if ((int)(mousePos.y - Mathf.FloorToInt(height / 2) + 1) < myWorld.mapSize - 1
                    && (int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i) > 0
                    && (int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i) < myWorld.mapSize - 1
                    && myWorld.mountainousTerrain.Contains(terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i),
                    (int)(mousePos.y + Mathf.FloorToInt(height / 2) + 1)].tag))
                {
                    nearbyMountains = true;
                }
                //Bottom Side
                if ((int)(mousePos.y - Mathf.FloorToInt(height / 2)) < myWorld.mapSize - 1
                    && (int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i) > 0
                    && (int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i) < myWorld.mapSize - 1
                    && myWorld.mountainousTerrain.Contains(terrainArr[(int)(mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i),
                    (int)(mousePos.y - Mathf.FloorToInt(height / 2))].tag))
                {
                    nearbyMountains = true;
                }
                i++;
            }
            if (!nearbyMountains)
            {
                validPlacement = false;
            }
        }
        //gem mines need to be built next to "GemVein" terrain
        if (validPlacement && gameObject.name.Equals("BuildGemMine(Clone)"))
        {
            validPlacement = checkForRawResources("GemVein", mousePos, terrainArr);
        }
        //ochre pits need to be built next to "OchreHills" terrain
        if (validPlacement && gameObject.name.Equals("BuildOchrePit(Clone)"))
        {
            validPlacement = checkForRawResources("OchreHills", mousePos, terrainArr);
        }
        //lumberyards need to be built next to "Trees" terrain
        if (validPlacement && gameObject.name.Equals("BuildLumberMill(Clone)"))
        {
            validPlacement = checkForRawResources(World.TREES, mousePos, terrainArr);
        }
        //buildings using boats must be built next to water
        if (validPlacement && (gameObject.name.Equals("BuildBoatyard(Clone)") || gameObject.name.Equals("BuildFishingWharf(Clone)")))
        {
            validPlacement = checkForRawResources(World.WATER, mousePos, terrainArr);
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
                        && !terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + c, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + r].tag.Equals("FertileLand"))
                    {
                        validPlacement = false;
                    }
                }
                c++;
            }
            r++;
        }

        //reservoirs appear filled when next to water
        bool useFirstSprites = true;
        if (gameObject.name.Equals("BuildReservoir(Clone)"))
        {
            useFirstSprites = checkForRawResources(World.WATER, mousePos, terrainArr);
        }
        if (gameObject.name.Equals("BuildFountain(Clone)")
            && !terrainArr[(int)mousePos.x, (int)mousePos.y].GetComponent<Tile>().hasPipes())
        {
            useFirstSprites = false;
        }

        //make sure sprite is correct based on if the location is possible or not to build on
        if (validPlacement)
        {
            if (useFirstSprites)
            {
                GetComponent<SpriteRenderer>().sprite = possibleSprite;
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = possibleSprite2;
            }
        }
        else
        {
            if (useFirstSprites)
            {
                GetComponent<SpriteRenderer>().sprite = impossibleSprite;
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = impossibleSprite2;
            }
        }

        //Place the object in the world upon left mouse release
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0) && validPlacement)
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
                AudioSource buildAudioSource = GameObject.Find(World.BUILD_AUDIO).GetComponent<AudioSource>();
                buildAudioSource.clip = buildAudioClip;
                buildAudioSource.volume = myWorld.getSettingsMenu().getClickVolume();
                buildAudioSource.Play();
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

    /// <summary>
    /// Checks the areas around a building for if a raw resource is next to it
    /// </summary>
    /// <param name="tagName">the name of the resource to check for</param>
    /// <param name="mousePos">the position of the mouse</param>
    /// <param name="terrainArr">the array of terrain elements</param>
    /// <returns>whether the building has any of the raw resource next to it</returns>
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
                && (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) - 1, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + i].tag.Equals(tagName)))
            {
                nextToResource = true;
            }
            //Right side
            else if ((int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + width < myWorld.mapSize - 1
                && (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + i < myWorld.mapSize - 1
                && (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + i > 0
                && terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + width, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + i] != null
                && (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + width, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + i].tag.Equals(tagName)))
            {
                nextToResource = true;
            }
            //Top side
            else if ((int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i < myWorld.mapSize - 1
                && (int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i > 0
                && (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + height < myWorld.mapSize - 1
                && terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + height] != null
                && (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) + height].tag.Equals(tagName)))
            {
                nextToResource = true;
            }
            //Bottom side
            else if ((int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i < myWorld.mapSize - 1
                && (int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i > 0
                && (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) - 1 > 0
                && terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) - 1] != null
                && (terrainArr[(int)mousePos.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)mousePos.y - Mathf.CeilToInt(height / 2.0f - 1) - 1].tag.Equals(tagName)))
            {
                nextToResource = true;
            }
        }
        return nextToResource;
    }
}
