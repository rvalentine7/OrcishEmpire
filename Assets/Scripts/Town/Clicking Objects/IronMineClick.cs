using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IronMineClick : MonoBehaviour {
    public GameObject productionPopupObject;

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
            GameObject popup = Instantiate(productionPopupObject) as GameObject;
            IronMinePopup ironMinePopup = popup.GetComponent<IronMinePopup>();
            ironMinePopup.setIronMine(gameObject);
        }
    }
}
