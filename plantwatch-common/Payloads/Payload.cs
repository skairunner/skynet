using System;
using System.Collections.Generic;
using System.IO;

namespace plantwatch
{
    public enum PayloadTypes
    {
        Generic = 0,
        Moisture
    }
    
    // MQTT packets have binary data as the payload, which we define here.
    public class Payload
    {
        public PayloadTypes Type { get; protected set; } = PayloadTypes.Generic;
        public Int32 Version = 1;
        public UInt32 UID = 0; // Non-zero UIDs are valid

        public long Timestamp; // This is always the last thing to serialize, because it's appended to the packet data by the MQTT server.
        // It should be 64 bit epoch time.

        public virtual int ExpectedLength => 20; // the expected length of the packet in bytes
        protected string StringHeader => $"Payload '{Type.ToString()}' v{Version} @ {UnixTime.FromUnix(Timestamp).LocalDateTime}";

        public virtual byte[] ToBytes(bool includeTimestamp = true)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            
            writer.Write((int)Type);
            writer.Write(Version);
            writer.Write(UID);
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
        }

        public virtual void FromBytes(byte[] bytes)
        {
            if (bytes.Length != ExpectedLength)
            {
                throw new PacketParseError($"Expected the payload to be {ExpectedLength} bytes, but it was actually {bytes.Length} bytes.");
            }

            var byteReader = new BinaryReader(new MemoryStream(bytes));

            var packetType = (PayloadTypes) byteReader.ReadInt32();

            if (packetType != Type)
            {
                throw new PacketParseError($"Expected the payload to be of value {Type} ({Type.ToString()}, but it was actually {packetType} ({packetType.ToString()}).");
            }
            
            Version = byteReader.ReadInt32();
            UID = byteReader.ReadUInt32();
            ConvertFromBytes(byteReader);
            Timestamp = byteReader.ReadInt64();
        }

        protected virtual void ConvertFromBytes(BinaryReader reader)
        {
        }

        public override string ToString()
        {
            return $"{StringHeader}: Empty";
        }
    }
}