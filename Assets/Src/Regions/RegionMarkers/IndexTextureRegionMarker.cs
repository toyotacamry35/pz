using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourceSystem;
using Assets.Src.SpatialSystem;
using Assets.Src.SpatialSystem.Editor;
using GeneratedCode.Custom.Config;
using ResourcesSystem.Loader;
using SharedCode.Aspects.Regions;
using SharedCode.Shared;
using Uins;
using UnityEngine;
using Utilities;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;

#endif

namespace Assets.Src.Regions.RegionMarkers
{
    [ExecuteInEditMode]
    public class IndexTextureRegionMarker : RegionMarker
    {
        [SerializeField]
        private Vector3 Size = new Vector3(1, 1, 1);

        [SerializeField]
        private Texture2D Texture;

        [SerializeField]
        private Material QuadMaterial;

        [SerializeField]
        private Material RegionsMaterial;

        [SerializeField]
        private int GridCellsCount = 22;

        [SerializeField]
        private float GridThickness = 2;

        [SerializeField]
        private Color GridColor = new Color32(212, 164, 117, 255);

        [SerializeField]
        private Color GridFogColor = new Color32(40, 40, 40, 255);

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float DisplayedPosition = 1;

        [SerializeField]
        private JdbMetadata MapDef;

        private SVO IndexedBitmap { get; set; }
        public Dictionary<Color32, short> IndexedColors { get; private set; }
        
        private void UpdateIndexes()
        {
            if (Texture != null)
            {
                var pixels = Texture.GetPixels32();
                IndexedColors = SVOExporter.CreateIndexes<Color32, short>(pixels);
                IndexedBitmap = SVOExporter.CreateSVO(pixels, Texture.width, Texture.height, IndexedColors);
            }
            else
            {
                IndexedColors = null;
                IndexedBitmap = null;
            }
        }

        public Color32 GetColorAt(Vector3 position)
        {
            if (Texture != null)
            {
                var objectTransform = gameObject.transform;
                if (GeometryHelpers.PointToRectPos(
                    (SharedCode.Utils.Vector3) position,
                    (SharedCode.Utils.Vector3) objectTransform.position,
                    (SharedCode.Utils.Vector3) (Size / 2),
                    (SharedCode.Utils.Quaternion) Quaternion.Inverse(objectTransform.rotation),
                    Texture.width,
                    Texture.height,
                    out var x,
                    out var y))
                    return Texture.GetPixel(x, y);
            }

            return default;
        }

        public Vector3 GetPositionOf(Color32 color)
        {
            if (Texture == null)
                return default;

            var colors = Texture.GetPixels32();
            var colorsLength = colors.Length;
            var textureWidth = Texture.width;

            var index = colors.IndexOf(color);
            var hashSet = new HashSet<int> {index};
            for (var j = index + 1; j < colorsLength; j++)
                if (colors[j].Equals(color))
                    hashSet.Add(j);

            var centerX = 0;
            var centerY = 0;
            foreach (var i in hashSet)
            {
                var y = Math.DivRem(i, textureWidth, out var x);
                centerX += x;
                centerY += y;
            }

            centerX /= hashSet.Count;
            centerY /= hashSet.Count;

            if (!hashSet.Contains(centerX + centerY * textureWidth))
                (centerX, centerY) = hashSet.Select(
                        i =>
                        {
                            var y = Math.DivRem(i, textureWidth, out var x);
                            return (x, y);
                        })
                    .OrderBy(
                        tuple => Math.Sqrt(Math.Pow(tuple.x - centerX, 2) + Math.Pow(tuple.y - centerY, 2))
                    )
                    .First();

            var objectTransform = gameObject.transform;
            if (GeometryHelpers.RectPosToPoint(
                out var position,
                (SharedCode.Utils.Vector3) objectTransform.position,
                (SharedCode.Utils.Vector3) (Size / 2),
                (SharedCode.Utils.Quaternion) Quaternion.Inverse(objectTransform.rotation),
                Texture.width,
                Texture.height,
                centerX,
                centerY
            ))
                return (Vector3) position;

            return default;
        }

        public override ARegionDef BuildDefs()
        {
            // ExportIndexR16();

            var regDef = GetRegionDefinition();
            var childRegions = GetChildMarkers();
            if (childRegions.Count == 0)
                throw new Exception($"{nameof(GameObject)} {name} Has No Children");

            regDef.ChildRegions = childRegions.Select(
                    childReg => new ResourceRef<ARegionDef>(childReg.BuildDefs())
                )
                .ToArray();

            regDef.Data = GetRegionData();
            return regDef;
        }

        private IndexTextureRegionDef GetRegionDefinition()
        {
            if (Texture == default(Texture2D))
            {
                Debug.LogError($"No texture set in {gameObject.name}. Can't export regions.");
                return default;
            }

            UpdateIndexes();

            var mapDef = new MapDef();
            var mapDefResource = (IResource) mapDef;
            mapDefResource.Address = new ResourceIDFull(MapDef.RootPath);

            var binaryResource = ExportSVOResource();

            var o = gameObject;
            var def = new IndexTextureRegionDef
            {
                Center = (SharedCode.Utils.Vector3) o.transform.position,
                Extents = (SharedCode.Utils.Vector3) (Size / 2),
                InverseRotation = (SharedCode.Utils.Quaternion) Quaternion.Inverse(o.transform.rotation),
                IndexWidth = Texture.width,
                IndexHeight = Texture.height,
                TexData = binaryResource,
                MapDef = mapDef
            };
            return def;
        }


#if UNITY_EDITOR
        // private void ExportIndexR16()
        // {
        //     var r16 = SVOExporter.ExportIndexTextureR16(Pixels32, Texture.width, Texture.height, IndexedColors);
        //     var path = Path.ChangeExtension(AssetDatabase.GetAssetPath(Texture), ".index.bytes");
        //     File.WriteAllBytes(path, r16.GetRawTextureData());
        //     AssetDatabase.ImportAsset(path);
        // }

        private BinaryResource<SVO> ExportSVOResource()
        {
            if (IndexedBitmap == null)
            {
                Debug.LogError($"No indexed Bitmap Found In {gameObject.name}");
                return default;
            }

            var binaryResource = new BinaryResource<SVO>(IndexedBitmap);
            var assetPath = AssetDatabase.GetAssetPath(Texture);
            GameResources.SimpleSave(
                GameResourcesHolder.Instance.Deserializer.Loader.GetRoot(),
                $"{FolderLoader.CleanUpRelativePath(assetPath)}_svo",
                binaryResource,
                out var resultPath
            );
            var relativePath = "Assets/" + PathHelper.GetRelativePath(Application.dataPath, resultPath);
            AssetDatabase.ImportAsset(relativePath);
            Debug.Log($"Index Texture SVO Saved: {relativePath}");
            return binaryResource;
        }

        #region drawing

        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private Material _regionsMaterialInstance;

        private Material RegionsMaterialInstance
        {
            get
            {
                if (_regionsMaterialInstance == null)
                    _regionsMaterialInstance = new Material(RegionsMaterial)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };

                return _regionsMaterialInstance;
            }
        }

        private Material _quadMaterialInstance;

        private Material QuadMaterialInstance
        {
            get
            {
                if (_quadMaterialInstance == null)
                {
                    _quadMaterialInstance = new Material(QuadMaterial)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                    // Turn on alpha blending
                    _quadMaterialInstance.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                    _quadMaterialInstance.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    // Turn off depth writes
                    _quadMaterialInstance.SetTexture(MainTex, Texture);
                }

                return _quadMaterialInstance;
            }
        }

        private Hash128 _imageContentsHash;
        private int[] _allVisibleIndexesDataArray;
        private int[] _tempVisibleIndexesDataArray;
        private ComputeBuffer _compBuf;
        private Texture2D _fogIndexTexture;
        private RenderTexture _fogRenderTexture;
        private RenderTexture _mapRenderTexture;
        private Texture _sourceTexture;
        private SVO _sourceIndexedBitmap;

        private void OnEnable()
        {
            UpdateIndexes();
            if (!EditorApplication.isPlaying)
                EditorApplication.update += UpdateTextureContent;
        }

        private void OnDisable()
        {
            ReleaseRenderTextures();
            _compBuf?.Release();
            _compBuf = null;
            _sourceTexture = null;
            _sourceIndexedBitmap = null;
            // ReSharper disable once DelegateSubtraction
            if (!EditorApplication.isPlaying)
                EditorApplication.update -= UpdateTextureContent;
        }


        private void OnDrawGizmos()
        {
            InitGizmos();

            if (_fogRenderTexture != null && _mapRenderTexture != null && _fogIndexTexture != null)
            {
                var temp = RenderTexture.active;
                _compBuf.SetData(GetVisibleIndexes());
                RegionsMaterialInstance.SetBuffer(FogOfWarShader.BufferId, _compBuf);

                RenderTexture.active = _fogRenderTexture;
                GL.Clear(true, true, Color.clear);
                Graphics.Blit(_fogIndexTexture, _fogRenderTexture, RegionsMaterialInstance, 0);
                RegionsMaterialInstance.SetFloat(FogOfWarShader.GridThicknessId, GridThickness);
                RegionsMaterialInstance.SetInt(FogOfWarShader.GridCellsCountId, GridCellsCount);
                RegionsMaterialInstance.SetColor(FogOfWarShader.GridColorId, GridColor);
                RegionsMaterialInstance.SetColor(FogOfWarShader.GridFogColorId, GridFogColor);
                RegionsMaterialInstance.SetTexture(FogOfWarShader.FogTextureId, _fogRenderTexture);
                Graphics.Blit(Texture, _mapRenderTexture, RegionsMaterialInstance, 1);

                RenderTexture.active = temp;

                QuadMaterialInstance.mainTexture = _mapRenderTexture;
            }
            else
            {
                QuadMaterialInstance.mainTexture = Texture;
            }

            var draw = DebugExtension.Draw;
            DebugExtension.Draw = true;
            DebugExtension.DebugLocalCube(gameObject.transform, Size, Color.green);
            DebugExtension.Draw = draw;

            var height = (DisplayedPosition - 0.5f) * Size.y;
            var coordLocal1 = new Vector3(-Size.x / 2, height, -Size.z / 2);
            var coordLocal2 = new Vector3(Size.x / 2, height, -Size.z / 2);
            var coordLocal3 = new Vector3(Size.x / 2, height, Size.z / 2);
            var coordLocal4 = new Vector3(-Size.x / 2, height, Size.z / 2);

            QuadMaterialInstance.SetPass(0);
            GL.PushMatrix();
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

        private int[] GetVisibleIndexes()
        {
            var selected = Selection.activeGameObject;
            if (selected != null)
            {
                var parent = selected.transform;
                while (parent != null && parent != transform) 
                    parent = parent.parent;
                if (parent == transform)
                {
                    var group = selected.GetComponent<IndexedRegionGroupMarker>();
                    if (group != null)
                    {
                        var indexes = group
                            .GetComponentsInChildren<IndexedRegionMarker>()
                            .Where(marker => IndexedColors.ContainsKey(marker.Color))
                            .Select(marker => IndexedColors[marker.Color]);
                        for (var index = 0; index < _tempVisibleIndexesDataArray.Length; index++)
                            _tempVisibleIndexesDataArray[index] = 0;
                        foreach (var index in indexes) _tempVisibleIndexesDataArray[index] = 1;
                        return _tempVisibleIndexesDataArray;
                    }

                    var region = selected.GetComponent<IndexedRegionMarker>();
                    if (region != null)
                    {
                        for (var index = 0; index < _tempVisibleIndexesDataArray.Length; index++)
                            _tempVisibleIndexesDataArray[index] = 0;
                        if (IndexedColors.TryGetValue(region.Color, out var i))
                            _tempVisibleIndexesDataArray[i] = 1;
                        return _tempVisibleIndexesDataArray;
                    }
                }
            }

            return _allVisibleIndexesDataArray;
        }

        private void InitGizmos()
        {
            _allVisibleIndexesDataArray = _allVisibleIndexesDataArray ?? Enumerable.Repeat(1, FogOfWarShader.MaxIndexValue + 1).ToArray();
            _tempVisibleIndexesDataArray = _tempVisibleIndexesDataArray ?? Enumerable.Repeat(0, FogOfWarShader.MaxIndexValue + 1).ToArray();
            _compBuf = _compBuf ?? new ComputeBuffer(FogOfWarShader.MaxIndexValue + 1, sizeof(int), ComputeBufferType.Default);

            if (IndexedBitmap != null && _sourceIndexedBitmap != IndexedBitmap)
            {
                _sourceIndexedBitmap = IndexedBitmap;
                _fogIndexTexture = FogOfWarShader.CreateIndexTexture(
                    IndexedBitmap.Width,
                    IndexedBitmap.Height,
                    IndexedBitmap.GetIndexBlocks()
                );
            }

            if (Texture != null && _sourceTexture != Texture)
            {
                _sourceTexture = Texture;

                ReleaseRenderTextures();
                _fogRenderTexture = new RenderTexture(
                    Texture.width,
                    Texture.height,
                    0,
                    RenderTextureFormat.RHalf,
                    RenderTextureReadWrite.Linear
                );

                _mapRenderTexture = new RenderTexture(
                    Texture.width,
                    Texture.height,
                    0,
                    RenderTextureFormat.ARGBHalf,
                    0
                )
                {
                    antiAliasing = 1,
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Point
                };
            }
        }

        private void ReleaseRenderTextures()
        {
            if (_fogRenderTexture != null)
                _fogRenderTexture.Release();
            if (_mapRenderTexture != null)
                _mapRenderTexture.Release();
        }

        private void UpdateTextureContent()
        {
            if (Texture == null || Texture.imageContentsHash != _imageContentsHash)
            {
                _imageContentsHash = Texture ? Texture.imageContentsHash : default;
                UpdateIndexes();
                if (!EditorApplication.isPlaying)
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        #endregion drawing

#else
        private BinaryResource<SVO> ExportSVOResource()
        {
            return null;
        }

        private void ExportIndexR16()
        {
        }
#endif
    }
}