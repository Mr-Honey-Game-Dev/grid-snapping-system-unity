using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(GraphicRaycaster))]
public class UiHoverDetect : MonoBehaviour
{
    EventTrigger eventTrigger;
    bool lastUpdatedFlag = false;
    private void Awake()
    {
        _graphicRaycaster = GetComponent<GraphicRaycaster>();
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null)
            eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventdata) => { MouseEnter(); });
        eventTrigger.triggers.Add(entryEnter);



        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventdata) => { MouseExit(); });
        eventTrigger.triggers.Add(entryExit);

        
    }

    public void MouseEnter()
    {
        InputManager.manager.overUI = true;
    }
    public void MouseExit()
    {
        InputManager.manager.overUI = false;
    }
    private GraphicRaycaster _graphicRaycaster;

 
    private void Update()
    {
        if (IsPointerOverUI())
        {
            InputManager.manager.overUI = IsPointerOverUI();
            lastUpdatedFlag = true;
        }
        else 
        {
            if (lastUpdatedFlag)
            {
                InputManager.manager.overUI = IsPointerOverUI();
                lastUpdatedFlag = false;
            }
        }
    }
    public bool IsPointerOverUI()
    {
        // Obtain the current mouse position.
        var mousePosition =InputManager.manager.pointerPosition;

        // Create a pointer event data structure with the current mouse position.
        var pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = mousePosition;

        // Use the GraphicRaycaster instance to determine how many UI items
        // the pointer event hits.  If this value is greater-than zero, skip
        // further processing.
        var results = new List<RaycastResult>();
        _graphicRaycaster.Raycast(pointerEventData, results);
        return results.Count > 0;
    }
}
