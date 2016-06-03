using DeJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyFunTimes {

//    public class HTFEventDispatcher {
//
//        public delegate void UntypedCmdEventHandler(Dictionary<string, object> data);
//        public delegate void TypedCmdEventHandler<T>(T eventArgs);
//        private delegate bool CmdEventHandler(Deserializer deserialzier, Dictionary<string, object> dict);
//
//        public void RegisterCmdHandler<T>(string name, TypedCmdEventHandler<T> callback) {
//            CmdConverter<T> converter = new CmdConverter<T>(log_, callback);
//            messageHandlers_[name] = converter.Callback;
//        }
//
//        public bool DispatchMessage(string msg) {
//            MiniMsg message = null;
//            try
//            {
//                message = deserializer_.Deserialize<MiniMsg>(msg);
//
//            }
//            catch (System.Exception ex)
//            {
//                log_.Error(ex.ToString());
//                return false;
//            }
//
//            var handler = messageHandlers_[message.cmd];
//            if (handler == null)
//            {
//              log_.Error("unknown player message: " + message.cmd);
//              return false;
//            }
//
//            return handler(deserializer_, message.data);
//        }
//
//
//        private class CmdConverter<T>
//        {
//            public CmdConverter(HFTLog log, TypedCmdEventHandler<T> handler) {
//                m_log = log;
//                m_handler = handler;
//            }
//
//            public bool Callback(Deserializer deserializer, Dictionary<string, object> dict) {
//                try
//                {
//                    T data = deserializer.Deserialize<T>(dict);
//                    m_handler(data);
//                    return true;
//                }
//                catch (System.Exception ex)
//                {
//                    m_log.Error(ex.ToString());
//                    return false;
//                }
//            }
//
//            TypedCmdEventHandler<T> m_handler;
//            HFTLog m_log;
//        }
//
//        private class UntypedCmdConverter {
//            public UntypedCmdConverter(HFTLog log, UntypedCmdEventHandler handler) {
//                m_log = log;
//                m_handler = handler;
//            }
//
//            public bool Callback(Deserializer deserializer, Dictionary<string, object> dict) {
//                try
//                {
//                    m_handler(dict);
//                    return true;
//                }
//                catch (System.Exception ex)
//                {
//                    m_log.Error(ex.ToString());
//                    return false;
//                }
//            }
//
//            HFTLog m_log;
//            UntypedCmdEventHandler m_handler;
//        }
//
//        private class MiniMsg {
//            public string cmd;
//            public Dictionary<string, object> data;
//        };
//
//        Deserializer deserializer_;
//        HFTLog log_;
//        Dictionary<string, CmdEventHandler> messageHandlers_;
//    }

}  // namespace HappyFunTimes

