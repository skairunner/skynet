using System.IO;

namespace plantwatch
{
    public class MoisturePayload: Payload
    {
        public double MoistureFraction;

        public MoisturePayload()
        {
            Version = 1;
            Type = PayloadTypes.Moisture;
        }

        public override int ExpectedLength => base.ExpectedLength + 8;

        protected override void ConvertFromBytes(BinaryReader reader)
        {
            MoistureFraction = reader.ReadDouble();
        }

        protected override void ConvertToBytes(BinaryWriter writer)
        {
            writer.Write(MoistureFraction);
        }

        public override string ToString()
        {
            return $"{StringHeader}: Moisture {MoistureFraction}";
        }
    }
}