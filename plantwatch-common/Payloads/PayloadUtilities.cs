using System;
using System.IO;

namespace plantwatch
{
    public static class PayloadUtilities
    {
        public static PayloadTypes FindBinaryPayloadType(byte[] bytes)
        {
            var reader = new BinaryReader(new MemoryStream(bytes));
            return (PayloadTypes) reader.ReadInt32();
        }
    }
}