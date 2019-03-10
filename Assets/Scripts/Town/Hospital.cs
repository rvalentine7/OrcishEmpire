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

    private List<GameObject> sickOrcs;
    private int numAvailableBeds;

    // Start is called before the first frame update
    void Start()
    {
        sickOrcs = new List<GameObject>();
        numAvailableBeds = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: if there are sick orcs, help them recover
        //numAvailableBeds updates based on numWorkers... get this from employment
    }
}
