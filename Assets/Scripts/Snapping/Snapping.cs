using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snapping : MonoBehaviour
{
      
    bool selected;
    Tile tileSnappedTo;
    private void Start()
    {
        SnapToNearestFreeTile();
    }

    void LateUpdate()
    {   
        if (selected)
        {
            transform.position =SnappingManager.manager.targetPosition;

            float smallestDistance = SnappingManager.manager.snapDistance;

            if(tileSnappedTo)
            tileSnappedTo.preOccupied = false;
           
            foreach (Tile tile in SnappingManager.manager.tiles)
            {
                if (!tile.preOccupied && Vector3.Distance(tile.transform.position, SnappingManager.manager.targetPosition) < smallestDistance)
                {
                    transform.position = tile.transform.position;
                    smallestDistance = Vector3.Distance(tile.transform.position, SnappingManager.manager.targetPosition);
                    tileSnappedTo = tile;
                }
            }
            tileSnappedTo.preOccupied = true;
        }
    }
    public void OnMouseDown()
    {
        if (!InputManager.manager.touchInput) {
            if (selected)
                DeSelect();
            else
                Invoke(nameof(Select), 2*Time.deltaTime);
        }
    }
    public void OnMouseDrag()
    {
        if (InputManager.manager.touchInput && InputManager.manager.primaryTouchStart)
        {
            if (!selected)
                Invoke(nameof(Select),2*Time.deltaTime);
          
        }
    }
    public void OnMouseUp()
    {
        if (InputManager.manager.touchInput && selected)
            DeSelect();
    }
    public void SnapToNearestFreeTile() {

        int firstFreeTileIndex=0;
        Tile nearestTile= SnappingManager.manager.tiles[0];
        for (int i = 0; i < SnappingManager.manager.tiles.Count; i++) {
            if (!SnappingManager.manager.tiles[i].preOccupied || SnappingManager.manager.tiles[i]==tileSnappedTo) {
                nearestTile = SnappingManager.manager.tiles[i];
                firstFreeTileIndex = i;
                break;
            }
        }
        if (tileSnappedTo)
            tileSnappedTo.preOccupied = false;
        for(int i=firstFreeTileIndex;i< SnappingManager.manager.tiles.Count; i++)
        {
            if (!SnappingManager.manager.tiles[i].preOccupied && Vector3.Distance(nearestTile.transform.position, transform.position) > Vector3.Distance(SnappingManager.manager.tiles[i].transform.position, transform.position))
            {
                nearestTile = SnappingManager.manager.tiles[i];
            }
        }
        transform.position = nearestTile.transform.position;
        tileSnappedTo= nearestTile;
        tileSnappedTo.preOccupied = true;
    }

    public void SnapToTile(int tileIndex) {
        if (SnappingManager.manager.tiles.Exists(x => x.index == tileIndex) && !SnappingManager.manager.tiles.Find(x => x.index == tileIndex).preOccupied ){
            Tile tile = SnappingManager.manager.tiles.Find(x => x.index == tileIndex);
            transform.position = tile.transform.position;
            tileSnappedTo = tile;
            tile.preOccupied = true;
        }
        else {
            SnapToNearestFreeTile();
        }
    }
    public void ReleaseTile() {
        if (tileSnappedTo != null) {
            tileSnappedTo.preOccupied = false;
            tileSnappedTo = null;
        }
    }
    public void Select() {

        if (InputManager.manager.overUI) return;
       
        if (SnappingManager.manager.currentSnappedObject != null && SnappingManager.manager.currentSnappedObject != this)
            SnappingManager.manager.currentSnappedObject.DeSelect();
            SnappingManager.manager.currentSnappedObject = this;
        selected = true;

    }
    public void DeSelect() {

        if (selected)
        {
            if (InputManager.manager.touchInput)
            { Invoke(nameof(RemoveReference), 0.15f); }
            else
            { RemoveReference();  }
            
        }
    }
    public void RemoveReference() {
        SnappingManager.manager.currentSnappedObject = null;
        selected = false;
        SnapToNearestFreeTile();
    }
}
