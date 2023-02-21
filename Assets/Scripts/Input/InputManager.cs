using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    
    public static InputManager manager;
    public ActionMap inputActions;
    public bool primaryTouchStart;
    public float zoomAmount;
    
    [SerializeField] bool allowZoom;
    [SerializeField] bool allowMove;
    [HideInInspector] public Vector2 pointerPosition;
    [HideInInspector] public bool touchInput;
    [HideInInspector] public bool overUI;


    private Vector2 moveDirection;
    private float zoomPinchAmount;
    private float zoomScroolAmount;
    private bool pinchZooming;
    private Coroutine zoomCoroutine;
    private Vector2 touchOne;
    private Vector2 touchZero;
   
   
    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void InitializeManager()
    {
        if (manager != null && manager != this)
        { Destroy(gameObject); return; }
        manager = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Awake()
    {
        InitializeManager();
        inputActions = new ActionMap();

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR        
        touchInput = true;
#else
        touchInput=false;
#endif
     
    }
    private void Start()
    {
        inputActions.Zoom.SecondaryTouchStart.started += _ => ZoomStart();
        inputActions.Zoom.SecondaryTouchStart.canceled += _ => ZoomEnd();
        inputActions.Move.PrimaryTouchStart.started += _ => MoveStart();
        inputActions.Move.PrimaryTouchStart.canceled += _ => MoveEnd();
    }

    private void MoveEnd()
    {
        primaryTouchStart = false;
    }

    private void MoveStart()
    {
        primaryTouchStart = true;
    }

    private void ZoomEnd()
    {

        StopCoroutine(zoomCoroutine);
        zoomPinchAmount = 0;
        pinchZooming = false;
    }

    private void ZoomStart()
    {

        zoomCoroutine = StartCoroutine(ZoomPinch());
        pinchZooming = true;

    }

   
    void Update()
    {
        HandleZoom();
        HandleMove();
        UpdatePointerPosition();
    }
   
    private void UpdatePointerPosition()
    {
        pointerPosition = inputActions.Mouse.MousePosition.ReadValue<Vector2>();
    }

    private void HandleMove()
    {
        if (!allowMove || pinchZooming) {
            moveDirection = Vector2.zero;
            return;
        }
        if (inputActions.Move.MoveDir.ReadValue<Vector2>().magnitude > 0.1)
        {
            moveDirection = inputActions.Move.MoveDir.ReadValue<Vector2>();
        }
        else
        {
            moveDirection = Vector2.Lerp(moveDirection, inputActions.Move.MoveDir.ReadValue<Vector2>(), Time.deltaTime * 25);
        }
    }

    private void HandleZoom()
    {
        if (!allowZoom) {
            zoomAmount = 0;
            return;
        } 
        zoomScroolAmount = inputActions.Zoom.Scroll.ReadValue<Vector2>().y;
        zoomAmount = zoomScroolAmount + (zoomPinchAmount!=0? (zoomPinchAmount>0?1:-1):0);        
    }

    IEnumerator ZoomPinch() {

        float previousDistance = 0;
        touchZero = inputActions.Zoom.PrimaryTouchPosition.ReadValue<Vector2>();
        touchOne = inputActions.Zoom.SecondaryTouchPosition.ReadValue<Vector2>();
        float distance = Vector2.Distance(touchZero,touchOne);
        while (true)
        {
            touchZero = inputActions.Zoom.PrimaryTouchPosition.ReadValue<Vector2>();
            touchOne = inputActions.Zoom.SecondaryTouchPosition.ReadValue<Vector2>();
            distance = Vector2.Distance(touchZero, touchOne);

            if (distance > previousDistance)
            {
                zoomPinchAmount = (distance - previousDistance);
            }
            else if(distance < previousDistance)
            {
                zoomPinchAmount = (distance - previousDistance);
            }


            previousDistance = distance;
            yield return null;
        }
    }
}
