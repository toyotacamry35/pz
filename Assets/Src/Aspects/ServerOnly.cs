using Assets.Src.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Src.Aspects
{
    class ServerOnly : ColonyBehaviour
    {
        private void Awake()
        {
            // if (Server == null)
            //     this.gameObject.SetActive(false);
        }
    }
}
