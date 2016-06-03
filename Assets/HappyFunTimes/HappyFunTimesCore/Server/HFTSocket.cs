using DeJson;
using UnityEngine;
using System;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace HappyFunTimes
{

    public class HFTSocket : WebSocketBehavior
    {

        public HFTSocket()
        {
            log_ = new HFTLog("hftsocket");
        }

        protected override void OnOpen()
        {
            HFTGameManager.GetInstance().AddPlayer(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            log_.Info("closed");
            EventHandler<CloseEventArgs> handler = OnCloseEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnError(ErrorEventArgs e)
        {
            if (!closed_)
            {
                log_.Error("error: " + e.ToString() + ": " + e.Message);
                try
                {
                    Close();
                }
                catch (System.Exception)
                {
                }
            }
            EventHandler<ErrorEventArgs> handler = OnErrorEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
Debug.Log("rcv:" + e.Data);
            EventHandler<MessageEventArgs> handler = OnMessageEvent;
            if (handler != null)
            {
                try
                {
                    handler(this, e);
                }
                catch (System.Exception ex)
                {
                    log_.Error(ex.ToString());
                }
            }
        }

        new private void Send(string msg)
        {

        }

        public void Send(object msg)
        {
            if (!closed_)
            {
                string str = Serializer.Serialize(msg);
Debug.Log("snd:" + str);
                base.Send(str);
            }
        }

        public void Close()
        {
            if (!closed_)
            {
                closed_ = true;
                base.Context.WebSocket.Close();
            }
        }

        bool closed_ = false;
        HFTLog log_;

        public event EventHandler<MessageEventArgs> OnMessageEvent;
        public event EventHandler<CloseEventArgs> OnCloseEvent;
        public event EventHandler<ErrorEventArgs> OnErrorEvent;
    }

}  // namespace HappyFunTimes


