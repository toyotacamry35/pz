using UnityEngine;

namespace Assets.Src.Aspects
{
    public interface IEntityView
    {
        GameObject GameObject { get; }

        bool Enabled { get; set; }
    }
}