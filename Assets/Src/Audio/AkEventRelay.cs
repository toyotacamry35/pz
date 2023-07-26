using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.Audio
{

    public class AkEventRelay : MonoBehaviour
    {

#if UNITY_EDITOR
        public byte[] valueGuid = new byte[16];
#endif

        /// ID of the Event as found in the WwiseID.cs file
        public uint eventID = 0;
    }
}
