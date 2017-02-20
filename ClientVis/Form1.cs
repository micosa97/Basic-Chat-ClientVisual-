using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ClientVis
{
    public partial class Form1 : Form
    {
        Socket client;
        EndPoint remoteEP;
        byte[] data;
        Thread thread;


        private void donth ()
        {
            while (true) ;
        }
        public Form1()
        {
            InitializeComponent();
            thread = new Thread(donth);
            
        }


        delegate void StringArgReturningVoidDelegate(string text);

        private void AddText(string text)  // to avoid cross threading
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true.  
            if (this.richTextBox1.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(AddText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.richTextBox1.Text += text;
            }
        }



        public void Rec() //checking for messages
        {

            while (true)
            {
                int recv = client.ReceiveFrom(data, ref remoteEP);
                AddText(Encoding.ASCII.GetString(data, 0, recv) + "\r\n");
                //MessageBox.Show((Encoding.ASCII.GetString(data, 0, recv)));
               // richTextBox1.Text+= Encoding.ASCII.GetString(data, 0, recv) + "\r\n"; ;
               // Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv) + "\r\n");
            }
        }
        private void button1_Click(object sender, EventArgs e) //connect
        {
            thread.Abort(); //stop checking previous user if there was any
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1024);
            thread = new Thread(new ThreadStart(Rec));
            data = new byte[1024];
            thread.Start();

            client.SendTo(Encoding.ASCII.GetBytes("join"), remoteEP);
            client.SendTo(Encoding.ASCII.GetBytes(textBox1.Text), remoteEP);

            


        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.SendTo(Encoding.ASCII.GetBytes("quit"), remoteEP);
            client.SendTo(Encoding.ASCII.GetBytes(textBox1.Text), remoteEP);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            client.SendTo(Encoding.ASCII.GetBytes("message"), remoteEP);
            client.SendTo(Encoding.ASCII.GetBytes(textBox1.Text), remoteEP);  //ur name
            client.SendTo(Encoding.ASCII.GetBytes(textBox2.Text), remoteEP); //rec name
            client.SendTo(Encoding.ASCII.GetBytes(textBox3.Text), remoteEP); //mess
            AddText("You to " + textBox2.Text + ": " + textBox3.Text + "\r\n") ;


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
