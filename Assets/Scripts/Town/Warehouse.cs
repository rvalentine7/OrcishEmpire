using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Warehouse : MonoBehaviour {
    public GameObject warehousePopupObject;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
     * Returns information on what is stored at the warehouse.
     */
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameObject.FindWithTag("Popup") == null)
        {
            GameObject popup = Instantiate(warehousePopupObject) as GameObject;
            WarehousePopup warehousePopup = popup.GetComponent<WarehousePopup>();
            warehousePopup.setWarehouse(gameObject);
        }
    }
}
