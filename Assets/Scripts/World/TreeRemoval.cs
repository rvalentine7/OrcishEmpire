using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRemoval : MonoBehaviour {
    public GameObject ground;
    private GameObject world;
    private World myWorld;

    // Use this for initialization
    void Start () {
        world = GameObject.Find(World.WORLD_INFORMATION);
        myWorld = world.GetComponent<World>();
    }

    /**
     * Removes the trees gameobject this script is attached to and replaces it with a ground tile
     */
    public void removeTree()
    {
        Vector2 treeLocation = gameObject.transform.position;
        GameObject groundObject = Instantiate(ground, treeLocation, Quaternion.identity) as GameObject;
        myWorld.terrainNetwork.setTerrainArr((int)treeLocation.x, (int)treeLocation.y, groundObject);
        Destroy(gameObject);
    }
}
