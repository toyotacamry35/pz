using Assets.Src.ResourceSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.SpawnSystem
{
    public class SpawnPoint : MonoBehaviour
    {
        [Tooltip("What kind of objects can be spawned here")]
        public JdbMetadata PointType;
    }
}
