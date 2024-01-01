using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSEngine;
using RTSEngine.Game;

public class PlayerStartLocation : MonoBehaviour
{
    public bool ManualOverride = false;
    public RTSEScenePrepManager RTSESCPM = null;
    private Vector3 CurrentPos = Vector3.zero;
    public Transform BuildingsParent = null;
    public Transform UnitsParent = null;
    public Transform ResourcesParent = null;
    public Transform PositionRing = null;
    public Transform PositionPin = null;
    public Transform PositionPinTop = null;
    public Transform AboveTerrainTransform = null;
    public Transform BelowTerrainTransform = null;
    public List<FactionBuildingMarker> factionBuildingMarkers = new();
    public List<FactionUnitMarker> factionUnitMarkers = new();
    public List<FactionResourceMarker> factionResourceMarkers = new();
    public bool SnapToTerrain = false;
    public bool SnappedToTerrain = false;
    public bool SnapWhileMoving = false;
    private void OnDrawGizmosSelected()
    {
        CurrentPos = transform.position;
        if (SnapToTerrain && !SnappedToTerrain || SnapToTerrain && !SnappedToTerrain && SnapWhileMoving)
        {
            if(this.RTSESCPM.TerrainsParent.TryGetComponent(out Terrain terrain)){
                transform.position = new Vector3(
                    transform.position.x,
                    this.RTSESCPM.TerrainsParent.GetComponent<Terrain>().SampleHeight(transform.position),
                    transform.position.z
                );
            }
            else
            {
                foreach(Transform child in this.RTSESCPM.TerrainsParent.transform)
                {
                    if(child.TryGetComponent(out Terrain terrain1))
                    {
                        transform.position = new Vector3(
                            transform.position.x,
                            terrain1.SampleHeight(transform.position),
                            transform.position.z
                        );
                    }
                }
            }
        }
    }
}
