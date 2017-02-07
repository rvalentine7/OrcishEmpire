using UnityEngine;
using System.Collections;

/**
 * Adds roads already created on the map to the construct array
 */
public class PreConstructedRoads : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector2 roadPos = transform.position;
        GameObject world = GameObject.Find("WorldInformation");
        World myWorld = world.GetComponent<World>();
        myWorld.constructNetwork.setConstructArr((int)roadPos.x, (int)roadPos.y, gameObject);
    }
}
