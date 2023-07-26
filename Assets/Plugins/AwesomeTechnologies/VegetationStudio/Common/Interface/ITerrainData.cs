using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITerrainData {
    int heightmapHeight { get;  }
    int heightmapWidth { get;  }
    Vector3 size { get;  }
    Vector3 terrainPosition { get;  }

    int alphamapLayers { get; }
    int alphamapWidth { get; }
    int alphamapHeight { get; }
    float alphamapCellWidth { get; }
    float alphamapCellHeight { get; }

    float GetInterpolatedHeight(float x, float y);

    Vector3 GetInterpolatedNormal(float x, float y);

    float GetSteepness(float x, float y);
}
