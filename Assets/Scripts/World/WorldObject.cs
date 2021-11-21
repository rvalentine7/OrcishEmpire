using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// An object in the world that can be interacted with
/// </summary>
public class WorldObject : MonoBehaviour
{
    public GameObject worldObjectPopup;
    private GameObject createdPopup;
    
    /// <summary>
    /// Create a popup for the world object when clicked on
    /// </summary>
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameObject.FindWithTag(World.BUILD_OBJECT) == null)
        {
            GameObject otherPopupObject = GameObject.FindWithTag(World.POPUP);
            if (otherPopupObject != null)
            {
                Destroy(otherPopupObject);
            }
            GameObject instantiatedWorldObjectPopup = Instantiate(worldObjectPopup) as GameObject;
            Popup popupOfWorldObject = instantiatedWorldObjectPopup.GetComponent<Popup>();
            popupOfWorldObject.setGameObject(gameObject);
            createdPopup = instantiatedWorldObjectPopup;
        }
    }

    /// <summary>
    /// Destroys the popup if it still exists
    /// </summary>
    private void OnDestroy()
    {
        if (createdPopup)
        {
            Destroy(createdPopup);
        }
    }
}
