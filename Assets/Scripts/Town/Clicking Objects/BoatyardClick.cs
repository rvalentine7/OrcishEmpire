using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoatyardClick : MonoBehaviour
{
    public GameObject boatyardPopupObject;

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
            GameObject popup = Instantiate(boatyardPopupObject) as GameObject;
            BoatyardPopup boatyardPopup = popup.GetComponent<BoatyardPopup>();
            boatyardPopup.setBoatyard(gameObject);
        }
    }
}
