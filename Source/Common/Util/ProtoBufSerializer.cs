using System;
using System.IO;
using ProtoBuf;

namespace Common
{
    /// <summary>
    /// Wrapper over the <see cref="ProtoBuf.Serializer"/>.
    /// </summary>
    public struct ProtoBufSerializer
    {
        public static Byte[] Serialize<T>(T o)
        {
            using (MemoryStream stream = new MemoryStream(1024))
            {
                Serializer.Serialize<T>(stream, o);
                return stream.ToArray();
            }
        }

        public static T DeSerialize<T>(Byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                stream.Position = 0;
                return (T)Serializer.Deserialize<T>(stream);
            }
        }
    }
}




