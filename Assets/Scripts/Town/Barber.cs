using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helps keep orcs healthy
/// </summary>
public class Barber : Building
{
    public int numCustomersPerWorker;

    private Employment employment;
    private int maxCustomers;
    private List<OrcInhabitant> customers;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        World myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
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
                        && constructArr[(int)mudBathPosition.x + i, (int)mudBathPosition.y + j].tag.Equals(World.HOUSE))
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

    /// <summary>
    /// Updates the barber
    /// </summary>
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

    /// <summary>
    /// Gets the number of available customer spots
    /// </summary>
    /// <returns>the number of available customer spots</returns>
    public int getNumAvailableCustomerSpots()
    {
        return maxCustomers - customers.Count;
    }

    /// <summary>
    /// Removes a customer from the barber
    /// </summary>
    /// <param name="orcInhabitant">the customer to remove</param>
    public void removeCustomer(OrcInhabitant orcInhabitant)
    {
        if (customers.Contains(orcInhabitant))
        {
            customers.Remove(orcInhabitant);
        }
    }

    /// <summary>
    /// Adds a customer to the barber
    /// </summary>
    /// <param name="orcInhabitant">the customer to add</param>
    public void addCustomer(OrcInhabitant orcInhabitant)
    {
        if (customers.Count < maxCustomers && !customers.Contains(orcInhabitant))
        {
            customers.Add(orcInhabitant);
        }
    }

    /// <summary>
    /// Gets the number of customers using this barber
    /// </summary>
    /// <returns>The number of customers using this barber</returns>
    public int getNumCustomers()
    {
        return customers.Count;
    }

    /// <summary>
    /// Gets the maximum number of customers this barber can support
    /// </summary>
    /// <returns>The maximum number of customers this barber can support</returns>
    public int getNumMaxCustomers()
    {
        return maxCustomers;
    }
}
