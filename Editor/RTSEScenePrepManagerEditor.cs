using NSubstitute.Exceptions;
using RTSEngine.Faction;
using RTSEngine.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(RTSEScenePrepManager))]
public class RTSEScenePrepManagerEditor : Editor
{
    private RTSEScenePrepManager Instance;

    SerializedProperty PlayerStartPointPrefab;
    SerializedProperty PlayerPositionsParent;
    SerializedProperty PlayerStartLocations;

    SerializedProperty TerrainsParent;

    SerializedProperty FactionTypeAdder;

    SerializedProperty AvailableFactions;

    void OnEnable()
    {
        Instance = (RTSEScenePrepManager)target;

        PlayerStartPointPrefab = serializedObject.FindProperty("PlayerStartPointPrefab");
        PlayerPositionsParent = serializedObject.FindProperty("PlayerPositionsParent");
        PlayerStartLocations = serializedObject.FindProperty("PlayerStartLocations");

        TerrainsParent = serializedObject.FindProperty("TerrainsParent");

        FactionTypeAdder = serializedObject.FindProperty("FactionTypeAdder");
        AvailableFactions = serializedObject.FindProperty("AvailableFactions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        string[] topRow = {
            "Configure",
            "Players Setup",
        };
        Instance.TabID = GUILayout.SelectionGrid(Instance.TabID, topRow, 2);

        switch (Instance.TabID)
        {
            case 0:
                EditorGUILayout.BeginVertical("box");
                if(this.Instance.PlayerStartPointPrefab != null)
                {
                    EditorGUILayout.PropertyField(FactionTypeAdder);
                }
                if (GUILayout.Button("Add Selected Faction to this map") && this.Instance.PlayerStartPointPrefab != null)
                {
                    //GUILayout.Label("");
                    FetchFactionSlotData();
                }
                GUILayout.Space(5);
                GUILayout.Label("Faction Settings [==========>----------");
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(AvailableFactions);
                GUILayout.Space(5);
                GUILayout.Label("Terrain Settings [==========>----------");
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(TerrainsParent);
                GUILayout.Space(5);
                EditorGUILayout.EndVertical();
                break;
            case 1:
                EditorGUILayout.BeginVertical("box");
                GUILayout.Label("Players Setup");
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(PlayerPositionsParent);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(PlayerStartPointPrefab);
                EditorGUILayout.PropertyField(PlayerStartLocations);
                if(GUILayout.Button("Add Player Postion"))
                {
                    AddPlayerMarkerToMap();
                }
                EditorGUILayout.EndVertical();
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if(!EditorApplication.isPlaying && this.Instance.IsPlacingPlayerStart)
        {
            Event current = Event.current;
            if ((current.type == EventType.KeyDown && KeyCode.LeftControl == current.keyCode) && (current.type == EventType.MouseDown && current.button == 0))
            {
                Ray newPosRay = HandleUtility.GUIPointToWorldRay(current.mousePosition);
                if(Physics.Raycast(newPosRay, out RaycastHit hit, Mathf.Infinity, LayerMask.NameToLayer("GroundTerrain")))
                {
                    Instantiate(Instance.PlayerStartPointPrefab,hit.point, Quaternion.identity, this.Instance.PlayerPositionsParent);
                }
            }
        }
    }

    public void FetchFactionSlotData()
    {
        AvailableFaction availableFaction = this.Instance.AvailableFactions.Find(af => af.FactionType == this.Instance.FactionTypeAdder);
        if(availableFaction == null)
        {
            this.Instance.AvailableFactions.Add(
                new AvailableFaction(
                    this.Instance.FactionTypeAdder,
                    new(),
                    new()
                )
            );
        }
    }

    public void AddPlayerMarkerToMap()
    {
        if(this.Instance.PlayerPositionsParent != null && this.Instance.PlayerStartPointPrefab != null)
        {
            PlayerStartLocation newLocation = GameObject.Instantiate(this.Instance.PlayerStartPointPrefab, this.Instance.PlayerPositionsParent).GetComponent<PlayerStartLocation>();
            newLocation.RTSESCPM = this.Instance;
            this.Instance.PlayerStartLocations.Add(newLocation);
        }
    }
}
