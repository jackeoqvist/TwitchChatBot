﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using TwitchChatBot.Properties;

namespace TwitchChatBot
{
    public partial class Form1 : Form
    {
        TcpClient tcpClient;
        StreamReader reader;
        StreamWriter writer;
        readonly string userName, password, channelName, chatCommandId, chatMessagePrefix;

        public Form1()
        {
            this.userName = Settings.Default.UserName.ToLower();
            this.channelName = Settings.Default.ChannelName.ToLower();
            this.password = Settings.Default.oathkey;
            chatCommandId = "PRIVMSG";
            chatMessagePrefix = $":{userName}!{userName}@{userName}.tmi.twitch.tv {chatCommandId} #{channelName} :";

            InitializeComponent();
            Reconnect();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings s = new settings();
            s.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SendMessage(textBox2.Text);
            }
            catch { }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                try
                {
                    SendMessage(textBox2.Text);
                }
                catch { }
            }
        }

        private void Reconnect()
        {
            tcpClient = new TcpClient("irc.chat.twitch.tv", 6667);
            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream());

            writer.WriteLine("PASS " + password + Environment.NewLine
                + "NICK " + userName + Environment.NewLine
                + "USER " + userName + " 8 * :" + userName);
            writer.WriteLine("CAP REQ :twitch.tv/membership");
            writer.WriteLine("JOIN #swejacke");
            writer.Flush();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!tcpClient.Connected)
            {
                Reconnect();
            }

            if (tcpClient.Available > 0 || reader.Peek() >= 0)
            {
                var message = reader.ReadLine();

                var iCollon = message.IndexOf(":", 1);
                if (iCollon > 0)
                {
                    var command = message.Substring(1, iCollon);
                    if (message.Contains(chatCommandId))
                    {
                        var iBang = command.IndexOf("!");
                        if(iBang > 0)
                        {
                            var speaker = command.Substring(0, iBang);
                            var chatMessage = message.Substring(iCollon + 1);

                            ReceiveMessage(speaker, chatMessage);
                        }
                    }
                }
            }
        }

        void ReceiveMessage(string speaker, string message)
        { 
            textBox1.AppendText($"{speaker}: {message}\r\n");

            if (message.StartsWith("!hi"))
            {
                SendMessage($"Hello, {speaker}");
            }
            else if (message.StartsWith("!help"))
            {
                SendMessage($"Commands: !help, Shows this; !hi, says hello to (Speaker)");
            }
        }

        private void SendMessage(string message)
        {
            writer.WriteLine($"{chatMessagePrefix}{message}");
            writer.Flush();
        }
    }
}
