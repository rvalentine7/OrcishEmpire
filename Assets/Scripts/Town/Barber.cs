using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Helps keep orcs healthy
 */
public class Barber : MonoBehaviour
{
    public int numCustomersPerWorker;

    private Employment employment;
    private int maxCustomers;
    private List<OrcInhabitant> customers;

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

        employment = gameObject.GetComponent<Employment>();
        maxCustomers = employment.getNumHealthyWorkers() * numCustomersPerWorker;
        customers = new List<OrcInhabitant>();
    }

    // Update is called once per frame
    void Update()
    {
        //if there are available customer positions, check houses to see if they have any orcs needing barbers
        maxCustomers = employment.getNumHealthyWorkers() * numCustomersPerWorker;
        int numCustomersToRemove = customers.Count - maxCustomers;
        while (numCustomersToRemove > 0)
        {
            OrcInhabitant customer = customers[0];
            customer.removeBarber();
            customers.RemoveAt(0);
            numCustomersToRemove--;
        }
    }

    /**
     * Gets the number of available customer spots
     * 
     * @return the number of available customer spots
     */
    public int getNumAvailableCustomerSpots()
    {
        return maxCustomers - customers.Count;
    }

    /**
     * Removes a customer from the barber
     * 
     * @param orcInhabitant the customer to remove
     */
    public void removeCustomer(OrcInhabitant orcInhabitant)
    {
        if (customers.Contains(orcInhabitant))
        {
            customers.Remove(orcInhabitant);
        }
    }

    /**
     * Adds a customer to the barber
     * 
     * @param orcInhabitant the customer to add
     */
    public void addCustomer(OrcInhabitant orcInhabitant)
    {
        if (customers.Count < maxCustomers && !customers.Contains(orcInhabitant))
        {
            customers.Add(orcInhabitant);
        }
    }

    /**
     * Gets the number of customers using this barber
     */
    public int getNumCustomers()
    {
        return customers.Count;
    }

    /**
     * Gets the maximum number of customers this barber can support
     */
    public int getNumMaxCustomers()
    {
        return maxCustomers;
    }
}
