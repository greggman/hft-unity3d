using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using DNS.Client;
using DNS.Client.RequestResolver;

namespace DNS.Server {
    public class DnsServer {
        private const int DEFAULT_PORT = 53;
        private const int UDP_TIMEOUT = 2000;
        private const int UDP_LIMIT = 512;

        public delegate void RequestedEventHandler(IRequest request);
        public delegate void RespondedEventHandler(IRequest request, IResponse response);

        private volatile bool run = true;

        private IQuestionAnswerer questionAnswerer;

        private UdpClient udp;
        private EventEmitter emitter;
        private DnsClient client;

        public event RequestedEventHandler Requested;
        public event RespondedEventHandler Responded;

        public DnsServer(IQuestionAnswerer questionAnswerer, IPEndPoint endServer = null) {
            this.emitter = new EventEmitter();
            if (endServer != null) {
                this.client = new DnsClient(endServer, new UdpRequestResolver());
            }
            this.questionAnswerer = questionAnswerer;
        }

        public DnsServer(IQuestionAnswerer questionAnswerer, IPAddress endServer, int port = DEFAULT_PORT) : this(questionAnswerer, new IPEndPoint(endServer, port)) {}
        public DnsServer(IQuestionAnswerer questionAnswerer, string endServerIp, int port = DEFAULT_PORT) : this(questionAnswerer, IPAddress.Parse(endServerIp), port) {}

        public void Listen(int port = DEFAULT_PORT) {
            udp = new UdpClient(port);

            IPEndPoint local = new IPEndPoint(IPAddress.Any, port);

            emitter.Run();
            udp.Client.SendTimeout = UDP_TIMEOUT;

            while (run) {
                byte[] clientMessage = null;

                try {
                    clientMessage = udp.Receive(ref local);
                } catch (SocketException) {
                    continue;
                }

                Thread task = new Thread(() => {
                    Request request = null;

                    try {
                        request = Request.FromArray(clientMessage);
                        emitter.Schedule(() => {
                            RequestedEventHandler handler = OnRequested;
                            if (handler != null) {
                                handler(request);
                            }
                        });

                        IResponse response = ResolveLocal(request);

                        emitter.Schedule(() => {
                            RespondedEventHandler handler = Responded;
                            if (handler != null) {
                                handler(request, response);
                            }
                        });
                        udp.Send(response.ToArray(), response.Size, local);
                    }
                    catch(SocketException) {}
                    catch(ArgumentException) {}
                    catch(ResponseException e) {
                        IResponse response = e.Response;

                        if (response == null) {
                            response = Response.FromRequest(request);
                        }

                        udp.Send(response.ToArray(), response.Size, local);
                    }
                });

                task.Start();
            }
        }

        public void Close() {
            if (udp != null) {
                run = false;

                emitter.Stop();
                udp.Close();
            }
        }

        protected virtual void OnRequested(IRequest request) {
            RequestedEventHandler handlers = Requested;
            if (handlers != null) handlers(request);
        }

        protected virtual void OnResponded(IRequest request, IResponse response) {
            RespondedEventHandler handlers = Responded;
            if (handlers != null) handlers(request, response);
        }

        protected virtual IResponse ResolveLocal(Request request) {
            Response response = Response.FromRequest(request);

            foreach (Question question in request.Questions) {
                IList<IResourceRecord> answers = questionAnswerer.Get(question);

                if (answers.Count > 0) {
                    Merge(response.AnswerRecords, answers);
                } else {
                    return ResolveRemote(request);
                }
            }

            return response;
        }

        protected virtual IResponse ResolveRemote(Request request) {
            ClientRequest remoteRequest = client.Create(request);
            return remoteRequest.Resolve();
        }

        private static void Merge<T>(IList<T> l1, IList<T> l2) {
            foreach (T obj in l2) {
                l1.Add(obj);
            }
        }
    }

    internal class EventEmitter {
        public delegate void Emit();

        private System.Object m_queueLock = new System.Object();
        private AutoResetEvent m_autoEvent = new AutoResetEvent(false);
        private List<Emit> m_queuedEvents = new List<Emit>();
        private List<Emit> m_executingEvents = new List<Emit>();
        private bool m_run = true;

        public void Schedule(Emit emit) {
            lock(m_queueLock) {
                m_queuedEvents.Add(emit);
            }
            m_autoEvent.Set();
        }

        public void Run() {
            (new Thread(() => {
                try {
                    while (m_run) {
                        m_autoEvent.WaitOne();
                        if (m_run) {
                            ProcessEvents();
                        }
                    }
                } catch (OperationCanceledException) { }
            })).Start();
        }

        public void Stop() {
            m_run = false;
            m_autoEvent.Set();
        }

        private void ProcessEvents() {
            if (m_executingEvents != null) {
                MoveQueuedEventsToExecuting();

                while (m_executingEvents.Count > 0) {
                    Emit emit = m_executingEvents[0];
                    m_executingEvents.RemoveAt(0);
                    emit();
                }
            }
        }

        private void MoveQueuedEventsToExecuting() {
            lock(m_queueLock) {
                while (m_queuedEvents.Count > 0) {
                    Emit e = m_queuedEvents[0];
                    m_executingEvents.Add(e);
                    m_queuedEvents.RemoveAt(0);
                }
            }
        }
    }
}
