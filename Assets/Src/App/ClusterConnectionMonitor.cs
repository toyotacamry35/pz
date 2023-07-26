using Assets.Src.Wizardry;
using GeneratedCode.Manual.Repositories;
using Infrastructure.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.App
{
    public class ClusterConnectionMonitor : MonoBehaviour
    {
        float time;
        void OnGUI()
        {
            List<ClusterStatusReport> stReps = null;// CloudHost.ClusterNode?.StatusReports;
            if (stReps == null)
                return;
            if (stReps.Count == 0)
                return;
            var startC = GUI.color;
            if(stReps.Any() && stReps.All(x=>x.IsReady))
            {
                time += Time.deltaTime;
                if(time > 5)
                {
                    enabled = false;
                    return;
                }
            }
            try
            {
                for (int i = 0; i < stReps.Count; i++)
                {
                    var sr = stReps[i];
                    var pc = GUI.color;

                    GUI.color = sr.IsReady ? Color.green : Color.red;
                    var ownPos = GetPosition(i, stReps.Count);
                    GUI.Label(new Rect(ownPos, new Vector2(100, 30)), new GUIContent($"{sr.Label}"));
                    GUI.color = pc;
                    foreach (var hasRepoCom in sr.HasRepoComEntities)
                    {
                        GUIExtensions.DrawLine(ownPos, GetPosition(GetIndex(hasRepoCom.ConfigId, stReps), stReps.Count), hasRepoCom.ExternalCommunicationNodeOpen ? Color.green : Color.red);
                   
                    }
                }
            }
            finally
            {
                GUI.color = startC;
            }
        }

        int GetIndex(string label, List<ClusterStatusReport> reports)
        {
            for (int i = 0; i < reports.Count; i++)
                if (label == reports[i].Label)
                    return i;
            return -1;
        }

        Vector3 GetPosition(int pos, int count)
        {

            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            float radius = 100;
            float anglePerStep = 360 / count;
            var qe = Quaternion.Euler(0, 0, anglePerStep * pos);
            var labelPoint = screenCenter + qe * Vector3.up * radius;
            return labelPoint;
        }
    }
}
