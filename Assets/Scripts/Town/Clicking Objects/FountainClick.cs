using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FountainClick : MonoBehaviour {
    public GameObject fountainPopupObject;

    /**
     * Click the object to see information about it.
     */
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameObject.FindWithTag("Popup") == null)
        {
            GameObject popup = Instantiate(fountainPopupObject) as GameObject;
            FountainPopup fountainPopup = popup.GetComponent<FountainPopup>();
            fountainPopup.setFountain(gameObject);
        }
    }
}
