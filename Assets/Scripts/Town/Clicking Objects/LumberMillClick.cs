﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LumberMillClick : MonoBehaviour {
    public GameObject productionPopupObject;

    /**
     * Click the object to see information about it.
     */
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameObject.FindWithTag("Popup") == null)
        {
            GameObject popup = Instantiate(productionPopupObject) as GameObject;
            LumberMillPopup lumberMillPopup = popup.GetComponent<LumberMillPopup>();
            lumberMillPopup.setLumberMill(gameObject);
        }
    }
}