using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Atlassian.plvs.api;

namespace Atlassian.plvs.dialogs {
    public partial class IssueWorkflowAction : Form {
        public IssueWorkflowAction(List<JiraField> fields) {
            InitializeComponent();
        }
    }
}
