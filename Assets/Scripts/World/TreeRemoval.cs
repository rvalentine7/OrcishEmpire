using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Removes the tree and replaces itself with a patch of land
/// </summary>
public class TreeRemoval : MonoBehaviour {
    public GameObject ground;
    private GameObject world;
    private World myWorld;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start () {
        world = GameObject.Find(World.WORLD_INFORMATION);
        myWorld = world.GetComponent<World>();
    }

    /// <summary>
    /// Removes the trees gameobject this script is attached to and replaces it with a ground tile
    /// </summary>
    public void removeTree()
    {
        Vector2 treeLocation = gameObject.transform.position;
        GameObject groundObject = Instantiate(ground, treeLocation, Quaternion.identity) as GameObject;
        myWorld.terrainNetwork.setTerrainArr((int)treeLocation.x, (int)treeLocation.y, groundObject);
        Destroy(gameObject);
    }
}
