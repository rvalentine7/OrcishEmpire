using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles variables used by every type of build mode and moves a grid around with the cursor
/// </summary>
public class BuildMode : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject grid;
    public AudioClip buildAudioClip;

    protected World myWorld;
    protected Vector2 mousePos;

    private GameObject gridObject;

    /// <summary>
    /// Initializes the build mode
    /// </summary>
    private void Awake()
    {
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        gridObject = Instantiate(grid, new Vector2(0.0f, 0.0f), Quaternion.identity) as GameObject;
    }

    /// <summary>
    /// Destroys the grid
    /// </summary>
    private void OnDestroy()
    {
        Destroy(gridObject);
    }

    /// <summary>
    /// Updates the basic variables and grid for every build mode
    /// </summary>
    protected void updateBuildMode()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = Mathf.RoundToInt(mousePos.x);
        mousePos.y = Mathf.RoundToInt(mousePos.y);

        Vector2 gridVector = new Vector2(mousePos.x, mousePos.y);
        gridObject.transform.position = Vector2.Lerp(transform.position, gridVector, 1f);
    }
}
