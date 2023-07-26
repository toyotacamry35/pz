using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.GeneratedCode.Regions;
using Core.Cheats;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityEngine.UI;
using DisposableComposite = ReactivePropsNs.DisposableComposite;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace Uins
{
    [RequireComponent(typeof(RawImage))]
    public class FogOfWarView : MonoBehaviour
    {
        //BlurShader
        private static readonly int BlurDirectionId = Shader.PropertyToID("_Direction");
        private static readonly int BlurResolutionId = Shader.PropertyToID("_InverseResolution");

        private static readonly Vector2Int BlurResolution = new Vector2Int(256, 256);

        private const float RegionsUpdatePeriodSeconds = 0.3f;

        // ReSharper disable once InconsistentNaming
        private readonly DisposableComposite D = new DisposableComposite();

        [SerializeField]
        private Texture DefaultTexture;

        [SerializeField]
        private Material Material;

        [SerializeField]
        private Material BlurMaterial;

        [SerializeField]
        private int GridCellsCount;

        [SerializeField]
        private float GridThickness;

        [SerializeField]
        private Color GridColor;

        [SerializeField]
        private Color GridFogColor;

        private ComputeBuffer _compBuf;
        private RawImage _rawImage;
        private Texture2D _fogIndexTexture;
        private IIndexRegion _indexRegion;

        private Texture _mapTexture;
        private RenderTexture _mapRenderTexture;
        private RenderTexture _fogRenderTexture;

        private readonly Dictionary<IndexedRegion, bool> _queuedIndexedRegions = new Dictionary<IndexedRegion, bool>();
        private bool _updateQueued;
        private IIndexRegion _newIndexRegion;
        private int[] _indexesDataArray;
        private float _mapZoomRatio;
        private bool _fogEnabled = true;
        private int[] _fogDisabledIndexesDataArray;

        [UsedImplicitly]
        public float MapZoomRatio
        {
            get => _mapZoomRatio;
            set
            {
                _mapZoomRatio = value;
                UpdateMapTexture();
            }
        }

        [UsedImplicitly]
        public bool FogEnabled
        {
            get => _fogEnabled;
            set
            {
                if (_fogEnabled != value)
                {
                    _fogEnabled = value;
                    UpdateTexture();
                }
            }
        }

        private void Awake()
        {
            _rawImage = GetComponent<RawImage>();
            _mapTexture = _rawImage.texture;
            _mapRenderTexture = new RenderTexture(_mapTexture.width, _mapTexture.height, 0, RenderTextureFormat.ARGBHalf, 0)
            {
                antiAliasing = 1,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point
            };
            _fogRenderTexture = new RenderTexture(
                BlurResolution.x,
                BlurResolution.y,
                0,
                RenderTextureFormat.RHalf,
                RenderTextureReadWrite.Linear);
            _rawImage.texture = DefaultTexture;
            Material = new Material(Material) {hideFlags = HideFlags.HideAndDontSave};
            BlurMaterial = new Material(BlurMaterial) {hideFlags = HideFlags.HideAndDontSave};

            _compBuf = new ComputeBuffer(FogOfWarShader.MaxIndexValue + 1, sizeof(int), ComputeBufferType.Default);
            _indexesDataArray = new int[FogOfWarShader.MaxIndexValue + 1];
        }

        public void Init(MapFogOfWarVM mapFogOfWarVM)
        {
            var timer = TimeTicker.Instance.GetUtcTimer(RegionsUpdatePeriodSeconds);
            timer.Action(
                D,
                tuple =>
                {
                    if (_updateQueued)
                    {
                        _updateQueued = false;
                        UpdateTexture();
                    }
                });

            mapFogOfWarVM.IndexRegion.Action(
                D,
                indexRegion =>
                {
                    _newIndexRegion = indexRegion;
                    _updateQueued = true;
                });
            mapFogOfWarVM.Discovered.AddStream.Action(
                D,
                s =>
                {
                    _queuedIndexedRegions[s] = true;
                    _updateQueued = true;
                });
            mapFogOfWarVM.Discovered.RemoveStream.Action(
                D,
                s =>
                {
                    _queuedIndexedRegions[s] = false;
                    _updateQueued = true;
                });
        }

        private void UpdateTexture()
        {
            UpdateIndexRegion();
            UpdateFogTexture();
            UpdateMapTexture();
        }

        private void UpdateIndexRegion()
        {
            if (_indexRegion != _newIndexRegion)
            {
                _indexRegion = _newIndexRegion;
                if (_indexRegion != null)
                {
                    _indexesDataArray = new int[FogOfWarShader.MaxIndexValue + 1];
                    _fogIndexTexture = FogOfWarShader.CreateIndexTexture(
                        _indexRegion.IndexWidth,
                        _indexRegion.IndexHeight,
                        _indexRegion.GetIndexBlocks()
                    );
                    _rawImage.texture = _mapRenderTexture;
                }
                else
                {
                    _fogIndexTexture = null;
                    _rawImage.texture = DefaultTexture;
                }
            }
        }

        private void UpdateFogTexture()
        {
            if (_indexRegion == null)
                return;

            if (_fogEnabled)
            {
                _fogDisabledIndexesDataArray = null;

                foreach (var regionValuePair in _queuedIndexedRegions)
                {
                    var indexedRegion = regionValuePair.Key;
                    if (indexedRegion.ParentIndexRegion != _indexRegion)
                        continue;
                    _indexesDataArray[indexedRegion.Index] = regionValuePair.Value ? 1 : 0;
                }

                _compBuf.SetData(_indexesDataArray);
            }
            else
            {
                _fogDisabledIndexesDataArray =
                    _fogDisabledIndexesDataArray ?? Enumerable.Repeat(1, FogOfWarShader.MaxIndexValue + 1).ToArray();
                _compBuf.SetData(_fogDisabledIndexesDataArray);
            }

            Material.SetBuffer(FogOfWarShader.BufferId, _compBuf);

            var fogTexture = RenderTexture.GetTemporary(
                BlurResolution.x,
                BlurResolution.y,
                0,
                RenderTextureFormat.R16,
                RenderTextureReadWrite.Linear);
            Graphics.Blit(_fogIndexTexture, fogTexture, Material, 0);

            var renderTextureBlurredX = RenderTexture.GetTemporary(
                BlurResolution.x,
                BlurResolution.y,
                0,
                RenderTextureFormat.RHalf,
                RenderTextureReadWrite.Linear);

            BlurMaterial.SetVector(BlurResolutionId, new Vector2(1f / BlurResolution.x, 1f / BlurResolution.y));
            BlurMaterial.SetVector(BlurDirectionId, new Vector2(1f, 0f));
            Graphics.Blit(fogTexture, renderTextureBlurredX, BlurMaterial);
            BlurMaterial.SetVector(BlurDirectionId, new Vector2(0f, 1f));
            Graphics.Blit(renderTextureBlurredX, _fogRenderTexture, BlurMaterial);
            RenderTexture.ReleaseTemporary(fogTexture);
            RenderTexture.ReleaseTemporary(renderTextureBlurredX);
        }

        private void UpdateMapTexture()
        {
            var gridThickness = GridThickness;
            if (MapZoomRatio > 0)
                gridThickness /= MapZoomRatio;
            Material.SetFloat(FogOfWarShader.GridThicknessId, gridThickness);
            Material.SetInt(FogOfWarShader.GridCellsCountId, GridCellsCount);
            Material.SetColor(FogOfWarShader.GridColorId, GridColor);
            Material.SetColor(FogOfWarShader.GridFogColorId, GridFogColor);

            Material.SetTexture(FogOfWarShader.FogTextureId, _fogRenderTexture);
            Graphics.Blit(_mapTexture, _mapRenderTexture, Material, 1);
        }

        private void OnDestroy()
        {
            _compBuf?.Release();
            _compBuf = null;
            _mapRenderTexture.Release();
            _fogRenderTexture.Release();
        }

        public FogOfWarView()
        {
            _instance = this;
        }

        private static FogOfWarView _instance;

        [Cheat, UsedImplicitly]
        public static void SetFogEnabled(bool enabled)
        {
            _instance.FogEnabled = enabled;
        }

        [Cheat, UsedImplicitly]
        public static void ClearRegions()
        {
            AsyncUtils.RunAsyncTask(
                async () =>
                {
                    var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                    var repo = GameState.Instance.ClientClusterNode;

                    using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                    {
                        var entity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                        await entity.FogOfWar.ClearRegions();
                    }
                }
            );
        }
    }
}