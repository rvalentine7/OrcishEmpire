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
    public int timeToKeepWet;

    private GameObject world;
    private World myWorld;
    private GameObject[,] terrainArr;
    private Employment employment;
    private bool hasWaterAccess;
    private Condition currentCondition;
    private Condition previousCondition;
    private enum Condition {WET, DRY};
    private bool hadPriorWorkers;
    private float timeSinceLastWorkerWasPresent;
    private float timeOfLastRefill;
    private float timeLeftToRefill;
    private float timeUntilDry;

    private void Awake()
    {
        world = GameObject.Find(World.WORLD_INFORMATION);
        myWorld = world.GetComponent<World>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentCondition = Condition.DRY;
        previousCondition = Condition.DRY;
        hadPriorWorkers = false;
        timeSinceLastWorkerWasPresent = 0;
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        employment = gameObject.GetComponent<Employment>();
        if (terrainArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].GetComponent<Tile>().hasPipes())
        {
            hasWaterAccess = true;
            //gameObject.GetComponent<Employment>().addWaterSource();
        }
        else
        {
            hasWaterAccess = false;
        }
    }

    // Update is called once per frame
    void Update()//TODO: fix the time issue (time is ticking and counting towards progress before workers even start working)
    {
        //If there are no workers, the bath should dry up
        if (employment.getNumHealthyWorkers() == 0/* || !hasWaterAccess*/)
        {
            //If no workers or no workers able to refill (lack of water), time left to refill should increase to timeToKeepWet's value
            if (hadPriorWorkers)
            {
                timeSinceLastWorkerWasPresent = Time.time;
                hadPriorWorkers = false;
                handleServices(false);//doing this here because no worker to provide services even if the bath was still wet
            }
            timeLeftToRefill = Mathf.Infinity;
            //Everything should be drying up
            if (currentCondition != Condition.DRY)
            {
                timeUntilDry = timeToKeepWet - (Time.time - timeOfLastRefill);
                if (timeUntilDry <= 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = dryBath;
                    timeUntilDry = 0;
                    currentCondition = Condition.DRY;
                }
            }
        }
        //If it is not alraedy wet or the number of workers is not at its max, we need to keep track of refilling
        else if (/*hasWaterAccess && */(currentCondition == Condition.DRY || employment.getNumHealthyWorkers() != employment.getWorkerCap()))
        {
            hadPriorWorkers = true;//there are workers here now
            if (timeLeftToRefill == Mathf.Infinity)
            {
                timeLeftToRefill = timeToKeepWet + (employment.getWorkerCap() - employment.getNumHealthyWorkers()) * employment.getWorkerValue();
            }
            else
            {
                timeLeftToRefill = (timeToKeepWet + (employment.getWorkerCap() - employment.getNumHealthyWorkers()) * employment.getWorkerValue()) - (Time.time - timeOfLastRefill);
            }
            //Update progress on refilling
            //timeLeftToRefill = (timeToKeepWet + (employment.getWorkerCap() - employment.getNumHealthyWorkers()) * employment.getWorkerValue()) - (Time.time - timeOfLastRefill);
            if (timeLeftToRefill < 0)
            {
                timeLeftToRefill = 0;
            }
            if (timeLeftToRefill == 0 && currentCondition == Condition.DRY)
            {
                timeOfLastRefill = Time.time;
                currentCondition = Condition.WET;
                timeLeftToRefill = employment.getNumHealthyWorkers() * employment.getWorkerValue();
                timeUntilDry = timeToKeepWet;
                gameObject.GetComponent<SpriteRenderer>().sprite = wetBath;
                handleServices(true);
            }
            //If there are not a max number of workers or it just got back to a max number of workers, it should be drying up
            if (timeUntilDry != 0 && (employment.getNumHealthyWorkers() != employment.getWorkerCap() || timeLeftToRefill > timeUntilDry))
            {
                timeUntilDry = timeToKeepWet - (Time.time - timeOfLastRefill);
                if (timeUntilDry <= 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = dryBath;
                    timeUntilDry = 0;
                    currentCondition = Condition.DRY;
                    handleServices(false);
                }
            }
        }
    }

    /**
     * Updates whether the mud bath has water access
     * @param filled whether the mud bath has water access
     */
    public void updateFilled(bool hasWaterAccess)
    {
        this.hasWaterAccess = hasWaterAccess;
    }

    /**
     * Lets households within the bath radius know whether this bath is providing services to them
     * @param providing whether the bath is providing its services to nearby houses
     */
    public void handleServices(bool providing)
    {
        GameObject[,] constructArr = myWorld.constructNetwork.getConstructArr();
        Vector2 mudBathPosition = gameObject.transform.position;
        //search for nearby houses
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
     * Gets the time left to refill the mud bath to keep it wet
     * @return the time left to refill the mud bath
     */
    public int getTimeLeftToRefill()
    {
        return Mathf.RoundToInt(timeLeftToRefill);
    }

    /**
     * Gets the time until this mud bath will dry up
     * @return int the time until the mad bath dries up
     */
    public int getTimeUntilDry()
    {
        return Mathf.RoundToInt(timeUntilDry);
    }

    /**
     * Gets whether the mud bath is currently wet
     * @return whether the mud bath is currently wet
     */
    public bool getWet()
    {
        return currentCondition == Condition.WET;
    }

    /**
     * Gets whether the mud bath is filled
     * @return filled whether the mud bath is filled
     */
    //public bool getFilled()
    //{
    //    return hasWaterAccess;//TODO: this needs to be changed? is this used for anything at all?
    //}
}
