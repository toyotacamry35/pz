using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sebastian.Geometry;
using System;
using Assets.Src.Aspects.RegionsScenicObjects;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ShapeCreator : MonoBehaviour
{
    public Material DefaultMaterial;
    //[HideInInspector]
    public List<Shape> shapes = new List<Shape>();
    public Mesh ComputedMesh;
    [HideInInspector]
    public bool showShapesList;
    public bool DoNotRegen = false;
    public float handleRadius = .5f;
    public void UpdateMeshDisplay()
    {
        if (DoNotRegen)
            return;
        var pRegion = GetComponent<PolygonRegion>();
        if(pRegion)
            pRegion.GenerationVersion++;
        GetComponent<MeshRenderer>().material = DefaultMaterial;
        CompositeShape compShape = new CompositeShape(shapes);
        ComputedMesh = compShape.GetMesh();
        GetComponent<MeshFilter>().mesh = ComputedMesh;
    }
}