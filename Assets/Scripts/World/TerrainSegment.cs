using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Adds this terrain GameObject to the terrain network.
 */
public class TerrainSegment : MonoBehaviour {
    public List<Sprite> spriteChoices;

	/**
     * Adds the terrain segment to the terrain array.
     */
	void Start () {
        Vector2 terrainPos = transform.position;
        GameObject world = GameObject.Find("WorldInformation");
        World myWorld = world.GetComponent<World>();
        myWorld.terrainNetwork.setTerrainArr((int)terrainPos.x, (int)terrainPos.y, gameObject);

        if (spriteChoices.Count > 0)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            int groundSpriteNum = Mathf.FloorToInt(Random.value * spriteChoices.Count);
            spriteRenderer.sprite = spriteChoices[groundSpriteNum];
        }
    }
}
