﻿using System;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.soap;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs.jira {
    public sealed class TestJiraConnection : AbstractTestConnection {
        private readonly JiraServerFacade facade;
        private readonly JiraServer server;

        public TestJiraConnection(JiraServerFacade facade, JiraServer server) : base(server) {
            this.facade = facade;
            this.server = server;
        }

        public override void testConnection() {
            string result = "Connection to server successful";
            Exception ex = null;
            try {
                facade.login(server);
            } catch (Exception e) {
                ex = e; 
                result = "Failed to connect to to server";
            }
            this.safeInvoke(new MethodInvoker(() => stopTest(result, ex)));
        }
    }
}