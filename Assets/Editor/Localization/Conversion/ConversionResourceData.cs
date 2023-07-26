//using System.Collections.Generic;
//using ResourcesSystem.Loader;
//using EnumerableExtensions;
//using L10n.ConversionNs;
//
//namespace L10n
//{
//    public class ConversionResourceData : ResourceData
//    {
//        public List<string> UnconvertedProps = new List<string>();
//
//        public bool IsConverted;
//
//        public bool HasUnconvertedProps => UnconvertedProps.Count > 0;
//
//        public ConversionResourceData(string relPath, GameResources gameResources) : base(relPath, gameResources)
//        {
//        }
//
//        public void UpdateUnconvertedProps(List<ConversionMetadata> conversionMetadataList)
//        {
//            IsConverted = false;
//            UnconvertedProps.Clear();
//
//            if (conversionMetadataList == null)
//                return;
//
//            foreach (var conversionMetadata in conversionMetadataList)
//            {
//                if (conversionMetadata.IsConverted)
//                    IsConverted = true;
//
//                if (conversionMetadata.UnconvertedStrings != null)
//                    conversionMetadata.UnconvertedStrings.ForEach(s =>
//                    {
//                        if (!string.IsNullOrEmpty(s))
//                            UnconvertedProps.Add(s);
//                    });
//            }
//
//            if (IsConverted)
//                Reload();
//        }
//    }
//}