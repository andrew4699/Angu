using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Angu
{
    class Communication
    {
        private const string SERVER_IP = "192.168.0.109";
        private const int SERVER_PORT = 8888;
        private const int RECONNECT_INTERVAL = 5000;

        public const int MAX_SEND_LENGTH = 32767;
        //public const int MAX_SEND_LENGTH = 10;

        //private static System.Windows.Forms.Timer reconnectTimer;

        private static TcpClient socket;
        private static TcpListener listener;

        private static Thread readThread;
        private static Thread sendThread;
        private static Thread serverThread;

        public static int messageState = Message.STATE_IDLE;

        public static void initialize()
        {
            /*reconnectTimer = new System.Windows.Forms.Timer();
            reconnectTimer.Interval = RECONNECT_INTERVAL;
            reconnectTimer.Tick += connect;
            reconnectTimer.Start();*/

            socket = new TcpClient();
            connect();
        }

        private static void connect()
        {
            List<string> userIPs = new List<string>();

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach(var ip in host.AddressList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    userIPs.Add(ip.ToString());
                }
            }

            for (int i = 102; i <= 112; i++) // Test 192.168.0.102 - 192.168.0.112
            {
                string ip = "192.168.0." + i;

                if (userIPs.Contains(ip)) // Don't check own IP
                {
                    continue;
                }

                try
                {
                    Utils.Log("Pinging " + ip);

                    byte[] pingData = new byte[32];
                    (new Random()).NextBytes(pingData);
                    PingReply reply = (new Ping()).Send(ip, 1, pingData);

                    if(reply.Status != IPStatus.Success)
                    {
                        //Utils.Log("Ping failed");
                        continue;
                    }

                    Utils.Log("Attempting to connect to "+ ip + " on port " + SERVER_PORT);
                    socket.Connect(ip, SERVER_PORT);

                    socket.ReceiveBufferSize = MAX_SEND_LENGTH * 8;
                    socket.SendBufferSize = MAX_SEND_LENGTH * 8;
                    Utils.Log("receive: " + socket.ReceiveBufferSize + ", send: " + socket.SendBufferSize);
                    Utils.Log("Connection successful, starting client threads...");
                    Notification.create("Connection established");
                    startClientThreads();
                    return;
                }
                catch(Exception ex)
                {
                    Utils.Log(ex.ToString());
                }
            }

            listener = new TcpListener(IPAddress.Any, SERVER_PORT);
            listener.Start();

            Utils.Log("Server started");
            Notification.create("Server started");

            serverThread = new Thread(handleClient);
            serverThread.Start();
        }

        private static void startClientThreads()
        {
            readThread = new Thread(handleMessages);
            readThread.Start();

            sendThread = new Thread(sendMessages);
            sendThread.Start();
        }

        private static void handleMessages()
        {
            NetworkStream stream = socket.GetStream();
            List<byte> data = new List<byte>();

            byte[] preTempData = new byte[MAX_SEND_LENGTH];
            List<byte> tempData = new List<byte>();

            int messageType = -1;
            int messageLength = -1;

            while (socket.Connected)
            {
                try
                {
                    if (stream.DataAvailable)
                    {
                        Utils.Log("recv buffer size: " + socket.ReceiveBufferSize);
                        stream.Read(preTempData, 0, MAX_SEND_LENGTH);
                        Utils.Log("op: " + Utils.ByteArrayToString(preTempData));
                        tempData = preTempData.ToList();

                        if (messageType == -1) // Beginning of message
                        {
                            Utils.Log("-- BEGINNING --");
                            data.Clear();
                        }
                        
                        for (int i = 0; i < tempData.Count; i++)
                        {
                            if (messageType == -1 && tempData[i] == 'm')
                            {
                                //Utils.Log("removing " + Utils.ByteArrayToString(Utils.SubArray(tempData.ToArray(), 0, i + 1)));
                                messageType = Convert.ToInt32(Encoding.ASCII.GetString(tempData.Take(i).ToArray()));

                                tempData.RemoveRange(0, i + 1);
                                Utils.Log("found messageType (i = " + i + "): " + messageType);
                                i = -1;
                            }
                            else if (messageLength == -1 && tempData[i] == 'b')
                            {
                                messageLength = Convert.ToInt32(Encoding.ASCII.GetString(tempData.Take(i).ToArray()));
                                messageLength = messageLength + messageType.ToString().Length + messageLength.ToString().Length + 2;

                                tempData.RemoveRange(0, i + 1);
                                Utils.Log("found messageLength (i = " + i + "): " + messageLength);
                                i = -1;
                            }
                        }

                        //Utils.Log("received file: " + Utils.ByteArrayToString(tempData.ToArray()));
                        if (messageLength <= MAX_SEND_LENGTH) // If an end delimeter is found, this message is finished
                        {
                            if (tempData.Count - messageLength > 0)
                                tempData.RemoveRange(messageLength, tempData.Count - messageLength);

                            data.AddRange(tempData);

                            //Utils.Log("[FINAL SET] " + Encoding.ASCII.GetString(tempData.ToArray()));

                            Utils.Log("[TYPE = " + messageType + "] " + Encoding.ASCII.GetString(data.ToArray()));
                            //Utils.Log("[TYPE = " + messageType + "] " + Utils.ByteArrayToString(data.ToArray()));
                            Program.anguMain.onReceiveMessage(messageType, data.ToArray()); // Process the message

                            if (messageType != Message.NEXT && messageType != Message.FINISHED)
                                queueMessage(Message.FINISHED); // Tell the other client that we're finished

                            messageType = -1;
                            messageLength = -1;
                        }
                        else
                        {
                            Utils.Log("[NEXT] " + Encoding.ASCII.GetString(tempData.ToArray()));

                            data.AddRange(tempData);

                            messageLength -= MAX_SEND_LENGTH;

                            if (messageType != Message.NEXT && messageType != Message.FINISHED)
                                queueMessage(Message.NEXT); // Ask for more data
                        }
                    }
                }
                catch(Exception ex)
                {
                    Utils.Log("handleMessages error: " + ex.ToString());
                }
            }

            Utils.Log("=== disconnect");
        }

        public static void sendImage(string filePath)
        {
            Utils.Log("sent image: " + Utils.ByteArrayToString(File.ReadAllBytes(filePath)));
            queueMessage(Message.IMAGE, File.ReadAllBytes(filePath));
        }

        public static void sendFile(string filePath)
        {
            Utils.Log("sent file: " + Utils.ByteArrayToString(File.ReadAllBytes(filePath)));
            queueMessage(Message.FILE, File.ReadAllBytes(filePath));
        }

        public static void queueMessage(int messageType, byte[] data = null)
        {
            if(data == null)
            {
                data = new byte[0];
            }

            Message.queue.Add(new Message(messageType, data));
        }

        private static void sendMessages()
        {
            NetworkStream stream = socket.GetStream();
            byte[] data;
            
            while (true)
            {
                //Utils.Log("sendMessages, state = " + messageState);
                if(messageState == Message.STATE_IDLE) // Await next message in queue
                {
                    if(Message.queue.Count > 0)
                    {
                        messageState = Message.STATE_NEXT;
                    }
                }
                else if(messageState == Message.STATE_FINISHED) // Remove the message from the queue, ready for next
                {
                    //Utils.Log("finished, " + Message.queue.Count + " messages left in queue");
                    Message.queue.RemoveAt(0);
                    messageState = Message.STATE_IDLE;
                }
                else if(messageState == Message.STATE_NEXT) // Client is requesting more data
                {
                    data = Message.queue[0].popData();
                    Utils.Log("Processing next message (len = " + data.Length + "): " + Encoding.ASCII.GetString(data));

                    stream.Write(data, 0, data.Length);

                    if(Message.queue[0].type == Message.NEXT || Message.queue[0].type == Message.FINISHED)
                        messageState = Message.STATE_FINISHED;
                    else
                        messageState = Message.STATE_WAITING;
                }
            }
        }

        private static void handleClient()
        {
            while(!socket.Connected)
            {
                socket = listener.AcceptTcpClient();
                Utils.Log("Client connected");
                socket.ReceiveBufferSize = MAX_SEND_LENGTH * 8;
                socket.SendBufferSize = MAX_SEND_LENGTH * 8;
                Utils.Log("receive: " + socket.ReceiveBufferSize + ", send: " + socket.SendBufferSize);

                Program.anguMain.BeginInvoke((Action)delegate
                {
                    Notification.create("Connection established");
                });

                startClientThreads();
                break;
            }

            #pragma warning disable
            //socket.Close();
            #pragma warning restore

            //listener.Stop();

            Utils.Log("Server stopped");
        }
    }
}
