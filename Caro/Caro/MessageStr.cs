using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Caro
{
    public class MessageStr: ISerializable
    {
        
        private string _playername;

        public string Playername
        {
            get { return _playername; }
            set { _playername = value; }
        }
        private string _textchat;

        public string Textchat
        {
            get { return _textchat; }
            set { _textchat = value; }
        }

        private Point _endpoint;

        public Point Endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }

        public MessageStr()
        {
            this.Playername = null;
            this.Textchat = null;
            this.Endpoint = new Point(-1, -1);
            this.Canplay = false;
        }

        public MessageStr(string playername, string chat, Point endpoint, bool canplay)
        {
            this.Playername = playername;
            this.Textchat = chat;
            this.Endpoint = endpoint;
            this.Canplay = canplay;
        }

        public MessageStr(MessageStr mstr)
        {
            this.Playername = mstr.Playername;
            this.Textchat = mstr.Textchat;
            this.Endpoint = mstr.Endpoint;
            this.Canplay = mstr.Canplay;
        }

        private bool _canplay;

        public bool Canplay
        {
            get { return _canplay; }
            set { _canplay = value; }
        }

        //serialize
        public void GetObjectData(SerializationInfo info, StreamingContext strct)
        {
            info.AddValue("name", this.Playername);
            info.AddValue("chat", this.Textchat);
            info.AddValue("Endpoint", this.Endpoint);
            info.AddValue("Canplay", this.Canplay);
        }

        //Deserialize
        public MessageStr(SerializationInfo info, StreamingContext strct)
        {
            this.Playername = (string)info.GetValue("name", typeof(string));
            this.Textchat = (string)info.GetValue("chat", typeof(string));
            this.Endpoint = (Point)info.GetValue("Endpoint", typeof(Point));
            this.Canplay = (bool)info.GetValue("Canplay", typeof(bool));
        }
    
    }
}
