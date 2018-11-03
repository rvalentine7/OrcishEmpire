using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TaxCollectorClick : MonoBehaviour {
    public GameObject taxCollectorPopupObject;

    /**
     * Click the object to see information about it.
     */
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameObject.FindWithTag("Popup") == null)
        {
            GameObject popup = Instantiate(taxCollectorPopupObject) as GameObject;
            TaxCollectorPopup taxCollectorPopup = popup.GetComponent<TaxCollectorPopup>();
            taxCollectorPopup.setTaxCollector(gameObject);
        }
    }
}
