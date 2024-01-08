using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditor(typeof(PlayerStartLocation))]
public class PlayerStartLocationEditor : Editor
{
    private PlayerStartLocation Instance;
    SerializedProperty ManualOverride;
    SerializedProperty RTSESCPM;
    SerializedProperty BuildingsParent;
    SerializedProperty UnitsParent;
    SerializedProperty ResourcesParent;
    SerializedProperty PositionRing;
    SerializedProperty PositionPin;
    SerializedProperty PositionPinTop;
    SerializedProperty AboveTerrainTransform;
    SerializedProperty BelowTerrainTransform;

    SerializedProperty SnapToTerrain;
    SerializedProperty SnapWhileMoving;

    void OnEnable()
    {
        Instance = (PlayerStartLocation)target;

        ManualOverride = serializedObject.FindProperty("ManualOverride");
        RTSESCPM = serializedObject.FindProperty("RTSESCPM");
        BuildingsParent = serializedObject.FindProperty("BuildingsParent");
        UnitsParent = serializedObject.FindProperty("UnitsParent");
        ResourcesParent = serializedObject.FindProperty("ResourcesParent");
        PositionRing = serializedObject.FindProperty("PositionRing");
        PositionPin = serializedObject.FindProperty("PositionPin");
        PositionPinTop = serializedObject.FindProperty("PositionPinTop");

        AboveTerrainTransform = serializedObject.FindProperty("AboveTerrainTransform");
        BelowTerrainTransform = serializedObject.FindProperty("BelowTerrainTransform");

        SnapToTerrain = serializedObject.FindProperty("SnapToTerrain");
        SnapWhileMoving = serializedObject.FindProperty("SnapWhileMoving");

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.PropertyField(ManualOverride);
        EditorGUILayout.PropertyField(SnapToTerrain);
        EditorGUILayout.PropertyField(SnapWhileMoving);
        EditorGUILayout.EndVertical();

        if(Instance != null)
        {
            if(Instance.BuildingsParent != null)
            {
                GUILayout.Space(5);
                GUILayout.Label("--------------------");
                GUILayout.Space(5);
                if(Instance.BuildingsParent.childCount > 0)
                {
                    GUILayout.Label("Building Positions: " + Instance.BuildingsParent.childCount);
                    foreach(Transform child in this.Instance.BuildingsParent)
                    {
                        if(child != null)
                        {
                            if(child.TryGetComponent(out FactionBuildingMarker FBM))
                            {
                                GUILayout.Label("Building Position: " + FBM.gameObject.transform.position);
                                this.Instance.factionBuildingMarkers.Add(FBM);
                            }
                        }
                        else
                        {
                            DestroyImmediate(child.gameObject);
                        }
                    }
                }
                else
                {
                    GUILayout.Label("No Building Positions added");
                }
                if (GUILayout.Button("Place a Building Marker"))
                {
                    AddEntityPositionMarker("building");
                }
            }
            if (Instance.UnitsParent != null)
            {
                GUILayout.Space(5);
                GUILayout.Label("--------------------");
                GUILayout.Space(5);
                
                if (Instance.UnitsParent.childCount > 0)
                {
                    GUILayout.Label("Unit Positions: " + Instance.UnitsParent.childCount);
                    foreach (Transform child in this.Instance.UnitsParent)
                    {
                        if (child != null)
                        {
                            if (child.TryGetComponent(out FactionUnitMarker FUM))
                            {
                                GUILayout.Label("Unit Position: " + FUM.gameObject.transform.position);
                                this.Instance.factionUnitMarkers.Add(FUM);
                            }
                        }
                        else
                        {
                            DestroyImmediate(child.gameObject);
                        }
                    }
                }
                else
                {
                    GUILayout.Label("No Unit Positions added");
                }
                if (GUILayout.Button("Place a Unit Marker"))
                {
                    AddEntityPositionMarker("unit");
                }
            }
            if (Instance.ResourcesParent != null)
            {
                GUILayout.Space(5);
                GUILayout.Label("--------------------");
                GUILayout.Space(5);
                
                if (Instance.ResourcesParent.childCount > 0)
                {
                    GUILayout.Label("Resource Positions: " + Instance.ResourcesParent.childCount);
                    foreach (Transform child in this.Instance.ResourcesParent)
                    {
                        if (child != null)
                        {
                            if (child.TryGetComponent(out FactionResourceMarker FRM))
                            {
                                GUILayout.Label("Resource Position: " + FRM.gameObject.transform.position);
                                this.Instance.factionResourceMarkers.Add(FRM);
                            }
                        }
                        else
                        {
                            DestroyImmediate(child.gameObject);
                        }
                    }
                }
                else
                {
                    GUILayout.Label("No Resource Positions added");
                }
                if (GUILayout.Button("Place a Resource Marker"))
                {
                    AddEntityPositionMarker("resource");
                }
                GUILayout.Space(5);
                GUILayout.Label("--------------------");
                GUILayout.Space(5);
            }
        }

        if(ManualOverride.boolValue)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Manual Overrides");
            GUILayout.Label("Scene Prep Manager [==========>----------");
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(RTSESCPM);
            GUILayout.Label("Parent Entity Transforms [==========>----------");
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(BuildingsParent);
            EditorGUILayout.PropertyField(UnitsParent);
            EditorGUILayout.PropertyField(ResourcesParent);
            GUILayout.Space(5);
            GUILayout.Label("Position Marker Visual Elements [==========>----------");
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(PositionRing);
            EditorGUILayout.PropertyField(PositionPin);
            EditorGUILayout.PropertyField(PositionPinTop);
            GUILayout.Space(5);
            GUILayout.Label("Terrain Buffer Transforms [==========>----------");
            GUILayout.Space(5);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(AboveTerrainTransform);
            EditorGUILayout.PropertyField(BelowTerrainTransform);
            GUILayout.Space(5);
            GUILayout.Label("[==========>----------");
            GUILayout.Space(5);

            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void AddEntityPositionMarker(string typeIndex)
    {
        switch(typeIndex)
        {
            case "building":
                if (this.Instance.BuildingsParent != null)
                {
                    // Now the buildingIndexToSpawn has been automatically incremented :-) your welcome 
                    int buildingIndex = (Instance.BuildingsParent.childCount - 1)+1;
                    Instantiate(new GameObject("BuildingPosition"), this.Instance.BuildingsParent).AddComponent<FactionBuildingMarker>().buildingIndexToSpawn = buildingIndex++;
                    //Instantiate(new GameObject("BuildingPosition"), this.Instance.BuildingsParent).AddComponent<FactionBuildingMarker>();
                }
                break;
            case "unit":
                if (this.Instance.UnitsParent != null)
                {
                    int unitIndex = (Instance.UnitsParent.childCount - 1) + 1;
                    Instantiate(new GameObject("UnitPosition"), this.Instance.UnitsParent).AddComponent<FactionUnitMarker>().unitIndexToSpawn = unitIndex++;
                    //Instantiate(new GameObject("UnitPosition"), this.Instance.UnitsParent).AddComponent<FactionUnitMarker>();
                }
                break;
            case "resource":
                if (this.Instance.BuildingsParent != null)
                {
                    int resIndex = (Instance.ResourcesParent.childCount - 1) + 1;
                    Instantiate(new GameObject("ResourcePosition"), this.Instance.ResourcesParent).AddComponent<FactionResourceMarker>().ResourceIndex = resIndex++;
                    //Instantiate(new GameObject("ResourcePosition"), this.Instance.BuildingsParent).AddComponent<FactionResourceMarker>();
                }
                break;
        }
    }
}
