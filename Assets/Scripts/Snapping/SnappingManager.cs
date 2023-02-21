using System.Collections.Generic;
using UnityEngine;

public class SnappingManager : MonoBehaviour
{
    public static SnappingManager manager;
    public Snapping currentSnappedObject;
    public List<Tile> tiles = new List<Tile>();
    public Vector3 targetPosition;
    public float snapDistance = 1;

    private Vector3 worldPosition;  
    private Plane plane = new Plane(Vector3.up, 0);

    private void Awake()
    {
        if (manager != null && manager != this)
        {
            Destroy(gameObject);
            return;
        }
        manager = this;
    }
    private void Update()
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(InputManager.manager.pointerPosition);
        if (plane.Raycast(ray, out distance))
        {
            worldPosition = ray.GetPoint(distance);
        }
        targetPosition = worldPosition;
    }
    public void RemoveReference() {
        currentSnappedObject = null;
    }
   
}
