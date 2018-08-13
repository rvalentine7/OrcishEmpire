using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsmithPopup : MonoBehaviour {
    public GameObject weaponsmith;
    public Text status;
    public Text employeeNum;
    public Text ironNum;
    public Text progressNum;

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
        ItemProduction thisWeaponsmith = weaponsmith.GetComponent<ItemProduction>();
        progressNum.text = "" + thisWeaponsmith.getProgressNum() + "/100";
        Employment employment = weaponsmith.GetComponent<Employment>();
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        Storage storage = weaponsmith.GetComponent<Storage>();
        ironNum.text = "" + storage.getIronCount();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "Without any employees, this weaponsmith cannot produce any weapons.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "This smithy is producing slowly due to a lack of employees.";
        }
        else
        {
            status.text = "This smithy is producing weapons at peak efficiency.";
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
     * Sets the weaponsmith object this popup is displaying information on.
     * @param weaponsmith the weaponsmith the popup is displaying information on
     */
    public void setWeaponsmith(GameObject weaponsmith)
    {
        this.weaponsmith = weaponsmith;
    }
}
