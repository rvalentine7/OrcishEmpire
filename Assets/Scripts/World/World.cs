using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Contains information about the world the player is in.
 */
public class World : MonoBehaviour {
    public ConstructionNetwork constructNetwork;
    public TerrainNetwork terrainNetwork;
    public int mapSize;
    public Vector2 spawnLocation;//location where immigrants will spawn in
    public Vector2 exitLocation;//locations wher emmigrants will exit the world

    /**
     * Initializes the necessary information for a world.
     */
    void Awake()
    {
        constructNetwork = new ConstructionNetwork();
        terrainNetwork = new TerrainNetwork();
        mapSize = 42;
        spawnLocation = new Vector2(0f, 5.41f);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
