using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GladiatorPitClick : MonoBehaviour {
    public GameObject pitPopupObject;

    /**
     * Click the object to see information about it.
     */
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameObject.FindWithTag("Popup") == null)
        {
            GameObject popup = Instantiate(pitPopupObject) as GameObject;
            GladiatorPitPopup gladiatorPitPopup = popup.GetComponent<GladiatorPitPopup>();
            gladiatorPitPopup.setGladiatorPit(gameObject);
        }
    }
}
