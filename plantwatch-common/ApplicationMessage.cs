using System;
using System.Collections.Generic;
using System.IO;

namespace plantwatch
{
    public enum PayloadTypes
    {
        Generic = 0,
    }
    
    // MQTT packets have binary data as the payload, which we define here.
    public class Payload
    {
        public PayloadTypes Type = PayloadTypes.Generic;
        public Int32 Version;

        public long Timestamp; // This is always the last thing to serialize, because it's appended to the packet data by the MQTT server.
        // It should be 64 bit epoch time.

        public virtual int ExpectedLength => 16; // the expected length of the packet in bytes

        public virtual byte[] ToBytes(bool includeTimestamp = true)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            
            ConvertToBytes(writer);

            if (includeTimestamp)
            {
                writer.Write(Timestamp);
            }
            
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            return stream.ToArray();
        }

        // ideally implement the conversion entirely within this method, because the ToBytes method does some bookkeeping.
        protected virtual void ConvertToBytes(BinaryWriter writer)
        {
            writer.Write((int)Type);
            writer.Write(Version);
        }

        public virtual void FromBytes(byte[] bytes)
        {
            if (bytes.Length != ExpectedLength)
            {
                throw new PacketParseError($"Expected the payload to be {ExpectedLength} bytes, but it was actually {bytes.Length} bytes.");
            }

            var byteReader = new BinaryReader(new MemoryStream(bytes));

            var packetType = (PayloadTypes) byteReader.ReadInt32();

            if (packetType != PayloadTypes.Generic)
            {
                throw new PacketParseError($"Expected the payload to be of value {Type} ({Type.ToString()}, but it was actually {packetType} ({packetType.ToString()}).");
            }
            
            ConvertFromBytes(byteReader);
        }

        protected virtual void ConvertFromBytes(BinaryReader reader)
        {
            Version = reader.ReadInt32();
            Timestamp = reader.ReadInt64();
        }
    }
}