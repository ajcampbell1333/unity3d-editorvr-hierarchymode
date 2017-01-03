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

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Experimental.EditorVR.Menus;

public class HierarchyModeMenu : MonoBehaviour, IMenu
{
    public bool visible { get { return gameObject.activeSelf; } set { gameObject.SetActive(value); } }

    public GameObject menuContent { get { return gameObject; } }

    string childName;

    public void SetDefaultState(string defaultDisplayText)
    {
        childName = defaultDisplayText;
        transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = childName;
    }

    public void SetChildName(GameObject child)
    {
        childName = child.name + " Child(" + child.transform.GetSiblingIndex() + ")";
        transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = childName;
    }

}
