using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        /*List<int> meatInfo = storage.getMeatInfo();
        Debug.Log("Meat: " + meatInfo[0] + "/" + meatInfo[1]);*/
        if (GameObject.FindWithTag("Popup") == null)
        {
            GameObject popup = Instantiate(warehousePopupObject) as GameObject;
            WarehousePopup warehousePopup = popup.GetComponent<WarehousePopup>();
            warehousePopup.setWarehouse(gameObject);
        }
    }
}
