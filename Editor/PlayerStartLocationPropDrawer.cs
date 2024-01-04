using RTSEngine.Determinism;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PlayerStartLocation))]
public class PlayerStartLocationPropDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.labelWidth = 0.1f;

        var elementTitleRect = new Rect(position.x,position.y,150,20);
        var currentPositionRect = new Rect(position.x,position.y + 20,180,20);

        var addPlayerRect = new Rect(position.x + 230, position.y, 40, 40);
        var addBuildingRect = new Rect(position.x + 270, position.y, 40, 40);
        var addUnitRect = new Rect(position.x + 310, position.y, 40, 40);

        int index = -1;
        Regex regex = new Regex(@"\[([0-9]+)\]");
        if(regex.IsMatch(property.propertyPath)){
            Match match = regex.Match(property.propertyPath);
            index = int.Parse(match.Groups[1].Value);
            GUI.Label(elementTitleRect, "Player Position: " + index);
        }
        else
        {
            GUI.Label(elementTitleRect, "Player Position: No Index ");
        }
        GameObject parent = GameObject.Find("PlayerStartPos");
        if (parent != null && parent.transform.childCount > 0 && index < parent.transform.childCount) {
            if (index != -1 && parent.transform.GetChild(index) != null){
                    GUI.Label(currentPositionRect, "Position: " + parent.transform.GetChild(index).position);
             } else {
                    GUI.Label(currentPositionRect, "Position marker not set");
             }
        }      

        Texture2D markerIcon = EditorGUIUtility.Load("Assets/RTS Engine/Modules/RTSE_ScenePreparationManager/Textures/player-marker-icon.png") as Texture2D;
        Texture2D buildingIcon = EditorGUIUtility.Load("Assets/RTS Engine/Modules/RTSE_ScenePreparationManager/Textures/building-icon.png") as Texture2D;
        Texture2D unitIcon = EditorGUIUtility.Load("Assets/RTS Engine/Modules/RTSE_ScenePreparationManager/Textures/unit-icon.png") as Texture2D;

        EditorGUIUtility.labelWidth = 0;

       // GameObject parent = GameObject.Find("PlayerStartPositions");
        

        if (GUI.Button(addPlayerRect, markerIcon))
        {
            //Debug.Log("element at: "+ property.GetIndexInArray());
        }
        if (GUI.Button(addBuildingRect, buildingIcon))
        {
            
        }
        if (GUI.Button(addUnitRect, unitIcon))
        {
            
        }
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();

    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int totalLine = 3;
        return EditorGUIUtility.singleLineHeight * totalLine + EditorGUIUtility.standardVerticalSpacing * (totalLine);
    }
}
