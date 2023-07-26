using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Assets.Src.Audio.Editor
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(AkEventRelay))]
    public class AkEventRelayInspector : AkBaseInspector
    {
        SerializedProperty eventID;

        private GameObject emitterObject;

        public void OnEnable()
        {

            eventID = serializedObject.FindProperty("eventID");

            
        }

        public override void OnChildInspectorGUI()
        {
            serializedObject.Update();
        }

        
    }


}
