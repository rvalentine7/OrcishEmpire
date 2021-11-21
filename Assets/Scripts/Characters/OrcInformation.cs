using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Keeps track of information regarding an orc that the player might be interested
/// in knowing.
/// </summary>
public class OrcInformation : MonoBehaviour {
    private int orcCount;
    //will want to keep track of where an orc is working

    /// <summary>
    /// Initializes the number of orcs contained in this game object.
    /// </summary>
    void Start () {
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetHashCode();
	}

    /// <summary>
    /// Sets the number of orcs contained in this game object.
    /// </summary>
    /// <param name="num">The number of orcs</param>
    public void setOrcCount(int num)
    {
        orcCount = num;
    }

    /// <summary>
    /// Gets the number of orcs contained in this game object.
    /// </summary>
    /// <returns>The number of orcs contained in this object</returns>
    public int getOrcCount()
    {
        return orcCount;
    }
}
