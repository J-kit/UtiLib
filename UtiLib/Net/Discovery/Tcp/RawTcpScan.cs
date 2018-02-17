using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UtiLib.IO;
using UtiLib.Shared.Enums;
using UtiLib.Tuples;

namespace UtiLib.Net.Discovery.Tcp
{
    public class FluidTcpScan
    {
        private readonly Task _scanWorker;

        private volatile bool _doWork = true;
        private readonly AsyncAutoResetEvent _ae = new AsyncAutoResetEvent();

        private readonly ConcurrentQueue<IEnumerable<IPEndPoint>> _scanSchedule = new ConcurrentQueue<IEnumerable<IPEndPoint>>();

        public EventHandler<FluidCallbackArgs> OnConnected;

        public FluidTcpScan()
        {
            _scanWorker = new Task(WorkerDoWork, TaskCreationOptions.LongRunning);
            _scanWorker.Start();
        }

        public void Enqueue(IEnumerable<IPEndPoint> scanTask)
        {
            _scanSchedule.Enqueue(scanTask);
            _ae.Set();
        }

        private async void WorkerDoWork()
        {
            while (_doWork)
            {
                await _ae.WaitAsync();

                if (!_scanSchedule.TryDequeue(out var tasks)) continue;

                foreach (var task in tasks)
                {
                    var scanSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    scanSocket.BeginConnect(task, Callback, Tuple.Create(scanSocket, task));
                }
            }
        }

        private void Callback(IAsyncResult ar)
        {
            if (ar.AsyncState is Tuple<Socket, IPEndPoint> tplResult)
            {
                var (scanSocket, scanEndPoint) = tplResult;
                try
                {
                    scanSocket.EndConnect(ar);
                }
                catch (Exception)
                {
                    //
                }

                var isConnected = scanSocket.Connected;
                if (isConnected)
                {
                    Logger.Log($"{scanEndPoint.Address}:{scanEndPoint.Port} Connected!", LogSeverity.Warning);
                    scanSocket.Close();
                    scanSocket.Dispose();
                }
                else
                {
                    // Logger.Log($"{scanEndPoint.Address}:{scanEndPoint.Port} Not Connected!", LogSeverity.Error);
                }

                OnConnected?.Invoke(this, new FluidCallbackArgs { EndPoint = scanEndPoint, Connected = isConnected });
            }
            else
            {
                Logger.Log($"Invalid object", LogSeverity.Error);
            }

            //Debugger.Break();
        }
    }

    public class FluidCallbackArgs
    {
        public IPEndPoint EndPoint { get; internal set; }
        public bool Connected { get; internal set; }
    }
}