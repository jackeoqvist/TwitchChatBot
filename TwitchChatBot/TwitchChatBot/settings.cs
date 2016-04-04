using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchChatBot.Properties;

namespace TwitchChatBot
{
    public partial class settings : Form
    {

        string username, channelname, key;

        public settings()
        {
            InitializeComponent();

            textBox1.Text = Settings.Default.UserName;
            textBox2.Text = Settings.Default.ChannelName;
            textBox3.Text = Settings.Default.oathkey;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.UserName = textBox1.Text;
            Settings.Default.ChannelName = textBox2.Text;
            Settings.Default.oathkey = textBox3.Text;
            Settings.Default.Save();

            this.Close();
        }
    }
}
