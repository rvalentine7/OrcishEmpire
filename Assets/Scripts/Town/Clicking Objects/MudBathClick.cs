using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MudBathClick : MonoBehaviour
{
    public GameObject mudBathPopupObject;

    /**
     * Click the object to see information about it.
     */
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameObject.FindWithTag(World.BUILD_OBJECT) == null)
        {
            GameObject popupObject = GameObject.FindWithTag(World.POPUP);
            if (popupObject != null)
            {
                Destroy(popupObject);
            }
            GameObject popup = Instantiate(mudBathPopupObject) as GameObject;
            MudBathPopup mudBathPopup = popup.GetComponent<MudBathPopup>();
            mudBathPopup.setMudBath(gameObject);
        }
    }
}
