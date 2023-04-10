using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ScreenCapture.ScreenCapture;

namespace remoteControlV2Capturer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new Capturer(
                    IPAddress.Parse(args[0]),
                    ushort.Parse(args[1]),
                    ushort.Parse(args[2])
                    ).Work();
            }
            catch (Exception)
            {
                return;
            }
        }
    }

    class Capturer
    {
        public static double frameRate = 24;
        public IPEndPoint endPoint1;
        public IPEndPoint endPoint2;
        IPAddress address;

        TcpClient client;

        CaptureWorker worker1;
        CaptureWorker worker2;

        Timer timer;
        double[] fps = new double[]
        {
            5,
            10,
            20,
            24,
            30,
            33
        };


        public Capturer(IPAddress address, ushort port1, ushort port2)
        {
            this.address = address;
            endPoint1 = new IPEndPoint(address, port1);
            endPoint2 = new IPEndPoint(address, port2);
        }


        public void Work()
        {
            timer = new Timer((e) =>
            {

            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1 / frameRate));
        }

        public void Pause()
        {
            timer.Dispose();
        }
    }

    class CaptureWorker
    {
        public IPEndPoint endPoint;
        public UdpClient client;
        bool enabled = true;

        public CaptureWorker(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
            client = new UdpClient(endPoint);
        }

        public async void Capture(int segment = 0)
        {
            byte[] buffer = CaptureScreen(segment);

            var mem = new MemoryStream();
            using (var zipStream = new GZipStream(mem, CompressionMode.Compress))
            {
                zipStream.Write(buffer, 0, buffer.Length);
                var compressed = mem.ToArray();
            }
        }
    }
}
