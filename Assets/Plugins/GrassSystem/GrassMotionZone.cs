using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GrassMotionZone : MonoBehaviour
{
	[SerializeField]
	private NormalizedAnimationCurve _intensityCurve = new NormalizedAnimationCurve();

	private float _birthTime;

	private Mesh _mesh;

	private Material _material;

    public bool isEnd = false;

	void Start()
	{
		_mesh = GetComponent<MeshFilter>().sharedMesh;
	}

	void OnEnable()
	{
		_birthTime = Time.time;

		GrassMotionRenderer.AddZone(this);
	}

	void OnDisable()
	{
        GrassMotionRenderer.RemoveZone(this);
        Kill();
    }

    void OnDestroy()
    {
        GrassMotionRenderer.RemoveZone(this);
    }

    public void Kill()
    {
        Destroy(_material);
        Destroy(this.gameObject);
    }
	public void Render(Material material)
	{
		if (_material == null)
		{
			_material = Instantiate(material);
			_material.CopyPropertiesFromMaterial(material);
		}

		_material.SetFloat("_Intensity", _intensityCurve.Evaluate(Time.time - _birthTime));
		_material.SetVector("_Origin", transform.position);

		if (_material.SetPass(0))
		{
			Graphics.DrawMeshNow(_mesh, transform.localToWorldMatrix);
		}

        
        if (Time.time - _birthTime > 1)
        {
            GrassMotionRenderer._zonesToDelete.Add(this);
        }
        
	}

}
