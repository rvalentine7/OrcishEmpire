using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Places houses in the world
/// </summary>
public class HousingPlacement : BuildMode
{
    public Sprite possibleSprite;
    public Sprite impossibleSprite;
    public GameObject validTempHouse;
    public GameObject invalidTempHouse;
    public GameObject house;
    public int buildingCost;
    private SpriteRenderer spriteRenderer;
    private bool startingPositionChosen;
    private Vector2 startingPosition;
    private Dictionary<Vector2, GameObject> tempHouses;//The key is the position
    private int currentCost;

    /// <summary>
    /// Initializes the HousingPlacement class
    /// </summary>
    void Start()
    {
        //validPlacement = true;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        startingPositionChosen = false;
        startingPosition = new Vector2(-1, -1);
        tempHouses = new Dictionary<Vector2, GameObject>();
        currentCost = 0;
    }

    /// <summary>
    /// Allows the player to place houses in viable locations.
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (tempHouses.Count > 0)
            {
                //Destroy the temp houses because we will have to check them all over again
                foreach (GameObject houseToCreate in tempHouses.Values)
                {
                    Destroy(houseToCreate);
                }
                tempHouses = new Dictionary<Vector2, GameObject>();
                startingPositionChosen = false;
                startingPosition = new Vector2(-1, -1);
            }
            else
            {
                //exits out of construction mode if the right mouse button or escape is clicked
                Destroy(gameObject);
            }
        }

        updateBuildMode();

        if (!startingPositionChosen && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
        {
            startingPosition = mousePos;
            startingPositionChosen = true;
        }
        else if (!startingPositionChosen)
        {
            //Single house
            bool validPlacement = checkPositionValidity((int)mousePos.x, (int)mousePos.y);

            //make sure sprite is correct based on if the location is possible or not to build on
            if (validPlacement)
            {
                spriteRenderer.sprite = possibleSprite;
            }
            else
            {
                spriteRenderer.sprite = impossibleSprite;
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
        else if (startingPositionChosen)
        {
            int xCount = Mathf.Abs((int)mousePos.x - (int)startingPosition.x) + 1;
            int yCount = Mathf.Abs((int)mousePos.y - (int)startingPosition.y) + 1;

            //Modifiers for determining which way the houses are being placed
            int xModifier = 1;
            int yModifier = 1;
            //Mouse is to the left of the starting position
            if (mousePos.x - startingPosition.x < 0)
            {
                xModifier *= -1;
            }
            //Mouse is under the starting position
            if (mousePos.y - startingPosition.y < 0)
            {
                yModifier *= -1;
            }

            //tempHousesToDestroy is used to clear out housing locations we're no longer looking to build in
            Dictionary<Vector2, GameObject> tempHousesToDestroy = new Dictionary<Vector2, GameObject>(tempHouses);
            currentCost = 0;
            for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    int modifiedX = (int)startingPosition.x + i * xModifier;
                    int modifiedY = (int)startingPosition.y + j * yModifier;

                    bool validPlacement = checkPositionValidity(modifiedX, modifiedY);

                    Vector2 tempHousePosition = new Vector2(modifiedX, modifiedY);
                    if (!tempHouses.ContainsKey(tempHousePosition))
                    {
                        //A new temp house object to place

                        if (validPlacement)
                        {
                            //Valid House
                            currentCost += buildingCost;
                            tempHouses.Add(tempHousePosition, Instantiate(validTempHouse, tempHousePosition, Quaternion.identity) as GameObject);
                        }
                        else
                        {
                            //Invalid House
                            tempHouses.Add(tempHousePosition, Instantiate(invalidTempHouse, tempHousePosition, Quaternion.identity) as GameObject);
                        }
                        //Keep the starting position from having 2 different sprites
                        if (i == 0 && j == 0)
                        {
                            tempHouses[startingPosition].GetComponent<SpriteRenderer>().enabled = false;
                        }
                    }
                    else
                    {
                        //An old temp house object that should still exist

                        //If the validity of the house has changed, update its sprite
                        if (validPlacement)
                        {
                            Vector2 currentPosition = new Vector2(modifiedX, modifiedY);
                            if (tempHouses[currentPosition].GetComponent<SpriteRenderer>().sprite.name.Contains("Impossible"))
                            {
                                tempHouses[currentPosition].GetComponent<SpriteRenderer>().sprite = possibleSprite;
                            }
                        }
                        else
                        {
                            Vector2 currentPosition = new Vector2(modifiedX, modifiedY);
                            if (!tempHouses[currentPosition].GetComponent<SpriteRenderer>().sprite.name.Contains("Impossible"))
                            {
                                tempHouses[currentPosition].GetComponent<SpriteRenderer>().sprite = impossibleSprite;
                            }
                        }

                        //This is a valid house, so we should not clear it out
                        tempHousesToDestroy.Remove(tempHousePosition);
                    }
                }
            }
            //The temp house is no longer in the area we're trying to build. Remove it from tempHouses and destroy it
            foreach (GameObject tempHouseToDestroy in tempHousesToDestroy.Values)
            {
                tempHouses.Remove(tempHouseToDestroy.transform.position);
                Destroy(tempHouseToDestroy);
            }
        }

        //Create houses if possible
        if (Input.GetMouseButtonUp(0))
        {
            startingPositionChosen = false;
            startingPosition = new Vector2(-1, -1);
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                foreach (GameObject houseToCreate in tempHouses.Values)
                {
                    if (!houseToCreate.GetComponent<SpriteRenderer>().sprite.name.Contains("Impossible"))
                    {
                        Vector2 housePosition = houseToCreate.transform.position;
                        AudioSource buildAudioSource = GameObject.Find(World.BUILD_AUDIO).GetComponent<AudioSource>();
                        buildAudioSource.clip = buildAudioClip;
                        buildAudioSource.volume = myWorld.getSettingsMenu().getClickVolume();
                        buildAudioSource.Play();
                        GameObject buildingObj = Instantiate(house, housePosition, Quaternion.identity) as GameObject;
                        myWorld.updateCurrency(-buildingCost);
                        myWorld.constructNetwork.setConstructArr((int)housePosition.x, (int)housePosition.y, buildingObj);
                    }
                }
            }

            //Destroy the temp houses because we will have to check them all over again
            foreach (GameObject houseToCreate in tempHouses.Values)
            {
                Destroy(houseToCreate);
            }

            tempHouses = new Dictionary<Vector2, GameObject>();
        }
    }

    /// <summary>
    /// Checks the validity of the house in the given position
    /// </summary>
    /// <param name="xPosition">The x coordinate</param>
    /// <param name="yPosition">The y coordinate</param>
    /// <returns>Whether the house is in a valid position</returns>
    private bool checkPositionValidity(int xPosition, int yPosition)
    {
        bool validPlacement = true;

        //Need enough currency to build the building
        if (myWorld.getCurrency() < buildingCost + currentCost)
        {
            validPlacement = false;
        }
        else
        {
            validPlacement = true;
        }
        //Buildings cannot be placed outside of the map
        if (xPosition < 0)
        {
            xPosition = 0;
            validPlacement = false;
        }
        if (xPosition > myWorld.mapSize - 1)
        {
            xPosition = myWorld.mapSize - 1;
            validPlacement = false;
        }
        if (yPosition < 0)
        {
            yPosition = 0;
            validPlacement = false;
        }
        if (yPosition > myWorld.mapSize - 1)
        {
            yPosition = myWorld.mapSize - 1;
            validPlacement = false;
        }

        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
        //distance to location just outside object? floor(width / 2 + 1) floor(height / 2 + 1)

        if (validPlacement && (xPosition - Mathf.CeilToInt(width / 2.0f - 1) < 1 || xPosition + Mathf.FloorToInt(width / 2) >= myWorld.mapSize - 1
            || yPosition - Mathf.FloorToInt(height / 2) < 1 || yPosition + Mathf.FloorToInt(height / 2) >= myWorld.mapSize - 1))
        {
            validPlacement = false;
        }

        //houses can only be built within a distance of 2 from a road in horizontal and vertical directions
        if (validPlacement && (xPosition >= myWorld.mapSize - 1 || (xPosition + 1 > 0 && xPosition + 1 < myWorld.mapSize && yPosition > 0 && yPosition < myWorld.mapSize
            && (structureArr[xPosition + 1, yPosition] == null || !structureArr[xPosition + 1, yPosition].tag.Equals(World.ROAD))))
            && (xPosition >= myWorld.mapSize - 2 || (xPosition + 2 > 0 && xPosition + 2 < myWorld.mapSize && yPosition > 0 && yPosition < myWorld.mapSize
            && (structureArr[xPosition + 2, yPosition] == null || !structureArr[xPosition + 2, yPosition].tag.Equals(World.ROAD))))
            && (xPosition <= 1 || (xPosition - 1 > 0 && xPosition - 1 < myWorld.mapSize && yPosition > 0 && yPosition < myWorld.mapSize
            && (structureArr[xPosition - 1, yPosition] == null || !structureArr[xPosition - 1, yPosition].tag.Equals(World.ROAD))))
            && (xPosition <= 2 || (xPosition - 2 > 0 && xPosition - 2 < myWorld.mapSize && yPosition > 0 && yPosition < myWorld.mapSize
            && (structureArr[xPosition - 2, yPosition] == null || !structureArr[xPosition - 2, yPosition].tag.Equals(World.ROAD))))
            && (yPosition >= myWorld.mapSize - 1 || (xPosition > 0 && xPosition < myWorld.mapSize && yPosition + 1 > 0 && yPosition + 1 < myWorld.mapSize
            && (structureArr[xPosition, yPosition + 1] == null || !structureArr[xPosition, yPosition + 1].tag.Equals(World.ROAD))))
            && (yPosition >= myWorld.mapSize - 2 || (xPosition > 0 && xPosition < myWorld.mapSize && yPosition + 2 > 0 && yPosition + 2 < myWorld.mapSize
            && (structureArr[xPosition, yPosition + 2] == null || !structureArr[xPosition, yPosition + 2].tag.Equals(World.ROAD))))
            && (yPosition <= 1 || (xPosition > 0 && xPosition < myWorld.mapSize && yPosition - 1 > 0 && yPosition - 1 < myWorld.mapSize
            && (structureArr[xPosition, yPosition - 1] == null || !structureArr[xPosition, yPosition - 1].tag.Equals(World.ROAD))))
            && (yPosition <= 2 || (xPosition > 0 && xPosition < myWorld.mapSize && yPosition - 2 > 0 && yPosition - 2 < myWorld.mapSize
            && (structureArr[xPosition, yPosition - 2] == null || !structureArr[xPosition, yPosition - 2].tag.Equals(World.ROAD)))))
        {
            validPlacement = false;
        }

        //can't place a house on other constructs or water
        if (structureArr[xPosition, yPosition] != null
            || !myWorld.buildableTerrain.Contains(terrainArr[xPosition, yPosition].tag))
        {
            validPlacement = false;
        }
        return validPlacement;
    }
}
