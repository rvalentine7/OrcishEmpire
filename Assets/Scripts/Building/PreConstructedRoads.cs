﻿using UnityEngine;
using System.Collections;

/**
 * Adds roads already created on the map to the construct array
 */
public class PreConstructedRoads : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector2 roadPos = transform.position;
        GameObject myCamera = GameObject.Find("Main Camera");
        Controls myControls = myCamera.GetComponent<Controls>();
        myControls.constructNetwork.setConstructArr((int)roadPos.y, (int)roadPos.x, gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}