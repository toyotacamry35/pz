using System.Collections.Generic;
using Assets.Src.SpatialSystem;
using UnityEngine;

namespace Uins
{
    public static class FogOfWarShader
    {
        public static readonly int BufferId = Shader.PropertyToID("_IndexBuffer");
        public static readonly int FogTextureId = Shader.PropertyToID("_FogTex");
        public static readonly int GridColorId = Shader.PropertyToID("_GridColor");
        public static readonly int GridFogColorId = Shader.PropertyToID("_GridFogColor");
        public static readonly int GridCellsCountId = Shader.PropertyToID("_GridCells");
        public static readonly int GridThicknessId = Shader.PropertyToID("_GridLineThickness");

        public const int MaxIndexValue = 32767;

        public static Texture2D CreateIndexTexture(int width, int height, IEnumerable<IndexBlock> indexBlocks)
        {
            var indexTexture = new Texture2D(width, height, TextureFormat.R16, false, true);

            var colors = new Color[width * height];
            foreach (var block in indexBlocks)
            {
                var color = new Color((float) block.Index / MaxIndexValue, 0, 0);
                for (var i = 0; i < block.Width * block.Height; i++) colors[i] = color;
                indexTexture.SetPixels(block.X, block.Y, block.Width, block.Height, colors);
            }

            indexTexture.Apply();

            return indexTexture;
        }
    }
}