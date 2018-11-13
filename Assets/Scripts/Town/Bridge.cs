using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour {
    public Sprite northConnection;
    public Sprite southConnection;
    public Sprite westConnection;
    public Sprite eastConnection;
    public Sprite verticalMiddle;
    public Sprite horizontalMiddle;

    private List<GameObject> connectedBridgeObjects;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setConnectedBridgeObjects(List<GameObject> connectedBridgeObjects)
    {
        this.connectedBridgeObjects = connectedBridgeObjects;
    }
}
