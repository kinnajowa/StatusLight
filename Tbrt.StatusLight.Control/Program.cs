using System.IO.Pipes;
using BlinkStickNET;

namespace Tbrt.StatusLight.Control
{
    public abstract class Program
    {
        static readonly byte[] Red =
        [
            0, 255, 0,
            0, 255, 0,
            0, 255, 0,
            0, 255, 0,
            0, 255, 0,
            0, 255, 0,
            0, 255, 0,
            0, 255, 0
        ]; 
        
        static readonly byte[] Yellow =
        [
            255, 255, 0,
            255, 255, 0,
            255, 255, 0,
            255, 255, 0,
            255, 255, 0,
            255, 255, 0,
            255, 255, 0,
            255, 255, 0
        ]; 
        
        static readonly byte[] Green =
        [
            255, 0, 0,
            255, 0, 0,
            255, 0, 0,
            255, 0, 0,
            255, 0, 0,
            255, 0, 0,
            255, 0, 0,
            255, 0, 0
        ];

        private static readonly byte[] Off = 
        [
            0, 0, 0,
            0, 0, 0,
            0, 0, 0,
            0, 0, 0,
            0, 0, 0,
            0, 0, 0,
            0, 0, 0,
            0, 0, 0
        ];
        
        private enum Color : byte
        {
            Off = 0,
            Red,
            Yellow,
            Green,
        }

        private enum Mode : byte
        {
            Blink,
            Continuous
        }

        private struct Command
        {
            public Color Color;
            public Mode Mode;
            public short Duration;
        }
        
        public static int Main()
        {
            
            var device = BlinkStick.FindFirst();
            if (device == null) return 1;
            device.SetMode(2);
            if (!device.OpenDevice()) return 2;
            
            var pipe = new NamedPipeServerStream("tbrtstatuslight", PipeDirection.In);
            
            while (true)
            {
                pipe.WaitForConnection();

                var buffer = new byte[4];
                pipe.ReadExactly(buffer, 0, 4);
                var color = (Color) buffer[0];
                var mode = (Mode) buffer[1];
                var duration = BitConverter.ToInt16(buffer, 2);

                switch (mode)
                {
                    case Mode.Blink:
                        device.SetColors(0, GetColor(color));
                        Thread.Sleep(duration);
                        device.SetColors(0, Off);
                        break;
                    case Mode.Continuous:
                        device.SetColors(0, GetColor(color));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                pipe.Disconnect();
            }
        }

        private static byte[] GetColor(Color color)
        {
            return color switch
            {
                Color.Off => Off,
                Color.Red => Red,
                Color.Yellow => Yellow,
                Color.Green => Green,
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };
        }

        private static void OnNewCommand(Command command)
        {
            
        }
    }
}