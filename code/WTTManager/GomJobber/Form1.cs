using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GomJobber
{
    public partial class Form1 : Form
    {
        public IEnumerable<JobStatus> jobStatuses;

        public Form1()
        {
            InitializeComponent();

            WTTManager wttManager = new WTTManager();
            jobStatuses = wttManager.GetJobStatuses(Environment.UserName);

            this.jobStatusBindingSource.DataSource = jobStatuses;
        }
    }
}
