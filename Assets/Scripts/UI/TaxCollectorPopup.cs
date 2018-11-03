using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaxCollectorPopup : MonoBehaviour {
    public GameObject taxCollector;
    public Text status;
    public Text employeeNum;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
    }

    /**
     * Removes the game object from the game.
     */
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    /**
     * Sets the reservoir object this popup is displaying information on.
     * @param reservoir the reservoir the popup is displaying information on
     */
    public void setTaxCollector(GameObject taxCollector)
    {
        this.taxCollector = taxCollector;
    }
}
