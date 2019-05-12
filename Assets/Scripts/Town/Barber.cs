using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barber : MonoBehaviour
{
    public int numCustomersPerWorker;

    // Start is called before the first frame update
    void Start()
    {
        GameObject world = GameObject.Find(World.WORLD_INFORMATION);
        World myWorld = world.GetComponent<World>();
        GameObject[,] constructArr = myWorld.constructNetwork.getConstructArr();
        //If there are nearby houses, let them know there is a barber nearby
        Vector2 mudBathPosition = gameObject.transform.position;
        int healthBuildingRadius = World.HEALTH_BUILDING_RADIUS;
        //search for nearby houses
        for (int i = -healthBuildingRadius; i <= healthBuildingRadius; i++)
        {
            for (int j = -healthBuildingRadius; j <= healthBuildingRadius; j++)
            {
                if (mudBathPosition.x + i >= 0 && mudBathPosition.y + j >= 0
                        && mudBathPosition.x + i <= myWorld.mapSize - 1 && mudBathPosition.y + j <= myWorld.mapSize - 1
                        && constructArr[(int)mudBathPosition.x + i, (int)mudBathPosition.y + j] != null
                        && constructArr[(int)mudBathPosition.x + i, (int)mudBathPosition.y + j].tag == World.HOUSE)
                {
                    HouseInformation houseInformation = constructArr[(int)mudBathPosition.x + i,
                        (int)mudBathPosition.y + j].GetComponent<HouseInformation>();
                    houseInformation.addBarber(gameObject);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if there are available customer positions, check houses to see if they have any orcs needing barbers
    }

    public double getNumAvailableCustomerSpots()
    {
        return 0;
    }

    public void removeCustomer(OrcInhabitant orcInhabitant)
    {

    }

    public void addCustomer(OrcInhabitant orcInhabitant)
    {

    }
}
