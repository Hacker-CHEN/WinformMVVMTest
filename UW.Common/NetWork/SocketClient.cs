using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/*
 * ==========字段和属性==========
 * BufferSize:          常量，缓冲区大小为 4096 字节，用于读取和发送数据。
 * _encoding:           用于指定字符串的编码方式，这里使用 UTF8 编码。
 * _disposed:           一个布尔变量，用来标识该对象是否已被释放。
 * _lockObj:            一个锁对象，用于线程安全地处理资源的释放。
 * _clientSocket:       用来表示与服务器的套接字连接。
 * _buffer:             字节数组，作为数据接收时的缓冲区。
 * DataReceived:        事件，当从服务器接收到数据时触发。
 * ConnectionLost:      事件，当与服务器的连接丢失时触发。
 * 
 * ==========构造函数==========
 * SocketClient(string serverIp, int serverPort):
 * 构造函数通过服务器 IP 和端口创建一个套接字，并尝试连接到指定的服务器端点。
 * 调用 BeginConnect 异步方法，开始连接服务器，并指定回调方法 OnConnectCompleted。
 * 
 * ==========方法==========
 * OnConnectCompleted(IAsyncResult asyncResult):
 *      这是异步连接完成后的回调方法。
 *      调用 EndConnect 结束连接过程，成功后开始接收数据（通过调用 StartReceivingData）。
 *      如果连接失败，调用 OnConnectionLost 处理异常。
 *      
 * StartReceivingData():
 *      调用 BeginReceive 开始异步接收数据，指定缓冲区 _buffer、数据长度 BufferSize 和回调方法 OnReceiveCompleted。
 *      
 * OnReceiveCompleted(IAsyncResult asyncResult):
 *      当接收数据完成时调用此方法。
 *      调用 EndReceive 获取接收到的字节数 bytesRead。
 *      如果接收到的数据长度大于 0，则将数据转换为字符串并触发 DataReceived 事件，然后再次调用 StartReceivingData 继续接收。
 *      如果接收到的数据长度为 0，表示连接被服务器关闭，调用 OnConnectionLost 处理连接关闭情况。
 *      如果在接收数据的过程中发生异常，也会调用 OnConnectionLost。
 *      
 * Send(string data):
 *      该方法用于将字符串数据发送到服务器。
 *      将字符串 data 转换为字节数组后，调用 BeginSend 开始异步发送数据，发送完成后回调 OnSendCompleted。
 *      
 * OnSendCompleted(IAsyncResult asyncResult):
 *      当数据发送完成时调用此方法。
 *      调用 EndSend 确认发送完成。
 *      如果发送过程中发生异常，调用 OnConnectionLost 处理异常。
 *      
 * OnDataReceived(string data):
 *      当接收到数据时触发 DataReceived 事件，通知订阅此事件的外部方法。
 *      
 * OnConnectionLost(Exception ex):\
 *      连接丢失或发生异常时，触发 ConnectionLost 事件，通知订阅此事件的外部方法。
 *      
 * Dispose():
 *      实现 IDisposable 接口，用于释放资源。
 *      使用 lock (_lockObj) 确保资源释放时的线程安全性。
 *      调用 Shutdown 关闭套接字的发送和接收功能，然后关闭套接字。
 *      
 * ==========事件机制==========
 * DataReceived: 在接收到服务器发送的数据时触发。
 * ConnectionLost: 在连接丢失或发生异常时触发。
 * 
 * ==========调用方法举例==========
 * 
 * 1.创建并连接到服务器
 * SocketClient client = new SocketClient("192.168.1.100", 8080);
 *
 * 2.订阅接收到数据的事件
 * client.DataReceived += (data) =>
 * {
 *     Console.WriteLine("Received: " + data);
 * };
 *
 * 3.订阅连接丢失的事件
 * client.ConnectionLost += (exception) =>
 * {
 *     Console.WriteLine("Connection lost: " + exception.Message);
 * };
 *
 * 4.发送数据到服务器
 * client.Send("Hello, Server!");
 *
 * 5.模拟客户端运行，等待接收或发送更多数据
 * Console.WriteLine("Press Enter to exit.");
 * Console.ReadLine();
 *
 * 6. 释放资源，关闭连接
 * client.Dispose();
 */

namespace UW.Common.NetWork
{
    public class SocketClient : IDisposable
    {
        private const int BufferSize = 4096;

        private readonly Encoding _encoding = Encoding.UTF8;

        private bool _disposed;

        private readonly object _lockObj = new object();

        private readonly Socket _clientSocket;

        private readonly byte[] _buffer = new byte[4096];

        public event Action<string> DataReceived;

        public event Action<Exception> ConnectionLost;

        public SocketClient(string serverIp, int serverPort)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
            _clientSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _clientSocket.BeginConnect(endPoint, OnConnectCompleted, null);
        }

        private void OnConnectCompleted(IAsyncResult asyncResult)
        {
            try
            {
                _clientSocket.EndConnect(asyncResult);
                StartReceivingData();
            }
            catch (Exception ex)
            {
                OnConnectionLost(ex);
            }
        }

        private void StartReceivingData()
        {
            _clientSocket.BeginReceive(_buffer, 0, 4096, SocketFlags.None, OnReceiveCompleted, null);
        }

        private void OnReceiveCompleted(IAsyncResult asyncResult)
        {
            try
            {
                int bytesRead = _clientSocket.EndReceive(asyncResult);
                if (bytesRead > 0)
                {
                    string data = _encoding.GetString(_buffer, 0, bytesRead);
                    OnDataReceived(data);
                    StartReceivingData();
                }
                else
                {
                    OnConnectionLost(new Exception("The connection was closed by the server."));
                }
            }
            catch (Exception ex)
            {
                OnConnectionLost(ex);
            }
        }

        public void Send(string data)
        {
            byte[] buffer = _encoding.GetBytes(data);
            _clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, OnSendCompleted, null);
        }

        private void OnSendCompleted(IAsyncResult asyncResult)
        {
            try
            {
                _clientSocket.EndSend(asyncResult);
            }
            catch (Exception ex)
            {
                OnConnectionLost(ex);
            }
        }

        private void OnDataReceived(string data)
        {
            this.DataReceived?.Invoke(data);
        }

        private void OnConnectionLost(Exception ex)
        {
            this.ConnectionLost?.Invoke(ex);
        }

        public void Dispose()
        {
            lock (_lockObj)
            {
                if (!_disposed)
                {
                    _disposed = true;
                    _clientSocket.Shutdown(SocketShutdown.Both);
                    _clientSocket.Close();
                }
            }
        }
    }
}
