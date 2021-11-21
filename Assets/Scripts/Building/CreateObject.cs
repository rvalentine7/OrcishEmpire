using UnityEngine;
using System.Collections;

/// <summary>
/// Used by the building creation buttons in the UI to enter build mode for a particular structure
/// </summary>
public class CreateObject : MonoBehaviour
{
    public GameObject construct;

    /// <summary>
    /// Creates a new road object to display where roads can be built and what they will look like when built.
    /// </summary>
    public void Create ()
    {
        //Destroys the existing popup/building creation object if the user attempts to create something else
        GameObject buildObjectToDestroy = GameObject.FindWithTag(World.BUILD_OBJECT);
        if (buildObjectToDestroy != null)
        {
            Destroy(buildObjectToDestroy);
        }
        GameObject constructObj = Instantiate(construct, Input.mousePosition, Quaternion.identity) as GameObject;
        constructObj.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
