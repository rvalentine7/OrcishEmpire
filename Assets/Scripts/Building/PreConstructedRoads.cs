using UnityEngine;
using System.Collections;

/// <summary>
/// Adds roads already created on the map to the construct array
/// </summary>
public class PreConstructedRoads : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector2 roadPos = transform.position;
        World myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        myWorld.constructNetwork.setConstructArr((int)roadPos.x, (int)roadPos.y, gameObject);
    }
}
