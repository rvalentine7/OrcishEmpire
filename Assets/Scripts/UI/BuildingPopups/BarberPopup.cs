using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// A popup for a barber
/// </summary>
public class BarberPopup : EmploymentPopup
{
    public Text status;
    public Text employeeNum;
    public Text sickEmployeeNum;
    public Text customersNum;

    /// <summary>
    /// Updates the barber popup
    /// </summary>
    new void Update()
    {
        employmentUpdate();
        
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        sickEmployeeNum.text = "" + (employment.getNumWorkers() - employment.getNumHealthyWorkers()) + "/" + employment.getNumWorkers();
        Barber barberClass = objectOfPopup.GetComponent<Barber>();
        this.customersNum.text = "" + barberClass.getNumCustomers() + "/" + barberClass.getNumMaxCustomers();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "No orcs can visit this barber while it has no employees.";
        }
        else if (employment.getWorkerCap() > employment.getNumHealthyWorkers())
        {
            status.text = "This barber is not serving as many customers as it could due to its lack of employees.";
        }
        else
        {
            status.text = "This barber is providing haircuts to nearby orcs.";
        }
    }
}
