using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldUI : MonoBehaviour
{
    public GameObject cityPopupPanel;
    public GameObject playerTradeManagementPanel;
    protected bool initialClick;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        initialClick = false;
    }

    /// <summary>
    /// Updates popup
    /// </summary>
    protected void Update()
    {
        if (initialClick && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)))
        {
            initialClick = false;
        }
        if (Input.GetKey(KeyCode.Escape) || (!initialClick
            && !EventSystem.current.IsPointerOverGameObject()
            && (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))))
        {
            gameObject.SetActive(false); ;
        }
    }

    private void OnDisable()
    {
        cityPopupPanel.SetActive(false);
        playerTradeManagementPanel.SetActive(false);
    }
}
