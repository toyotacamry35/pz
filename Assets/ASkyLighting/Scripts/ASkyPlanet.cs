using UnityEngine;
using System.Collections;
using TOD;

public class ASkyPlanet : MonoBehaviour
{
    [HideInInspector]
    public bool atmosphere = true;

    [HideInInspector]
    [ColorUsage(false, true)]
    public Color atmoSphereColor = new Color(0, 0, 0);
    [HideInInspector]
    public Vector3 sunDirection;

    public GameObject planet;
    public MeshRenderer planetRenderer;
    public Material sourceMaterial;
    
    private Material instanceMaterial;
    
    [HideInInspector]
    public CelestialMechanics.CelestialOrbit celestialOrbit;
    [HideInInspector]
    public CelestialMechanics.CelestialRotation celestialRotation;
    

    public float dist;
    [HideInInspector]
    public float movementTime;
    [HideInInspector]
    public float rotationTime;
    [HideInInspector]
    public 	float atmoStrength = 10.0f;
    [HideInInspector]
    [Range(0.5f, 4f)]
    public float atmoColorCoeff = 1.0f;
    [HideInInspector]
    public float kr = 0.0025f;
    [HideInInspector]
    public float km = 0.0010f;
    [HideInInspector]
    public float outerScaleFactor = 1.025f;
    [HideInInspector]
    public float innerRadius;
    [HideInInspector]
    public float outerRadius;
    [HideInInspector]
    public float scaleDepth = 0.25f;
    [HideInInspector]
    public float scale;
   [HideInInspector]
    public float atmoThickness = 1.0f;
   [HideInInspector]
    public float lightingOffset = 0.2f;

  
    public void Calculate()
    {
        celestialOrbit.UpdateSimulation();
        celestialRotation.UpdateSimulation();       
    }

    public void UpdateMaterial()
    {
        if (planetRenderer != null)
        {
            UpdateMaterial(planetRenderer.material);
            InitMaterial(planetRenderer.material);
        }
    }

    private void Update()
    {
        if (Application.isPlaying && ASkyLighting.isActiveOnClient)
        UpdateMaterial();
    }

    [ContextMenu("CreateMaterials")]
    public void CreateMaterials()
    {
        if (!Application.isPlaying)
            return;

        if (planetRenderer)
        {
            if (instanceMaterial==null)
                instanceMaterial = new Material(sourceMaterial);
            planetRenderer.material = instanceMaterial;
            InitMaterial(planetRenderer.material);
        }

    }

    public void UpdateMaterial(Material mat)
    {//
        mat.SetFloat("_AtmoThickness", atmoThickness);
        mat.SetVector("_SunDirection", TOD.ASkyLighting._instance.SunDirection);
        mat.SetColor("_SunColor", atmoSphereColor.linear);
    }

	public void InitMaterial(Material mat)
	{
        innerRadius = planet.transform.localScale.x;
        outerRadius = outerScaleFactor * planet.transform.localScale.x;
        scale = 1.0f / (outerRadius - innerRadius);

        mat.EnableKeyword(atmosphere == true ? "ATMO_ON" : "ATMO_OFF");
        float invWavelength = 1.0f / Mathf.Pow(1 - Color.gray.linear.r * atmoColorCoeff, 4);
        mat.SetFloat("_AtmoThickness", atmoThickness);
        mat.SetFloat("_Magnitude", planet.GetComponent<Renderer>().bounds.max.magnitude/2);
        
        mat.SetFloat("fOuterRadius", outerRadius);
        mat.SetFloat("fInnerRadius", innerRadius);
        mat.SetFloat("invWavelength", invWavelength);

        mat.SetFloat("fKr4PI", kr*4.0f*Mathf.PI);
        mat.SetFloat("fKm4PI", km*4.0f*Mathf.PI);
        mat.SetFloat("fKrESun", kr * atmoStrength);
        mat.SetFloat("fKmESun", km * atmoStrength);

        mat.SetFloat("fScale", scale);
        mat.SetFloat("fScaleDepth", scaleDepth);
        mat.SetFloat("fScaleOverScaleDepth", scale/scaleDepth);
        mat.SetFloat("_DistanceBlending", Mathf.Lerp(0.0001f, 0.01f, TOD.ASkyLighting.shadowSmooth));
        mat.SetFloat("_HighlightOffset", Mathf.Lerp(0.005f, 0.1f, TOD.ASkyLighting.highlightOffset));

	}
}

