using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GladiatorPit : MonoBehaviour {
    private int trainingProgress;
    private int progressRequired;
    private int numReadyGladiators;//the number of gladiators that have been trained and are ready to be sent out to perform
    private int gladiatorsGoingToFight;
    private float checkTime;
    private bool active;//will be used to activate/deactivate the building
                        //private Dictionary<GameObject, GameObject> arenaAndGladiators; //was this for if the arena is destroyed?

    public GameObject gladiatorOrc;
    public int maxGladiators;//the number of gladiators that can be ready and waiting at any given time
    public int timeInterval;

	// Use this for initialization
	void Start () {
        trainingProgress = 0;
        progressRequired = 100;
        numReadyGladiators = 0;
        gladiatorsGoingToFight = 0;
        checkTime = 0.0f;
        active = true;
        //arenaAndGladiators = new Dictionary<GameObject, GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        //number of workers impact how quickly gladiators can be trained
        //when gladiators are trained, send them to arenas
        if (numReadyGladiators < maxGladiators && active)
        {
            Employment employment = gameObject.GetComponent<Employment>();
            if (employment.getNumHealthyWorkers() > 0 && Time.time > checkTime)
            {
                checkTime = Time.time + timeInterval;
                if (trainingProgress < progressRequired)
                {
                    if (trainingProgress + employment.getNumHealthyWorkers() * employment.getWorkerValue() > progressRequired)
                    {
                        trainingProgress = progressRequired;
                    }
                    else
                    {
                        trainingProgress += employment.getNumHealthyWorkers() * employment.getWorkerValue();
                    }
                }
            }
            if (trainingProgress == progressRequired)
            {
                trainingProgress = 0;
                numReadyGladiators += 1;
            }
        }
	}

    /**
     * Used by other gameObjects to request gladiators to come perform at their buildings
     * @param numRequestedGladiators the number of gladiators the other building is requesting
     * @return the number of gladiators being sent to the other building
     */
    public int requestGladiators(int numRequestedGladiators, GameObject destination)
    {
        if (numReadyGladiators - gladiatorsGoingToFight > 0)
        {
            if (numRequestedGladiators > numReadyGladiators - gladiatorsGoingToFight)
            {
                //TODO?: Make sure to update arenaAndGladiators... was this from before I put more functionality in Gladiator itself?
                createGladiator(numReadyGladiators - gladiatorsGoingToFight, destination);
                gladiatorsGoingToFight += numReadyGladiators - gladiatorsGoingToFight;
                return numReadyGladiators;
            }
            else
            {
                //TODO?: Make sure to update arenaAndGladiators... was this from before I put more functionality in Gladiator itself?
                createGladiator(numRequestedGladiators, destination);
                gladiatorsGoingToFight += numRequestedGladiators;
                return numRequestedGladiators;
            }
        }
        return 0;
    }

    /**
     * When a gladiator reaches its destination, it informs the gladiator pit
     * in order to reduce the number of waiting gladiators
     * @param num the number of ready gladiators to reduce at this building
     */
    public void reduceNumReadyGladiators(int num)
    {
        numReadyGladiators -= num;
        gladiatorsGoingToFight -= num;
    }

    /**
     * When a gladiator is forced to return (arena is destroyed/runs out of employees
     * before the gladiator arrives at an arena), it lets the GladiatorPit know it
     * has come back
     * @param numberReturning the number of gladiators returning
     */
    public void gladiatorReturn(int numberReturning)
    {
        if (numberReturning <= gladiatorsGoingToFight)
        {
            gladiatorsGoingToFight -= numberReturning;
        }
        else
        {
            Debug.Log("There are more gladiators returning than there were supposed to be going to arenas");
        }
    }

    /**
     * Creates a gladiator to go fight at an arena
     * @param num the number of gladiators being counted by the one game object
     */
    public void createGladiator(int num, GameObject destination)
    {
        GameObject world = GameObject.Find(World.WORLD_INFORMATION);
        World myWorld = world.GetComponent<World>();
        GameObject[,] structArr = myWorld.constructNetwork.getConstructArr();
        int width = (int)gameObject.GetComponent<BoxCollider2D>().size.x;
        int height = (int)gameObject.GetComponent<BoxCollider2D>().size.y;
        //checking areas around the farm to place an orc on a road
        Vector2 employmentPos = gameObject.transform.position;
        bool foundSpawn = false;
        Vector2 spawnPosition = new Vector2();
        int i = 0;
        while (!foundSpawn && i < width)
        {
            //checking the row below the gameObject
            if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1)].tag == World.ROAD)
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1));
                foundSpawn = true;
            }
            //checking the row above the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)].tag == World.ROAD)
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1));
                foundSpawn = true;
            }
            i++;
        }
        int j = 0;
        while (!foundSpawn && j < height)
        {
            //checking the column to the left of the gameObject
            if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag == World.ROAD)
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j));
                foundSpawn = true;
            }
            //checking the column to the right of the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag == World.ROAD)
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j));
                foundSpawn = true;
            }
            j++;
        }

        //distribution orc will need to set distriubtion status to false when it returns
        GameObject newGladiator = Instantiate(gladiatorOrc, new Vector2(spawnPosition.x, spawnPosition.y + 0.4f), Quaternion.identity);
        Gladiator gladiator = newGladiator.GetComponent<Gladiator>();
        gladiator.setArena(destination, num);
        gladiator.setOriginalLocation(spawnPosition);
        gladiator.setOrcEmployment(gameObject);
    }

    /**
     * Gets the number of gladiators ready to fight in arenas
     */
    public int getNumReadyGladiators()
    {
        return numReadyGladiators;
    }

    /**
     * Gets the current progress in training a gladiator
     */
    public int getTrainingProgress()
    {
        return trainingProgress;
    }

    /**
     * Toggles whether the building is active
     */
    public void toggleActive()
    {
        active = !active;
    }
}
