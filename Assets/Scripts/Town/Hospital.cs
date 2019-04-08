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

    private List<SickOrc> sickOrcs;
    private int numAvailableBeds;
    Employment employment;
    private int maxEmployeeNum;

    // Start is called before the first frame update
    void Start()
    {
        sickOrcs = new List<SickOrc>();
        numAvailableBeds = 0;
        employment = gameObject.GetComponent<Employment>();
        maxEmployeeNum = employment.getWorkerCap();
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
    public bool addSickOrc(SickOrc sickOrc)
    {
        if (numAvailableBeds - sickOrcs.Count > 0)
        {
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
    public void removeSickOrc(SickOrc sickOrc)
    {
        sickOrcs.Remove(sickOrc);
        sickOrc.removeHospital();
    }
}
