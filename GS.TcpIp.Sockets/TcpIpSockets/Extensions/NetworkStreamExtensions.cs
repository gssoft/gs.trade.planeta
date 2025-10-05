using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace TcpIpSockets.Extensions
{
    public static class NetworkStreamExtensions
    {
        public static string ReadFrStrBuilder(this NetworkStream networkStream, int buffLength,
            Action<string> msgAction = null,
            Action<Exception> exAction = null)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            Exception exc;
            StringBuilder completeMessage = new StringBuilder();
            try
            {
                networkStream.ReadTimeout = 15*1000;
                if (networkStream.CanRead)
                {
                    byte[] myReadBuffer = new byte[buffLength];
                    do
                    {
                        var numberOfBytesRead = networkStream.Read(myReadBuffer, 0, myReadBuffer.Length);
                        // completeMessage.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));
                        completeMessage.Append(Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));
                    } while (networkStream.DataAvailable);

                    msgAction?.Invoke($"Method: {m} {completeMessage}");
                    return completeMessage.ToString();
                }
                msgAction?.Invoke("NetworkStream Can't Read.");
                return null;
            }
            catch (SocketException e)
            {
                msgAction?.Invoke($"Method:{m} {e.Message}");
                exc = e;
            }
            catch (Exception e)
            {
                msgAction?.Invoke($"Method:{m} {e.Message}");
                exc = e;
            }
            exAction?.Invoke(exc);
            return null;
        }
        public static Tuple<string, int> ReadFrStrBuilderTpl(this NetworkStream networkStream, int buffLength,
            Action<string> msgAction = null,
            Action<Exception> exAction = null)
        {
            Tuple<string, int> retTuple;
            var m = MethodBase.GetCurrentMethod().Name + "()";
            Exception exc;
            StringBuilder completeMessage = new StringBuilder();
            try
            {
                networkStream.ReadTimeout = 15*1000;
                if (networkStream.CanRead)
                {
                    byte[] myReadBuffer = new byte[buffLength];
                    do
                    {
                        var numberOfBytesRead = networkStream.Read(myReadBuffer, 0, myReadBuffer.Length);
                        // completeMessage.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));
                        completeMessage.Append(Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));
                    } while (networkStream.DataAvailable);

                    msgAction?.Invoke($"Method: {m} {completeMessage}");
                    return Tuple.Create(completeMessage.ToString(), 1);
                }
                msgAction?.Invoke("NetworkStream Can't Read.");
                return Tuple.Create(completeMessage.ToString(), -3);
            }
            catch (SocketException e)
            {
                msgAction?.Invoke($"Method:{m} {e.Message}");
                exc = e;
                retTuple = Tuple.Create("", -1);
            }
            catch (Exception e)
            {
                msgAction?.Invoke($"Method:{m} {e.Message}");
                exc = e;
                retTuple = Tuple.Create("", -2);
            }
            exAction?.Invoke(exc);
            return retTuple;
        }

        public static string ReadFrStreamReader(this NetworkStream networkStream, int buffLength,
            Action<string> msgAction = null,
            Action<Exception> exAction = null)
        {
            Exception exc;
            StringBuilder completeMessage = new StringBuilder();
            try
            {
                StreamReader readerStream = new StreamReader(networkStream);

                if (networkStream.CanRead)
                {
                    byte[] myReadBuffer = new byte[buffLength];
                    do
                    {
                        var numberOfBytesRead = networkStream.Read(myReadBuffer, 0, myReadBuffer.Length);
                        completeMessage.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));
                        // completeMessage.Append(Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));
                    } while (networkStream.DataAvailable);

                    msgAction?.Invoke($"Server: {completeMessage}");
                    return completeMessage.ToString();
                }
                msgAction?.Invoke("NetworkStream Can't Read.");
                return null;
            }
            catch (Exception e)
            {
                exc = e;
            }
            exAction?.Invoke(exc);
            return null;
        }

        public static int Write(this NetworkStream networkStream, string str,
            Action<string> msgAction = null,
            Action<Exception> exAction = null)
        {
            var m = MethodBase.GetCurrentMethod().Name;
            Exception exc = null;
            try
            {
                byte[] dataWrite = Encoding.UTF8.GetBytes(str);
                networkStream.Write(dataWrite, 0, dataWrite.Length);
                return 1;
            }
            catch (SocketException e)
            {
                msgAction?.Invoke($"Method:{m} {e.Message}");
                exc = e;
                exAction?.Invoke(exc);
                return -1;
            }
            catch (Exception e)
            {
                msgAction?.Invoke($"Method:{m} {e.Message}");
                exc = e;
                exAction?.Invoke(exc);
                return -2;
            }
        }

        public static void Write(this NetworkStream networkStream, byte[] bytes)
        {
            if (!networkStream.CanWrite) return;
            networkStream.Write(bytes, 0, bytes.Length);
        }

        public static int Write(this NetworkStream networkStream, byte[] bytes,
            Action<string> msgAction = null,
            Action<Exception> exAction = null)
        {
            var m = MethodBase.GetCurrentMethod().Name;
            Exception exc = null;
            try
            {
                if (networkStream.CanWrite)
                {
                    networkStream.Write(bytes, 0, bytes.Length);
                }
                return 1;
            }
            catch (SocketException e)
            {
                msgAction?.Invoke($"Method:{m} {e.Message}");
                exc = e;
                exAction?.Invoke(exc);
                return -1;
            }
            catch (Exception e)
            {
                msgAction?.Invoke($"Method:{m} {e.Message}");
                exc = e;
                exAction?.Invoke(exc);
                return -2;
            }
        }

        public static byte[] ReadToBytes(this NetworkStream networkStream, int tcpClientBufferSize,
            Action<string> msgAction = null,
            Action<Exception> exAction = null)
        {
            var m = MethodBase.GetCurrentMethod().Name;
           // try
           // {
                //if (!networkStream.CanRead) return null;
                // Buffer to store the response bytes.
                //byte[] readBuffer = new byte[tcpClient.ReceiveBufferSize];
                byte[] readBuffer = new byte[tcpClientBufferSize];
                using (var writer = new MemoryStream())
                {
                    // while (networkStream.DataAvailable)
                    if (networkStream.CanRead)
                    {
                        do
                        {
                            var numberOfBytesRead = networkStream.Read(readBuffer, 0, readBuffer.Length);
                            // if (numberOfBytesRead <= 0) break;
                            writer.Write(readBuffer, 0, numberOfBytesRead);
                        } while (networkStream.DataAvailable);
                        // fullServerReply = Encoding.UTF8.GetString(writer.ToArray());
                        // Console.WriteLine($"{m}:{writer.Length}");
                        var bytes = writer.ToArray();
                        // Console.WriteLine($"{m}:Writer:{writer.Length} Bytes:{bytes.Length}");
                        return bytes;
                    }
                    msgAction?.Invoke("NetworkStream Can't Read.");
                    return null;
                }
            //}
            //catch (System.Net.Sockets.SocketException e)
            //{
            //    var exctype = e.GetType();
            //    msgAction?.Invoke($"Method:{m} Type:{exctype} {e.Message}");
            //    exAction?.Invoke(e);

            //    if (e.InnerException != null)
            //    {
            //        var ex = e.InnerException;
            //        var extype = ex.GetType();
            //        msgAction?.Invoke($"Method:{m} Type:{extype} {e.Message}");
            //        exAction?.Invoke(e);

            //    }
            //    return null;
            //}
            //catch (System.IO.IOException e)
            //{
            //    var exctype = e.GetType();
            //    msgAction?.Invoke($"Method:{m} Type:{exctype} {e.Message}");
            //    exAction?.Invoke(e);

            //    if (e.InnerException != null)
            //    {
            //        var ex = e.InnerException;
            //        var extype = ex.GetType();
            //        msgAction?.Invoke($"Method:{m} Type:{extype} {e.Message}");
            //        exAction?.Invoke(e);

            //    }
            //    return null;
            //}
            //catch (Exception e)
            //{
            //    var exctype = e.GetType();
            //    msgAction?.Invoke($"Method:{m} Type:{exctype} {e.Message}");
            //    exAction?.Invoke(e);
            //    return null;
            //}
        }
    }
}
