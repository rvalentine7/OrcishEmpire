using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A gladiator pit trains gladiators to fight in an arena
/// </summary>
public class GladiatorPit : Building {
    private World myWorld;
    private Employment employment;
    private float trainingProgress;
    private float progressRequired;
    private float prevUpdateTime;
    private int numReadyGladiators;//the number of gladiators that have been trained and are ready to be sent out to perform
    private int gladiatorsGoingToFight;

    public GameObject gladiatorOrc;
    public int maxGladiators;//the number of gladiators that can be ready and waiting at any given time
    public int timeToTrain;

	/// <summary>
    /// Initialization
    /// </summary>
	void Start () {
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        employment = gameObject.GetComponent<Employment>();
        trainingProgress = 0.0f;
        progressRequired = 100.0f;
        prevUpdateTime = 0.0f;
        numReadyGladiators = 0;
        gladiatorsGoingToFight = 0;
	}
	
	/// <summary>
    /// Logic for the gladiator pit
    /// </summary>
	void Update () {
        //number of workers impact how quickly gladiators can be trained
        //when gladiators are trained, send them to arenas
        if (numReadyGladiators < maxGladiators)
        {
            int numHealthyWorkers = employment.getNumHealthyWorkers();
            if (numHealthyWorkers > 0)
            {
                if (trainingProgress < progressRequired)
                {
                    float progressedTime = Time.unscaledTime - prevUpdateTime;
                    float effectiveTimeToFinish = timeToTrain / (employment.getNumWorkers() / numHealthyWorkers);
                    trainingProgress += progressedTime / effectiveTimeToFinish * 100;
                    if (trainingProgress >= progressRequired)
                    {
                        trainingProgress = progressRequired;
                    }
                }
            }
            if (trainingProgress >= progressRequired)
            {
                trainingProgress = 0;
                numReadyGladiators += 1;
            }
        }

        prevUpdateTime = Time.unscaledTime;
    }

    /// <summary>
    /// Used by other gameObjects to request gladiators to come perform at their buildings
    /// </summary>
    /// <param name="numRequestedGladiators">the number of gladiators the other building is requesting</param>
    /// <param name="destination">The arena requesting the gladiators</param>
    /// <returns>the number of gladiators being sent to the other building</returns>
    public int requestGladiators(int numRequestedGladiators, GameObject destination)
    {
        if (numReadyGladiators - gladiatorsGoingToFight > 0)
        {
            if (numRequestedGladiators > numReadyGladiators - gladiatorsGoingToFight)
            {
                createGladiator(numReadyGladiators - gladiatorsGoingToFight, destination);
                gladiatorsGoingToFight += numReadyGladiators - gladiatorsGoingToFight;
                return numReadyGladiators;
            }
            else
            {
                createGladiator(numRequestedGladiators, destination);
                gladiatorsGoingToFight += numRequestedGladiators;
                return numRequestedGladiators;
            }
        }
        return 0;
    }

    /// <summary>
    /// When a gladiator reaches its destination, it informs the gladiator pit
    /// in order to reduce the number of waiting gladiators
    /// </summary>
    /// <param name="num">the number of ready gladiators to reduce at this building</param>
    public void reduceNumReadyGladiators(int num)
    {
        numReadyGladiators -= num;
        gladiatorsGoingToFight -= num;
    }

    /// <summary>
    /// When a gladiator is forced to return (arena is destroyed/runs out of employees
    /// before the gladiator arrives at an arena), it lets the GladiatorPit know it
    /// has come back
    /// </summary>
    /// <param name="numberReturning">the number of gladiators returning</param>
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

    /// <summary>
    /// Creates a gladiator to go fight at an arena
    /// </summary>
    /// <param name="num">the number of gladiators being counted by the one game object</param>
    /// <param name="destination">The arena the gladiator is going to</param>
    public void createGladiator(int num, GameObject destination)
    {
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
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1)].tag.Equals(World.ROAD))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) - 1));
                foundSpawn = true;
            }
            //checking the row above the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) + i),
                (Mathf.CeilToInt(employmentPos.y) + Mathf.CeilToInt(height / 2.0f - 1) + 1)].tag.Equals(World.ROAD))
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
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag.Equals(World.ROAD))
            {
                spawnPosition = new Vector2((Mathf.FloorToInt(employmentPos.x) - Mathf.CeilToInt(width / 2.0f - 1) - 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j));
                foundSpawn = true;
            }
            //checking the column to the right of the gameObject
            else if (!foundSpawn && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)] != null
                && structArr[(Mathf.FloorToInt(employmentPos.x) + Mathf.CeilToInt(width / 2.0f - 0.5f) + 1),
                (Mathf.FloorToInt(employmentPos.y) - Mathf.CeilToInt(height / 2.0f - 1) + j)].tag.Equals(World.ROAD))
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

    /// <summary>
    /// Gets the number of gladiators ready to fight in arenas
    /// </summary>
    /// <returns>The number of gladiators ready to fight in arenas</returns>
    public int getNumReadyGladiators()
    {
        return numReadyGladiators;
    }

    /// <summary>
    /// Gets the current progress in training a gladiator
    /// </summary>
    /// <returns>The current progress in training a gladiator</returns>
    public int getTrainingProgress()
    {
        return Mathf.FloorToInt(trainingProgress);
    }
}
