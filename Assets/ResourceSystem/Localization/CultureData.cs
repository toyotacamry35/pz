using System;

namespace L10n
{
    public struct CultureData : IEquatable<CultureData>
    {
        public string Code { get; set; }
        public string Folder { get; set; }
        public string FallbackCultureFolder { get; set; }
        public string Description { get; set; }

        public bool IsDev => Folder == "dev";

        public override string ToString()
        {
            return $"({nameof(CultureData)}: {nameof(Code)}={Code}, {nameof(Folder)}={Folder}, {nameof(FallbackCultureFolder)}={FallbackCultureFolder}, " +
                   $"{nameof(Description)}={Description})";
        }

        public bool Equals(CultureData other)
        {
            return Code == other.Code && Folder == other.Folder && FallbackCultureFolder == other.FallbackCultureFolder && Description == other.Description;
        }

        public override bool Equals(object obj)
        {
            return obj is CultureData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Code != null ? Code.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Folder != null ? Folder.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FallbackCultureFolder != null ? FallbackCultureFolder.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}