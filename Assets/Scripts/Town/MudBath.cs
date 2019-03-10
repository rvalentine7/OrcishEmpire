using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Provides bathing services for the health of orcs
 * TODO: there should probably be a water-type employment class that this and fountain extend from (methods dealing with filling)
 */
public class MudBath : MonoBehaviour
{
    public Sprite dryBath;
    public Sprite wetBath;
    public int bathRadius;

    private GameObject world;
    private World myWorld;
    private GameObject[,] terrainArr;
    private bool filled;

    private void Awake()
    {
        world = GameObject.Find(World.WORLD_INFORMATION);
        myWorld = world.GetComponent<World>();
    }

    // Start is called before the first frame update
    void Start()
    {
        filled = false;
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        if (terrainArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].GetComponent<Tile>().hasPipes())
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = wetBath;
            gameObject.GetComponent<Employment>().addWaterSource();
        }
        else
        {
            filled = false;
            gameObject.GetComponent<SpriteRenderer>().sprite = dryBath;
        }
        handleServices(filled);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * Updates whether the fountain is filled
     * @param filled whether the fountain is filled
     */
    public void updateFilled(bool filled)
    {
        this.filled = filled;
        if (this.filled)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = wetBath;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = dryBath;
        }
        handleServices(filled);
    }

    /**
     * Lets households within the bath radius know whether this bath is providing services to them
     * @param providing whether the bath is providing its services to nearby houses
     */
    public void handleServices(bool providing)
    {
        GameObject[,] constructArr = myWorld.constructNetwork.getConstructArr();
        Vector2 mudBathPosition = gameObject.transform.position;
        //search for nearby houses and supply them with water
        for (int i = 0; i <= bathRadius * 2; i++)
        {
            for (int j = 0; j <= bathRadius * 2; j++)
            {
                if (mudBathPosition.x - bathRadius + i >= 0 && mudBathPosition.y - bathRadius + j >= 0
                        && mudBathPosition.x - bathRadius + i <= myWorld.mapSize && mudBathPosition.y - bathRadius + j <= myWorld.mapSize
                        && constructArr[(int)mudBathPosition.x - bathRadius + i, (int)mudBathPosition.y - bathRadius + j] != null
                        && constructArr[(int)mudBathPosition.x - bathRadius + i, (int)mudBathPosition.y - bathRadius + j].tag == World.HOUSE)
                {
                    HouseInformation houseInformation = constructArr[(int)mudBathPosition.x - bathRadius + i,
                        (int)mudBathPosition.y - bathRadius + j].GetComponent<HouseInformation>();
                    if (providing)
                    {
                        houseInformation.addMudBath(gameObject);
                    }
                    else
                    {
                        houseInformation.removeMudBath(gameObject);
                    }
                }
            }
        }
    }

    /**
     * Gets whether the mud bath is filled
     * @return filled whether the mud bath is filled
     */
    public bool getFilled()
    {
        return filled;
    }
}
