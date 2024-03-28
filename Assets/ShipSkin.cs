using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipSkin : MonoBehaviour {
    [SerializeField] private GameObject skin;

    public void SetAlpha(float alpha) {
        if (skin == null) throw new Exception("`GameObject skin` property is null.");

        foreach (var mat in skin.GetComponent<MeshRenderer>().materials) {
            Color old_color = mat.color;
            Color new_color = new Color(old_color.r, old_color.g, old_color.b, alpha);         
            mat.SetColor("_Color", new_color);  
        }
    }
    
    public void SetMaterial(Material mat) {
        if (skin == null) throw new Exception("`GameObject skin` property is null.");
    
        MeshRenderer renderer = skin.GetComponent<MeshRenderer>();
        if (renderer == null) throw new Exception("`MeshRenderer` component not found on `skin` GameObject.");
    
        // Use LINQ to create and fill the array
        renderer.materials = Enumerable.Repeat(mat, renderer.materials.Length).ToArray();
    }
}