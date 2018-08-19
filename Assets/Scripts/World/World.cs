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
    public Vector2 exitLocation;//locations where emmigrants will exit the world
    public List<string> walkableTerrain;
    public List<string> nonWalkableTerrain;
    public List<string> buildableTerrain;
    public List<string> mountainousTerrain;
    public List<string> wateryTerrain;

    /**
     * Initializes the necessary information for a world.
     */
    void Awake()
    {
        //eventually change these to be general and passed in by a level
        // manager
        constructNetwork = new ConstructionNetwork();
        terrainNetwork = new TerrainNetwork();
        mapSize = 42;
        spawnLocation = new Vector2(0f, 5.41f);
        exitLocation = new Vector2(39f, 5.41f);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //with a level manager, I will need getters and setters for information such as "spawnLocation" so that
    // the level manager can input the data and all other classes will get the data from getters
}
