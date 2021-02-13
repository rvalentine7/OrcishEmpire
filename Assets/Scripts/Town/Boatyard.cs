using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boatyard : MonoBehaviour
{
    public GameObject collectorOrc;
    public int collectorCarryCapacity;
    public float timeInterval;
    public string requiredResourceName;
    public string resourceName;
    public int resourceProduced;
    private int progress;
    private float checkTime;
    private int numWorkers;
    private int workerValue;
    private Employment employment;
    private Storage storage;
    private int requiredResourcesInUse;
    private bool orcMovingGoods;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
