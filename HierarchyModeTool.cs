/*
 Copyright (C) 2016  Ammon J. Campbell

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    For a copy of the GNU General Public License, see http://www.gnu.org/licenses/.
 */

using System;
using UnityEngine;
using UnityEngine.InputNew;
using UnityEngine.Experimental.EditorVR.Menus;
using UnityEngine.Experimental.EditorVR.Tools;
using UnityEditor;
using UnityEngine.Experimental.EditorVR.Utilities;

[MainMenuItem("HierarchyMode", "Test", "3D Hierarchy for those who don't like lists")]
public class HierarchyModeTool : MonoBehaviour, ITool, IStandardActionMap, IUsesSpatialHash, IExclusiveMode, IUsesRayOrigin, IUsesRaycastResults, ISetHighlight, IConnectInterfaces, IInstantiateMenuUI
{
    [SerializeField]
    HierarchyModeMenu m_DisplayPrefab;

    GameObject m_CurrentParent;
    GameObject m_CurrentHover;
    GameObject m_DisplayMenu;
    
    public Transform rayOrigin { get; set; }
    
    public Action<GameObject> addToSpatialHash { get; set; }
    public Action<GameObject> removeFromSpatialHash { get; set; }

    public Func<Transform, GameObject> getFirstGameObject { private get; set; }

    public ConnectInterfacesDelegate connectInterfaces { private get; set; }

    public Func<Transform, IMenu, GameObject> instantiateMenuUI { private get; set; }

    public Action<GameObject, bool> setHighlight { private get; set; }

    public event Action<GameObject, Transform> hovered;
    public event Action<Transform> selected;
    
    void Start()
    {
        if (Selection.activeGameObject != null)
            m_CurrentParent = Selection.activeGameObject;

        m_DisplayMenu = instantiateMenuUI(rayOrigin, m_DisplayPrefab);
        var createDisplayMenu = m_DisplayMenu.GetComponent<HierarchyModeMenu>();
        connectInterfaces(createDisplayMenu, rayOrigin);

        string state = (m_CurrentParent != null) ? "Hover over child" : "Select an object";
        createDisplayMenu.SetDefaultState(state);
    }
    
    public void ProcessInput(ActionMapInput input, Action<InputControl> consumeControl)
    {
        var standardInput = (Standard)input;
        
        if (Selection.activeGameObject != null)
            if (FindObjectOfType<HierarchyParent>() == null)
                Selection.activeGameObject.AddComponent<HierarchyParent>();

        HandleHover();
        HandleObjectSelection(standardInput, consumeControl);
    }

    void HandleHover()
    {
        var newHoverGameObject = getFirstGameObject(rayOrigin);
        HierarchyModeMenu display = m_DisplayMenu.GetComponent<HierarchyModeMenu>();

        if (newHoverGameObject != m_CurrentHover)
        {
            if (m_CurrentHover != null)
            {
                setHighlight(m_CurrentHover, false);
                m_DisplayMenu.GetComponent<HierarchyModeMenu>().SetDefaultState("Hover over child");
            }

            if (newHoverGameObject != null)
            {
                setHighlight(newHoverGameObject, true);
                if (m_CurrentParent != null && newHoverGameObject.transform.parent == m_CurrentParent.transform)
                    display.SetChildName(newHoverGameObject);
                else if (m_CurrentParent != null && newHoverGameObject.transform == m_CurrentParent.transform)
                    display.SetDefaultState("Hover over child");
                else {
                    if (m_CurrentParent != null)
                        display.SetDefaultState("Trigger to Make Child");
                    else display.SetDefaultState("Select an object");
                } 
            }
            else if (Selection.activeGameObject == null)
                display.SetDefaultState("Select an object");
                
        }
        m_CurrentHover = newHoverGameObject;

        
    }

    void HandleObjectSelection(Standard input, Action<InputControl> consumeControl)
    {
        if (input.action.wasJustPressed)
        {
            if (m_CurrentHover != null)
            {
                if (m_CurrentParent == null)
                {
                    Selection.activeGameObject = m_CurrentHover;
                    DestroyImmediate(GameObject.FindObjectOfType(typeof(HierarchyParent)));
                    m_CurrentParent = m_CurrentHover;
                    if (FindObjectOfType<HierarchyParent>() == null)
                        Selection.activeGameObject.AddComponent<HierarchyParent>();
                }
                else if (m_CurrentHover != m_CurrentParent)
                {
                    DestroyImmediate(GameObject.FindObjectOfType(typeof(HierarchyParent)));
                    if (m_CurrentHover.transform.IsChildOf(m_CurrentParent.transform))
                    {
                        // Break child link
                        m_CurrentHover.transform.SetParent(null);
                    }
                    else
                    {
                        // Link as child to current selection
                        m_CurrentHover.transform.SetParent(m_CurrentParent.transform);
                    }
                    if (FindObjectOfType<HierarchyParent>() == null)
                        Selection.activeGameObject.AddComponent<HierarchyParent>();
                    m_DisplayMenu.GetComponent<HierarchyModeMenu>().SetChildName(m_CurrentHover);
                }
            }
            else
            {
                Selection.activeGameObject = null;
                m_CurrentParent = null;
                DestroyImmediate(GameObject.FindObjectOfType(typeof(HierarchyParent)));
            }
            consumeControl(input.action);
        }
    }

    void CheckForTriggerRelease(Standard standardInput, Action<InputControl> consumeControl)
    {
        if (standardInput.action.wasJustReleased)
        {
            consumeControl(standardInput.action);
        }
    }

    void OnDestroy()
    {
        U.Object.Destroy(m_DisplayMenu);
    }
}

