using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;

namespace Tbrt.StatusLight.Commander
{
    internal abstract class Program
    {
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private enum Color : byte
        {
            Off = 0,
            Red,
            Yellow,
            Green,
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private enum Mode : byte
        {
            Blink,
            Continuous
        }
        
        public static int Main(string[] args)
        {
            if (args.Length != 3) return 1;

            var success = true;

            success &= Enum.TryParse(args[0], true, out Color color);
            success &= Enum.TryParse(args[1], true, out Mode mode);
            success &= short.TryParse(args[2], out var duration);

            if (!success) return 1;
            
            var pipe = new NamedPipeClientStream(".", "tbrtstatuslight", PipeDirection.Out);
            pipe.Connect();
            pipe.WriteByte((byte)color);
            pipe.WriteByte((byte)mode);
            pipe.Write(BitConverter.GetBytes(duration));
            
            
            return 0;
        }
    }
}

