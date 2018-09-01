using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReservoirClick : MonoBehaviour {
    public GameObject reservoirPopupObject;

    /**
     * Click the object to see information about it.
     */
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameObject.FindWithTag("Popup") == null)
        {
            GameObject popup = Instantiate(reservoirPopupObject) as GameObject;
            ReservoirPopup reservoirPopup = popup.GetComponent<ReservoirPopup>();
            reservoirPopup.setReservoir(gameObject);
        }
    }
}
