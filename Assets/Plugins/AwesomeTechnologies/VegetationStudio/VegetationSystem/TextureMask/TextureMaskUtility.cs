using Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AwesomeTechnologies.Vegetation
{
    public class TextureMaskUtility
    {

    }

    [Serializable]
    public class TextureMaskTypeInfo
    {
        public string MaskTypeName;
        public Type MaskType;
        public string TextureMaskTypeId;
    }

    public class TextureMaskTypeManager
    {
        public List<TextureMaskTypeInfo> TypeList;

        private void InitTextureMaskTypes()
        {
            var interfaceType = typeof(ITextureMask);
            var maskTypes = AppDomain.CurrentDomain.GetAssembliesSafe()
                .SelectMany(x => x.GetTypesSafe())
                .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance);

            foreach (var maskType in maskTypes)
            {
                ITextureMask maskInterface = maskType as ITextureMask;
                if (maskInterface != null)
                {
                    InternalRegisterTextureMaskType(maskInterface.GetMaskType(), maskInterface.GetMaskTypeName(), maskInterface.GetTextureMaskTypeID());
                }
            }
        }

        public TextureMaskTypeManager()
        {
            TypeList = new List<TextureMaskTypeInfo>();
            InitTextureMaskTypes();
        }

        public static TextureMaskTypeManager Instance;
        static void ValidateInstance()
        {
            if (Instance == null)
            {
                Instance = new TextureMaskTypeManager();
            }
        }

        public static Type GetTypeFromMaskTypeID(string textureMaskTypeID)
        {
            ValidateInstance();
            for (int i = 0; i <= Instance.TypeList.Count - 1; i++)
            {
                if (Instance.TypeList[i].TextureMaskTypeId == textureMaskTypeID) return Instance.TypeList[i].MaskType;
            }
            return null;
        }
        void InternalRegisterTextureMaskType(Type type, string maskTypeName, string textureMaskTypeID)
        {
            for (int i = 0; i <= TypeList.Count - 1; i++)
            {
                if (TypeList[i].MaskType == type) return;
            }
            TextureMaskTypeInfo textureMaskTypeInfo = new TextureMaskTypeInfo
            {
                MaskType = type,
                MaskTypeName = maskTypeName,
                TextureMaskTypeId = textureMaskTypeID
            };
            TypeList.Add(textureMaskTypeInfo);
        }
        public static void RegisterTextureMaskType(Type type, string maskTypeName, string textureMaskTypeID)
        {
            //Debug.Log("RegisterTextureMaskType:" + maskTypeName);

            ValidateInstance();
            Instance.InternalRegisterTextureMaskType(type, maskTypeName, textureMaskTypeID);
        }

        public static string[] GetTextureMaskTypeStringArray()
        {
            ValidateInstance();

            string[] strings = new String[Instance.TypeList.Count];

            for (int i = 0; i <= Instance.TypeList.Count - 1; i++)
            {
                strings[i] = Instance.TypeList[i].MaskTypeName;
            }

            return strings;
        }

        public static Type GetTextureMaskTypeFromIndex(int index)
        {
            ValidateInstance();

            if (Instance.TypeList.Count > index)
            {
                return Instance.TypeList[index].MaskType;
            }

            return null;
        }

        public static void ListAllTypes()
        {
            ValidateInstance();
            for (int i = 0; i <= Instance.TypeList.Count - 1; i++)
            {
                Debug.Log(Instance.TypeList[i].MaskTypeName + " " + i.ToString());
            }
        }
    }
}

