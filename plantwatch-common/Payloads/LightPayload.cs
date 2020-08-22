using System.Collections.Generic;
using System.IO;
using InfluxDB.Collector;

namespace plantwatch
{
    public class LightPayload: Payload
    {
        // The raw lightness value read from the sensor
        public double RawLightValue;
        // The lightness value, adjusted to be absolute
        public double NormalizedLightValue;
        
        public LightPayload()
        {
            Version = 1;
            Type = PayloadTypes.Light;
        }

        public override int ExpectedLength => base.ExpectedLength + 16;

        protected override void ConvertFromBytes(BinaryReader reader)
        {
            RawLightValue = reader.ReadDouble();
            NormalizedLightValue = reader.ReadDouble();
        }

        protected override void ConvertToBytes(BinaryWriter writer)
        {
            writer.Write(RawLightValue);
            writer.Write(NormalizedLightValue);
        }

        public override void SendInflux(MetricsCollector collector, string prefix)
        {
            collector.Write(
                $"{prefix}{UID}",
                new Dictionary<string, object>
                {
                    {"light_raw", RawLightValue},
                    {"light_norm", NormalizedLightValue}
                }
            );
        }

        public override string ToString()
        {
            return $"{StringHeader}: Raw {RawLightValue} Norm {NormalizedLightValue}";
        }
    }
}