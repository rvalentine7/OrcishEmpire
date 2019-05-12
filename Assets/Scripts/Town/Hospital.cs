using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A hospital improves the rate at which orcs recover from being sick
 * and helps prevent the spread of sickness
 */
public class Hospital : MonoBehaviour
{
    public int numHospitalBeds;

    private List<OrcInhabitant> sickOrcs;
    private int numAvailableBeds;
    Employment employment;
    private int maxEmployeeNum;

    private void Awake()
    {
        sickOrcs = new List<OrcInhabitant>();
    }

    // Start is called before the first frame update
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

    // Update is called once per frame
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

    /**
     * Add a sick orc to this hospital if there are enough beds
     * @param sickOrc the orc to be admitted to this hospital
     * @return whether the orc was successfully admitted to the hospital
     */
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

    /**
     * Remove an orc from the hospital.  This will either be due to a reduction in staff
     * or due to the orc recovering from sickness.
     * @param sickOrc the orc in question
     */
    public void removeSickOrc(OrcInhabitant sickOrc)
    {
        sickOrcs.Remove(sickOrc);
        sickOrc.removeHospital();
    }

    /**
     * Gets the number of sick orcs recovering at this hospital
     */
    public int getNumPatients()
    {
        return sickOrcs.Count;
    }

    /**
     * Gets the number of beds this hospital currently has for patients
     */
    public int getNumAvailableBeds()
    {
        return numAvailableBeds;
    }
}
