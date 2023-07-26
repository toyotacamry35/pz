using Assets.Src.ResourceSystem;
using SharedCode.Aspects.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.SpawnSystem
{
    public class SpawnRegion : MonoBehaviour
    {
        [Tooltip("Any if none set")]
        public List<JdbMetadata> IncludedSpawnTemplates = new List<JdbMetadata>();
        public List<SpawnTemplate> Templates = new List<SpawnTemplate>();
    }
}
