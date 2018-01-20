using System;

namespace UtiLib.Net.Headers
{
    [Flags]
    public enum TcpFlags
    {
        Default = 1,
        Fin = 2,
        Syn = 4,
        Rst = 8,
        Psh = 16,
        Ack = 32,
        Urg = 64
    }
}