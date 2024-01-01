using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PositionPinSelect : MonoBehaviour
{
    public bool IsSelected = false;
    public PlayerStartLocation startLocation = null;

    public void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (startLocation != null)
        {
            if (!startLocation.ManualOverride)
            {
                Selection.activeObject = startLocation;
                this.enabled = false;
            }
        }
#endif
    }

    public void OnDisable()
    {
        IsSelected = false;
    }

}
