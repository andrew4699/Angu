using System;
using System.Collections.Generic;
using System.Text;

namespace Angu
{
    class Message
    {
        public const int NEXT = 1;
        public const int FINISHED = 2;
        public const int TEXT = 3;
        public const int FILE = 4;
        public const int IMAGE = 5;

        public const int STATE_IDLE = 0;
        public const int STATE_NEXT = 1;
        public const int STATE_FINISHED = 2;
        public const int STATE_WAITING = 3;

        public static List<Message> queue = new List<Message>();

        public int type;
        private byte[] data;

        private bool headerSent;

        public Message(int initType, byte[] initData)
        {
            this.type = initType;
            this.data = initData;

            this.headerSent = false;
        }

        public byte[] popData() // Return the next part of the message and remove it from the string
        {
            byte[] result = new byte[Communication.MAX_SEND_LENGTH];

            // Header
            string header = "";

            if(!this.headerSent)
            {
                header = this.type + "m" + this.data.Length + "b";

                this.headerSent = true;
            }

            byte[] bHeader = Encoding.ASCII.GetBytes(header);
            Buffer.BlockCopy(bHeader, 0, result, 0, bHeader.Length);

            // Data
            if (this.data.Length > Communication.MAX_SEND_LENGTH - bHeader.Length)
            {
                int dataLen = Communication.MAX_SEND_LENGTH - bHeader.Length;

                Buffer.BlockCopy(Utils.SubArray(this.data, 0, dataLen), 0, result, bHeader.Length, dataLen); // Add popped part to result
                this.data = Utils.SubArray(this.data, dataLen, this.data.Length - dataLen); // Remove popped part from data
                return result;
            }
            else
            {
                Buffer.BlockCopy(this.data, 0, result, bHeader.Length, this.data.Length); // Add remaining part to result, no more data
                //Buffer.BlockCopy(Encoding.ASCII.GetBytes("$"), 0, result, bHeader.Length + this.data.Length, 1); // Add sentinel
                return Utils.SubArray(result, 0, bHeader.Length + this.data.Length + 1);
            }
        }
    }
}
