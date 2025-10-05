using System;

namespace Ca_Tcp_Sockets_Serv02
{
    [Serializable]
    public class TcpDataDto
    {
        public string RouteKey { get; set; }
        public string Operation { get; set; }
        public string Data { get; set; }
        public bool Compare(TcpDataDto dto)
        {
            return RouteKey == dto.RouteKey && Data == dto.Data;
        }
        public override string ToString()
        {
            return $"RouteKey:{RouteKey} Operation:{Operation}{Environment.NewLine}Data: {Data}";
        }
    }

}
