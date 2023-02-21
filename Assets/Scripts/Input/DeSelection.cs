using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeSelection : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GetComponent<Snapping>() == null) {
            if (!InputManager.manager.touchInput) {
                if(SnappingManager.manager && SnappingManager.manager.currentSnappedObject)
                SnappingManager.manager.currentSnappedObject.DeSelect();
            }
        }
    }
}
