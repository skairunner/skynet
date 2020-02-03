using System;

namespace plantwatch
{
    public class PacketParseError : Exception
    {
        public PacketParseError(string message): base(message)
        {
            
        }
    }

    public class PacketTypeError: PacketParseError
    {
        public PacketTypeError(string message): base(message) {}
    }
}