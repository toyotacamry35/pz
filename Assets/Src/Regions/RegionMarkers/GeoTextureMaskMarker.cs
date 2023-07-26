using Assets.ColonyShared.GeneratedCode.Regions;
using SharedCode.Aspects.Regions;
using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Regions.RegionMarkers
{
    [ExecuteInEditMode]
    public class GeoTextureMaskMarker : GeoRegionMarker
    {
        public Vector3 _size = new Vector3(1, 1, 1);
        public Texture2D _texture;
        public Material _quadMaterial;
        private Material _quadMaterialInstance;
        private static readonly string _quadMaterialPath = @"Assets/Src/Regions/RegionMarkers/GeoTextureRegionMaterial.mat";

        [Range(0.0f, 1.0f)]
        public float _displayedPosition = 1;

        public override BoundingBoxDef GetBoundingBox() => GeometryHelpers.GetBoundingBoxOfBoxWithTransform((SharedCode.Utils.Vector3)_size, (SharedCode.Entities.Transform)transform);

        public override GeoRegionDef GetGeoRegionDefinition()
        {
            if (_texture == default(Texture2D))
            {
                Debug.LogError($"No texture set in {gameObject.name}. Can't export regions.");
                return default(GeoRegionDef);
            }
            var textureBytes = _texture.GetPixels();
            var quotient = textureBytes.Length / 8;
            var remainder = textureBytes.Length % 8;
            var pixelArraySize = remainder > 0 ? quotient + 1 : quotient;
            byte[] pixelArray = new byte[pixelArraySize];
            for (int i = 0; i < textureBytes.Length; i += 8)
            {
                var currentPixelIndexInArray = i / 8;
                pixelArray[currentPixelIndexInArray] = 0;
                for (int j = 0; j < 8; j++)
                {
                    if (textureBytes[i + j].r < 0.5f)
                        pixelArray[currentPixelIndexInArray] = (byte)(pixelArray[currentPixelIndexInArray] | (byte)(1 << j));
                }
            }
            string base64EncodedPixelArray = Convert.ToBase64String(pixelArray).Compress();

            var def = new GeoTextureMaskDef()
            {
                TexHeight = _texture.height,
                TexWidth = _texture.width,
                TexData = base64EncodedPixelArray,
                Center = (SharedCode.Utils.Vector3)gameObject.transform.position,
                Extents = (SharedCode.Utils.Vector3)(_size / 2),
                InverseRotation = (SharedCode.Utils.Quaternion)Quaternion.Inverse(gameObject.transform.localRotation)
            };
            return def;
        }

        #region drawing

#if UNITY_EDITOR
        private void Awake()
        {
            if (_quadMaterial == null)
                _quadMaterial = AssetDatabase.LoadAssetAtPath<Material>(_quadMaterialPath);
        }

        private void OnDrawGizmosSelected()
        {
            var draw = DebugExtension.Draw;
            DebugExtension.Draw = true;
            DebugExtension.DebugLocalCube(gameObject.transform, _size, Color.green);
            DebugExtension.Draw = draw;

            if (_texture != default(Texture2D))
            {
                // I could use 3 rotations here instead of 4 but this code runs only in editor
                var center = transform.position;
                var rotation = transform.rotation;
                float height = (_displayedPosition - 0.5f) * _size.y;
                var coordLocal1 = new Vector3(-_size.x / 2, height, -_size.z / 2);
                var coordLocal2 = new Vector3(_size.x / 2, height, -_size.z / 2);
                var coordLocal3 = new Vector3(_size.x / 2, height, _size.z / 2);
                var coordLocal4 = new Vector3(-_size.x / 2, height, _size.z / 2);

                if (_quadMaterialInstance == null)
                    _quadMaterialInstance = CreateQuadMaterial(_texture, _quadMaterial);
                _quadMaterialInstance?.SetPass(0);
                GL.PushMatrix();
                //GL.LoadIdentity();
                GL.MultMatrix(transform.localToWorldMatrix);
                GL.Begin(GL.QUADS);
                GL.TexCoord2(0, 0);
                GL.Vertex(coordLocal1);
                GL.TexCoord2(0, 1);
                GL.Vertex(coordLocal4);
                GL.TexCoord2(1, 1);
                GL.Vertex(coordLocal3);
                GL.TexCoord2(1, 0);
                GL.Vertex(coordLocal2);
                GL.End();
                GL.PopMatrix();
            }
        }

        private static Material CreateQuadMaterial(Texture texture, Material baseMaterial)
        {

            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            //Shader shader = Shader.Find("Unlit/Transparent");
            var materialInstance = new Material(baseMaterial);
            materialInstance.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            materialInstance.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            materialInstance.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off 
            //lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            materialInstance.SetTexture("_MainTex", texture);
            //quadMaterial.SetInt("_ZWrite", 0);
            return materialInstance;
        }

        private void OnValidate()
        {
            if (_texture != null && _quadMaterialInstance?.GetTexture("_MainTex") != _texture)
                _quadMaterialInstance?.SetTexture("_MainTex", _texture);
        }
#endif
        #endregion drawing
    }
}
