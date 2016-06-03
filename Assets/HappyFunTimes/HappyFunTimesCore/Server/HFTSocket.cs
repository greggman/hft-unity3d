using DeJson;
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
            base.IgnoreExtensions = true;
            log_ = new HFTLog("HFTSocket");
        }

        protected override void OnOpen()
        {
            log_.Info("open");
            try
            {
                HFTGameManager.GetInstance().AddPlayer(this);
            }
            catch (System.Exception ex)
            {
                log_.Error(ex.StackTrace);
            }
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
            log_.Error("error: " + e.ToString() + ": " + e.Message);
            if (!closed_)
            {
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
//Debug.Log("rcv:" + e.Data);
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
                    log_.Error(ex.StackTrace);
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
//Debug.Log("snd:" + str);
                base.Send(str);
            }
        }

        public void Close()
        {
            if (!closed_)
            {
                closed_ = true;
                Sessions.CloseSession(ID);
            }
        }

        bool closed_ = false;
        HFTLog log_;

        public event EventHandler<MessageEventArgs> OnMessageEvent;
        public event EventHandler<CloseEventArgs> OnCloseEvent;
        public event EventHandler<ErrorEventArgs> OnErrorEvent;
    }

}  // namespace HappyFunTimes


