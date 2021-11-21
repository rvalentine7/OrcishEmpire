using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A hospital improves the rate at which orcs recover from being sick
/// and helps prevent the spread of sickness
/// </summary>
public class Hospital : Building
{
    public int numHospitalBeds;

    private List<OrcInhabitant> sickOrcs;
    private int numAvailableBeds;
    Employment employment;
    private int maxEmployeeNum;

    /// <summary>
    /// Initialization that must occur before anything else
    /// </summary>
    private void Awake()
    {
        sickOrcs = new List<OrcInhabitant>();
    }

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        numAvailableBeds = 0;
        employment = gameObject.GetComponent<Employment>();
        maxEmployeeNum = employment.getWorkerCap();

        //Search for houses to add this hospital to the nearbyHospitals list
        GameObject world = GameObject.Find(World.WORLD_INFORMATION);
        World myWorld = world.GetComponent<World>();
        GameObject[,] constructArr = myWorld.constructNetwork.getConstructArr();
        Vector2 hospitalPosition = gameObject.transform.position;
        int healthBuildingRadius = World.HEALTH_BUILDING_RADIUS + 1;//+1 because it is a 3x3 building unlike the house which is 1x1
        for (int i = -healthBuildingRadius; i < healthBuildingRadius; i++)
        {
            for (int j = -healthBuildingRadius; j < healthBuildingRadius; j++)
            {
                if (hospitalPosition.x + i >= 0 && hospitalPosition.y + j >= 0
                        && hospitalPosition.x + i <= myWorld.mapSize - 1 && hospitalPosition.y + j <= myWorld.mapSize - 1
                        && constructArr[(int)hospitalPosition.x + i, (int)hospitalPosition.y + j] != null
                        && constructArr[(int)hospitalPosition.x + i, (int)hospitalPosition.y + j].tag == World.HOUSE)
                {
                    HouseInformation houseInfo = constructArr[(int)hospitalPosition.x + i, (int)hospitalPosition.y + j].GetComponent<HouseInformation>();
                    houseInfo.addHospital(gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Updates information for the hospital
    /// </summary>
    void Update()
    {
        double numEmployees = employment.getNumHealthyWorkers();
        numAvailableBeds = Mathf.FloorToInt((float) (numEmployees / maxEmployeeNum) * numHospitalBeds);

        //If the number of employees decreases, the number of available beds also decreases
        while (numAvailableBeds < sickOrcs.Count)
        {
            sickOrcs.RemoveAt(sickOrcs.Count - 1);
        }
    }

    /// <summary>
    /// Add a sick orc to this hospital if there are enough beds
    /// </summary>
    /// <param name="sickOrc">the orc to be admitted to this hospital</param>
    /// <returns>whether the orc was successfully admitted to the hospital</returns>
    public bool addSickOrc(OrcInhabitant sickOrc)
    {
        if (numAvailableBeds - sickOrcs.Count > 0)
        {
            sickOrcs.Add(sickOrc);
            sickOrc.setHospital(gameObject);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Remove an orc from the hospital.  This will either be due to a reduction in staff
    /// or due to the orc recovering from sickness.
    /// </summary>
    /// <param name="sickOrc">the orc in question</param>
    public void removeSickOrc(OrcInhabitant sickOrc)
    {
        sickOrcs.Remove(sickOrc);
        sickOrc.removeHospital();
    }

    /// <summary>
    /// Removes an orc from the list of sick orcs
    /// </summary>
    /// <param name="sickOrc">The orc that is no longer at the hospital</param>
    public void sickOrcLeft(OrcInhabitant sickOrc)
    {
        sickOrcs.Remove(sickOrc);
    }

    /// <summary>
    /// Gets the number of sick orcs recovering at this hospital
    /// </summary>
    /// <returns>The number of sick orcs recovering at this hospital</returns>
    public int getNumPatients()
    {
        return sickOrcs.Count;
    }

    /// <summary>
    /// Gets the number of beds this hospital currently has for patients
    /// </summary>
    /// <returns>The number of beds this hospital currently has for patients</returns>
    public int getNumAvailableBeds()
    {
        return numAvailableBeds;
    }
}
