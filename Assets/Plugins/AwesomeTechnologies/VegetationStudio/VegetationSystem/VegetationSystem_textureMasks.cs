using System.Collections.Generic;
using AwesomeTechnologies.Vegetation;
using UnityEngine;
using System;

namespace AwesomeTechnologies
{
    public partial class VegetationSystem
    {
        public List<LocalTextureMaskTextureInfo> LocalTextureMaskTextureList =
            new List<LocalTextureMaskTextureInfo>();

        public List<TextureMaskBase> RuntimeTextureMaskList = new List<TextureMaskBase>();

        public Texture2D GetLocalTextureMaskTexture(string id)
        {
            for (int i = 0; i <= LocalTextureMaskTextureList.Count - 1; i++)
            {
                if (LocalTextureMaskTextureList[i].Id == id) return LocalTextureMaskTextureList[i].MaskTexture;
            }

            return null;
        }

        public void RemoveLocalTextureMaskTexture(string id)
        {
            for (int i = 0; i <= LocalTextureMaskTextureList.Count - 1; i++)
            {
                if (LocalTextureMaskTextureList[i].Id == id)
                {
                    LocalTextureMaskTextureList.RemoveAt(i);
                    break;
                }
            }
        }

        public void UpdateLocalTextureMaskTexture(string id, Texture2D maskTexture)
        {
            RemoveLocalTextureMaskTexture(id);
            LocalTextureMaskTextureInfo localTextureMaskTextureInfo = new LocalTextureMaskTextureInfo
            {
                Id = id,
                MaskTexture = maskTexture
            };

            LocalTextureMaskTextureList.Add(localTextureMaskTextureInfo);
        }

        public void RefreshTextureMasks()
        {
            RuntimeTextureMaskList.Clear();

            for (int i = 0; i <= currentVegetationPackage.TextureMaskList.Count - 1; i++)
            {
                Type textureMaskType = TextureMaskTypeManager.GetTypeFromMaskTypeID(currentVegetationPackage
                    .TextureMaskList[i]
                    .TextureMaskTypeId);
                if (textureMaskType != null)
                {
                    TextureMaskBase textureMask = Activator.CreateInstance(textureMaskType) as TextureMaskBase;
                    if (textureMask != null)
                    {
                        textureMask.CopySettingsFrom(currentVegetationPackage.TextureMaskList[i]);

                        if (textureMask.TextureMaskTextureStorage == TextureMaskTextureStorage.VegetationSystem)
                        {
                            textureMask.MaskTexture = GetLocalTextureMaskTexture(textureMask.MaskID);
                        }                        

                        for (int j = 0;
                            j <= currentVegetationPackage.TextureMaskList[i].TextureMaskPropertiesList.Count - 1;
                            j++)
                        {
                            textureMask.AddTextureMaskProperty(new TextureMaskProperty(currentVegetationPackage
                                .TextureMaskList[i]
                                .TextureMaskPropertiesList[j]));
                        }
                        textureMask.RefreshTextureMask();

                        RuntimeTextureMaskList.Add(textureMask);
                        //CurrentVegetationPackage.TextureMaskList[i].RuntimeTextureMask = textureMask;
                    }
                }
            }

            //for (int i = 0; i <= CurrentVegetationPackage.VegetationInfoList.Count - 1; i++)
            //{
            //    for (int j = 0; j <= CurrentVegetationPackage.VegetationInfoList[i].DensityTextureMaskRuleList.Count -1 ; j++)
            //    {
            //        TextureMaskBase tempTextureMask = GetTextureMask(CurrentVegetationPackage.VegetationInfoList[i].DensityTextureMaskRuleList[j].MaskId);
            //        if (tempTextureMask != null)
            //        {
            //            CurrentVegetationPackage.VegetationInfoList[i].DensityTextureMaskRuleList[j].TextureMask =
            //                tempTextureMask.RuntimeTextureMask;
            //        }
            //        else
            //        {
            //            CurrentVegetationPackage.VegetationInfoList[i].DensityTextureMaskRuleList[j].TextureMask = null;
            //        }
            //    }

            //    for (int j = 0; j <= CurrentVegetationPackage.VegetationInfoList[i].ExcludeTextureMaskRuleList.Count -1; j++)
            //    {
            //        TextureMaskBase tempTextureMask = GetTextureMask(CurrentVegetationPackage.VegetationInfoList[i].ExcludeTextureMaskRuleList[j].MaskId);
            //        if (tempTextureMask != null)
            //        {
            //            CurrentVegetationPackage.VegetationInfoList[i].ExcludeTextureMaskRuleList[j].TextureMask =
            //                tempTextureMask.RuntimeTextureMask;
            //        }
            //        else
            //        {
            //            CurrentVegetationPackage.VegetationInfoList[i].ExcludeTextureMaskRuleList[j].TextureMask = null;
            //        }
            //    }

            //    for (int j = 0; j <= CurrentVegetationPackage.VegetationInfoList[i].IncludeTextureMaskRuleList.Count -1; j++)
            //    {
            //        TextureMaskBase tempTextureMask = GetTextureMask(CurrentVegetationPackage.VegetationInfoList[i].IncludeTextureMaskRuleList[j].MaskId);
            //        if (tempTextureMask != null)
            //        {
            //            CurrentVegetationPackage.VegetationInfoList[i].IncludeTextureMaskRuleList[j].TextureMask =
            //                tempTextureMask.RuntimeTextureMask;
            //        }
            //        else
            //        {
            //            CurrentVegetationPackage.VegetationInfoList[i].IncludeTextureMaskRuleList[j].TextureMask = null;
            //        }
            //    }
            //}
        }

        public string GetDefaultTextureMaskID()
        {
            if (currentVegetationPackage.TextureMaskList.Count > 0)
            {
                return currentVegetationPackage.TextureMaskList[0].MaskID;
            }

            return "";
        }

        public Texture2D GetTextureMaskTexture(string maskID)
        {
            TextureMaskBase textureMask = GetTextureMask(maskID);
            if (textureMask != null)
            {
                if (textureMask.MaskTexture)
                {
                    return textureMask.MaskTexture;
                }
            }
            return null;
        }

        public void AddTextureMaskProperties(TextureMaskRule textureMaskRule)
        {
            textureMaskRule.TextureMaskPropertiesList.Clear();
            TextureMaskBase textureMask = GetTextureMask(textureMaskRule.MaskId);
            if (textureMask == null) return;

            for (int i = 0; i <= textureMask.TextureMaskPropertiesList.Count - 1; i++)
            {
                textureMaskRule.TextureMaskPropertiesList.Add(new TextureMaskProperty(textureMask.TextureMaskPropertiesList[i]));
            }
        }

        public TextureMaskBase GetTextureMask(string id)
        {
            for (int i = 0; i <= currentVegetationPackage.TextureMaskList.Count - 1; i++)
            {
                if (currentVegetationPackage.TextureMaskList[i].MaskID == id)
                    return currentVegetationPackage.TextureMaskList[i];
            }

            return null;
        }

        public string[] GetTextureMaskNameArray()
        {
            string[] names = new string[currentVegetationPackage.TextureMaskList.Count];

            for (int i = 0; i <= currentVegetationPackage.TextureMaskList.Count - 1; i++)
            {
               names[i] = currentVegetationPackage.TextureMaskList[i].TextureMaskTypeReadableName + " - " +
                            currentVegetationPackage.TextureMaskList[i].MaskName;
            }

            return names;
        }

        public string GetTextureMaskIDFromIndex(int index)
        {
            if (currentVegetationPackage && currentVegetationPackage.TextureMaskList.Count > index)
            {
                return currentVegetationPackage.TextureMaskList[index].MaskID;
            }
            return "";
        }

        public int GetTextureMaskIndexFromID(string maskID)
        {
            for (int i = 0; i <= currentVegetationPackage.TextureMaskList.Count - 1; i++)
            {
                if (currentVegetationPackage.TextureMaskList[i].MaskID == maskID) return i;
            }

            return -1;
        }

        public bool HasTextureMask()
        {
            if (currentVegetationPackage.TextureMaskList.Count > 0) return true;
            return false;
        }
    }
}
