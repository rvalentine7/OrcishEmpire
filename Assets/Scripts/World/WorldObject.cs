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
    public AudioSource clickSound;
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
            GameObject worldUIObject = GameObject.FindWithTag(World.WORLD_UI);
            if (worldUIObject != null)
            {
                worldUIObject.SetActive(false);
            }
            GameObject instantiatedWorldObjectPopup = Instantiate(worldObjectPopup) as GameObject;
            Popup popupOfWorldObject = instantiatedWorldObjectPopup.GetComponent<Popup>();
            popupOfWorldObject.setGameObject(gameObject);
            createdPopup = instantiatedWorldObjectPopup;

            //sound
            if (clickSound != null)
            {
                clickSound.volume = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>().getSettingsMenu().getClickVolume();
                clickSound.Play();
            }
        }
    }

    /// <summary>
    /// Destroys the popup if it still exists
    /// </summary>
    protected void OnDestroy()
    {
        if (createdPopup)
        {
            Destroy(createdPopup);
        }
    }
}
