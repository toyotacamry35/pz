using ResourcesSystem.Loader;
using ProtoBuf;

namespace SharedCode.Utils
{
    [KnownToGameResources]
    [ProtoContract]
    public struct Color
    {
        [ProtoMember(1)]
        public float R;
        [ProtoMember(2)]
        public float G;
        [ProtoMember(3)]
        public float B;

        public Color(float r, float g, float b) : this()
        {
            R = r;
            G = g;
            B = b;
        }
#if UNITY_5_3_OR_NEWER
        public static implicit operator UnityEngine.Color (Color col)
        {
            return new UnityEngine.Color(col.R, col.G, col.B);
        }
#endif

        public static Color red
        {
            get
            {
                return new Color(1f, 0.0f, 0.0f);
            }
        }

        /// <summary>
        ///   <para>Solid green. RGBA is (0, 1, 0, 1).</para>
        /// </summary>
        public static Color green
        {
            get
            {
                return new Color(0.0f, 1f, 0.0f);
            }
        }

        /// <summary>
        ///   <para>Solid blue. RGBA is (0, 0, 1, 1).</para>
        /// </summary>
        public static Color blue
        {
            get
            {
                return new Color(0.0f, 0.0f, 1f);
            }
        }

        /// <summary>
        ///   <para>Solid white. RGBA is (1, 1, 1, 1).</para>
        /// </summary>
        public static Color white
        {
            get
            {
                return new Color(1f, 1f, 1f);
            }
        }

        /// <summary>
        ///   <para>Solid black. RGBA is (0, 0, 0, 1).</para>
        /// </summary>
        public static Color black
        {
            get
            {
                return new Color(0.0f, 0.0f, 0.0f);
            }
        }

        /// <summary>
        ///   <para>Yellow. RGBA is (1, 0.92, 0.016, 1), but the color is nice to look at!</para>
        /// </summary>
        public static Color yellow
        {
            get
            {
                return new Color(1f, 0.9215686f, 0.01568628f);
            }
        }

        /// <summary>
        ///   <para>Cyan. RGBA is (0, 1, 1, 1).</para>
        /// </summary>
        public static Color cyan
        {
            get
            {
                return new Color(0.0f, 1f, 1f);
            }
        }

        /// <summary>
        ///   <para>Magenta. RGBA is (1, 0, 1, 1).</para>
        /// </summary>
        public static Color magenta
        {
            get
            {
                return new Color(1f, 0.0f, 1f);
            }
        }

        /// <summary>
        ///   <para>Gray. RGBA is (0.5, 0.5, 0.5, 1).</para>
        /// </summary>
        public static Color gray
        {
            get
            {
                return new Color(0.5f, 0.5f, 0.5f);
            }
        }

        /// <summary>
        ///   <para>English spelling for gray. RGBA is the same (0.5, 0.5, 0.5, 1).</para>
        /// </summary>
        public static Color grey
        {
            get
            {
                return new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }
}
