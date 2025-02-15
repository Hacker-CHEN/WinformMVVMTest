using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/*
 主要功能说明：
        监听客户端连接：服务端使用 Socket.Listen 方法来开始监听指定端口，并使用 BeginAccept 方法异步接受客户端的连接。
        处理客户端连接：当客户端连接成功后，调用 OnClientConnected 方法，将客户端的 Socket 保存在 _clientSockets 列表中，以便进行后续操作。
        接收和发送数据：服务端和客户端类似，使用 BeginReceive 和 BeginSend 来处理数据的异步接收和发送。接收到的数据会通过 DataReceived 事件通知外部处理。
        处理客户端断开：如果客户端断开连接或者发生异常，会调用 OnClientDisconnected 方法来通知外部并关闭客户端的 Socket。
        资源清理：服务端实现了 IDisposable 接口，确保在服务端停止时，释放所有的资源，包括关闭所有与客户端的连接以及停止监听。
事件：
        DataReceived：当服务端接收到客户端的数据时，触发该事件，通知外部处理数据。
        ClientDisconnected：当客户端断开连接或出现异常时，触发该事件，通知外部处理断开情况。

使用方法：
        var server = new SocketServer("127.0.0.1", 8080);
        server.DataReceived += (data) =>
        {
            Console.WriteLine("Received: " + data);
            // 可以根据需要处理数据并响应客户端
        };
        server.ClientDisconnected += (ex) =>
        {
            Console.WriteLine("Client disconnected: " + ex.Message);
        };
 */

namespace UW.Common.NetWork
{
    public class SocketServer : IDisposable
    {
        private const int BufferSize = 4096;

        private readonly Encoding _encoding = Encoding.UTF8;

        private bool _disposed;

        private readonly object _lockObj = new object();

        private readonly Socket _listenerSocket;

        private readonly List<Socket> _clientSockets = new List<Socket>();

        private readonly byte[] _buffer = new byte[BufferSize];

        public event Action<string> DataReceived;

        public event Action<Exception> ClientDisconnected;

        public SocketServer(string ipAddress, int port)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            _listenerSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenerSocket.Bind(endPoint);
            _listenerSocket.Listen(10);  // 最大监听10个客户端
            StartAcceptingClients();
        }

        private void StartAcceptingClients()
        {
            _listenerSocket.BeginAccept(OnClientConnected, null);
        }

        private void OnClientConnected(IAsyncResult asyncResult)
        {
            try
            {
                Socket clientSocket = _listenerSocket.EndAccept(asyncResult);
                lock (_clientSockets)
                {
                    _clientSockets.Add(clientSocket);
                }
                StartReceivingData(clientSocket);
                StartAcceptingClients(); // 继续接受下一个客户端连接
            }
            catch (Exception ex)
            {
                OnClientDisconnected(ex);
            }
        }

        private void StartReceivingData(Socket clientSocket)
        {
            clientSocket.BeginReceive(_buffer, 0, BufferSize, SocketFlags.None, OnReceiveCompleted, clientSocket);
        }

        private void OnReceiveCompleted(IAsyncResult asyncResult)
        {
            Socket clientSocket = (Socket)asyncResult.AsyncState;
            try
            {
                int bytesRead = clientSocket.EndReceive(asyncResult);
                if (bytesRead > 0)
                {
                    string data = _encoding.GetString(_buffer, 0, bytesRead);
                    OnDataReceived(data);
                    StartReceivingData(clientSocket);  // 继续接收该客户端发送的数据
                }
                else
                {
                    OnClientDisconnected(new Exception("Client disconnected."));
                    CloseClientSocket(clientSocket);
                }
            }
            catch (Exception ex)
            {
                OnClientDisconnected(ex);
                CloseClientSocket(clientSocket);
            }
        }

        public void Send(Socket clientSocket, string data)
        {
            byte[] buffer = _encoding.GetBytes(data);
            clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, OnSendCompleted, clientSocket);
        }

        private void OnSendCompleted(IAsyncResult asyncResult)
        {
            try
            {
                Socket clientSocket = (Socket)asyncResult.AsyncState;
                clientSocket.EndSend(asyncResult);
            }
            catch (Exception ex)
            {
                OnClientDisconnected(ex);
            }
        }

        private void OnDataReceived(string data)
        {
            this.DataReceived?.Invoke(data);
        }

        private void OnClientDisconnected(Exception ex)
        {
            this.ClientDisconnected?.Invoke(ex);
        }

        private void CloseClientSocket(Socket clientSocket)
        {
            lock (_clientSockets)
            {
                if (_clientSockets.Contains(clientSocket))
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    _clientSockets.Remove(clientSocket);
                }
            }
        }

        public void Dispose()
        {
            lock (_lockObj)
            {
                if (!_disposed)
                {
                    _disposed = true;
                    foreach (var clientSocket in _clientSockets)
                    {
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                    }
                    _listenerSocket.Close();
                }
            }
        }
    }

}
