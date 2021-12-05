using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebSocket4Net;

namespace DEFI.haram
{
    public class Haram2
    {

        internal readonly string url;
        internal bool IsConnected = false;

        internal Haram2(string url)
        {
            this.url = url;
        }

        WebSocket4Net.WebSocket Socket;
        internal void Start()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            this.Socket = new WebSocket4Net.WebSocket(this.url);
            this.Socket.Security.AllowUnstrustedCertificate = true;
            this.Socket.Security.AllowNameMismatchCertificate = true;
            this.Socket.Security.AllowCertificateChainErrors = true;
            Socket.Opened += new EventHandler(OpenEventHandler);
            Socket.Error += new EventHandler<ErrorEventArgs>(ErrorEventHandler);
            Socket.Closed += new EventHandler(CloseEventHandler);
            Socket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(MessageReceivedEventHandler);
            Socket.Open();
            while (true)
            {
                if (IsConnected) return;
                System.Threading.Thread.Sleep(1000);
            }
        }

        public async void SubscribeBlockHeaders(Action<BlockHeaderResult> action)
        {
            try
            {
                var postObj = new
                {
                    @jsonrpc = "2.0",
                    @id = 1,
                    @method =
                "eth_subscribe",
                    @params = new object[] {
                    "newHeads"
                }
                };
                string body = JsonConvert.SerializeObject(postObj);
                SendRequest(body);
                string subscriptionId = GetNextSubscriptionId();

                SubscriptionDictionary[subscriptionId] = new Action<object>((model) =>
                {
                    string modelString = model.ToString();
                    BlockHeaderResult modelObj = JsonConvert.DeserializeObject<BlockHeaderResult>(modelString);
                    action(modelObj);
                });
                Console.WriteLine("Subscribed");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception");
            }
        }

        public void SubscribePendingTransactions(Action<string> action)
        {
            try
            {
                var postObj = new
                {
                    @jsonrpc = "2.0",
                    @id = 1,
                    @method =
                    "eth_subscribe",
                    @params = new object[] { "newPendingTransactions" }
                };
                string body = JsonConvert.SerializeObject(postObj);
                SendRequest(body);
                string subscriptionId = GetNextSubscriptionId();
                SubscriptionDictionary[subscriptionId] = new Action<object>((model) =>
                {
                    string modelString = model.ToString();
                    action(modelString);
                });
                Console.WriteLine("Subscribed");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception");
            }
        }

        public void SendRequest(string data)
        {
            Socket.Send(data);
        }

        Dictionary<string, Action<object>> SubscriptionDictionary = new Dictionary<string, Action<object>>();

        internal void ErrorEventHandler(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("Error: " + e.Exception.Message);
        }

        internal string GetNextSubscriptionId()
        {
            string foundSubId = null;
            Task task = Task.Run(() =>
            {
                while (true)
                {
                    lock (NewSubscriptionObject)
                    {
                        string id = this.NewSubscriptionIds.FirstOrDefault();
                        if (id != null)
                        {
                            this.NewSubscriptionIds.RemoveAt(0);
                            foundSubId = id;
                            return;
                        }
                    }
                    System.Threading.Thread.Sleep(100);
                }
            });
            Task.WaitAll(task);
            return foundSubId;
        }

        internal object NewSubscriptionObject = new object();
        internal List<string> NewSubscriptionIds = new List<string>();
        internal Action<object, ErrorEventArgs> ErrorHandler = null;
        internal Action<object, EventArgs> CloseHandler = null;

        internal void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e)
        {
            BaseMessageResult obj = JsonConvert.DeserializeObject<BaseMessageResult>(e.Message);
            if (!String.IsNullOrEmpty(obj?.Params?.subscription))
            {
                string sub = obj.Params.subscription;
                if (!this.SubscriptionDictionary.ContainsKey(sub)) { return; }
                Action<object> action = this.SubscriptionDictionary[sub];
                action(obj.Params.result);
                return;
            }
            else if (!String.IsNullOrEmpty(obj?.result?.ToString()))
            {
                string subscriptionId = (string)obj.result;
                lock (NewSubscriptionObject) this.NewSubscriptionIds.Add(subscriptionId);
                return;
            }
            return;
        }

        public class BaseMessageResult
        {
            public string jsonrpc { get; set; }
            public string method { get; set; }
            public object result { get; set; }
            public ParamsStringObject Params { get; set; }
        }

        public class ParamsStringObject
        {
            public string subscription { get; set; }
            public object result { get; set; }
        }

        internal void CloseEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine("CloseEventHandler");
            Console.WriteLine(JsonConvert.SerializeObject(sender));
            Console.WriteLine(JsonConvert.SerializeObject(e));
            if (this.CloseHandler != null)
            {
                CloseHandler(sender, e);
            }
        }

        internal void OpenEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine("OpenEventHandler");
            this.IsConnected = true;
        }

        public void Close()
        {
            Console.WriteLine("Close called");
            Socket.Close();
        }

        public class BlockHeaderResult
        {
            public string parentHash { get; set; }
            public string sha3Uncles { get; set; }
            public string miner { get; set; }
            public string stateRoot { get; set; }
            public string transactionsRoot { get; set; }
            public string receiptsRoot { get; set; }
            public string logsBloom { get; set; }
            public string difficulty { get; set; }
            public string number { get; set; }
            public string gasLimit { get; set; }
            public string gasUsed { get; set; }
            public string timestamp { get; set; }
            public string extraData { get; set; }
            public string mixHash { get; set; }
            public string nonce { get; set; }
            public string hash { get; set; }
            public string baseFeePerGas { get; set; }
        }

    }
}
