using com.DigitalLudens.Network.Socket;
using System;
using System.IO;
using System.Net;
using UnityEngine;

namespace beio.Network.Socket
{
	public abstract class SocketNetwork : MonoBehaviour
    {
        private static SocketNetwork _instance = null;
        public static SocketNetwork Instance => _instance;
        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            OnInitialize();
        }
        protected SocketIO socketIO = null;
        protected virtual void OnInitialize()
        {
            socketIO = new SocketIO(OnConnect, OnSocketClose, OnDataReceived, OnError);
        }
        protected void OnDestroy()
        {
            if(socketIO?.IsConnected ?? false)
                socketIO.Disconnect();
            socketIO = null;
        }

        public bool Connect(string host, int port)
        {
            if (socketIO == null)
            {
                UnityEngine.Debug.Log("Socket is null");
                return false;
            }
            if (socketIO.IsConnected)
            {
                UnityEngine.Debug.Log("Socket is connected");
                return false;
            }
            try
            {
                socketIO.Connect(new DnsEndPoint(host, port));
                return true;
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            return false;
        }
        protected abstract void OnConnect(object sender, EventArgs args);
        protected abstract void OnSocketClose(object sender, EventArgs args);
        protected abstract void OnError(object sender, ErrorEventArgs args);
        protected abstract void OnDataReceived(object sender, DataEventArgs args);
    }

}