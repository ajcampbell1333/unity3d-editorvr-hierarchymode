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

[ExecuteInEditMode]
public class HierarchyParent : MonoBehaviour {

    void OnEnable()
    {
      
        foreach (Transform child in transform)
        {
            LineRenderer lRend = child.gameObject.AddComponent<LineRenderer>() as LineRenderer;

            lRend.SetWidth(0.02f, 0.02f);
            
            Material blueDiffuseMat = new Material(Shader.Find("Legacy Shaders/Self-Illumin/Diffuse"));
            blueDiffuseMat.color = new Color(0, 50.0f, 255.0f);
            lRend.sharedMaterial = blueDiffuseMat;
        
            Vector3[] positions = new Vector3[2] { transform.position, child.position };
            lRend.SetPositions(positions);
        }
    }
    
    void Update()
    {
        if (transform.hasChanged)
            UpdateParentChildLines();      
     
        foreach (Transform child in transform)
            if (child.hasChanged)
                UpdateParentChildLines();
    }

    void UpdateParentChildLines()
    {
        foreach (Transform child in transform)
        {
            Vector3[] positions = new Vector3[2] { transform.position, child.position };
            GetChildLineRenderer(child).SetPositions(positions);
        }
    }

    LineRenderer GetChildLineRenderer(Transform child)
    {
        return child.gameObject.GetComponent<LineRenderer>();
    }

    void OnDestroy()
    {
        foreach (Transform child in transform)
        {
            LineRenderer lRend = GetChildLineRenderer(child);
            if (lRend != null)
                DestroyImmediate(lRend);
        }
    }
}
