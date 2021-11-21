using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles things required by every building
/// TODO: actually give this to every building... right now, this could be used for clicking each building (instead of all the different click scripts).
/// In the future, this class will be useful for maintenance, fire safety (making sure the building has water), and overlays (displaying what this building has access to)
/// TODO: this should implement a WorldObject which implements MonoBehaviour.  WorldObject would be used for clicking on objects in the game world and maybe some other stuff?
/// </summary>
public class Building : WorldObject
{
    public GameObject getGameObject()
    {
        return this.gameObject;
    }
}
