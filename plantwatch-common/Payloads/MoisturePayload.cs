using System.Collections.Generic;
using System.IO;
using InfluxDB.Collector;

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

        public override int ExpectedLength => base.ExpectedLength + 4;

        protected override void ConvertFromBytes(BinaryReader reader)
        {
            MoistureFraction = reader.ReadDouble();
        }

        protected override void ConvertToBytes(BinaryWriter writer)
        {
            writer.Write(MoistureFraction);
        }

        public override void SendInflux(MetricsCollector collector, string prefix)
        {
            collector.Write(
                $"{prefix}{UID}",
                new Dictionary<string, object>
                {
                    {"moisture", MoistureFraction}
                }
            );
        }

        public override string ToString()
        {
            return $"{StringHeader}: Moisture {MoistureFraction}";
        }
    }
}