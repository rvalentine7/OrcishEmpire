using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles things required by every building
/// TODO: actually give this to every building... right now, this could be used for clicking each building (instead of all the different click scripts).
/// In the future, this class will be useful for maintenance, fire safety (making sure the building has water), and overlays (displaying what this building has access to)
/// </summary>
public class Building : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject getGameObject()
    {
        return this.gameObject;
    }
}
