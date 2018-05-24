using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GladiatorPit : MonoBehaviour {
    private int numReadyGladiators;

    public int timeToTrain;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //number of workers impact how quickly gladiators can be trained
        //when gladiators are trained, send them to arenas
        Employment employment = gameObject.GetComponent<Employment>();
        if (employment.getNumWorkers() > 0)
        {

        }
	}
}
