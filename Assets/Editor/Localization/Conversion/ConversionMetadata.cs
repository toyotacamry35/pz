//using EnumerableExtensions;
//
//namespace L10n.ConversionNs
//{
//    public class ConversionMetadata
//    {
//        public string JdbRelPath;
//        public string HierPathLs;
//        public string[] UnconvertedStrings;
//        public bool IsConverted;
//
//        public string HierPath => JdbPropsConverter.GetHierPathForStringProperty(HierPathLs);
//
//        public ConversionMetadata(string jdbRelPath, string hierPathLs, string[] unconvertedStrings,
//            bool isConverted)
//        {
//            JdbRelPath = jdbRelPath;
//            HierPathLs = hierPathLs;
//            UnconvertedStrings = unconvertedStrings;
//            IsConverted = isConverted;
//        }
//
//        public override string ToString()
//        {
//            return $"(ConversionMetadata: JdbRelPath='{JdbRelPath}', HierPathLs='{HierPathLs}', HierPath={HierPath} " +
//                   $"UnconvertedStrings: {UnconvertedStrings.ItemsToString()}, IsConverted{IsConverted.AsSign()})";
//        }
//    }
//}