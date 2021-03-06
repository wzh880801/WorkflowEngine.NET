﻿using OptimaJet.Workflow.Core.Builder;
using OptimaJet.Workflow.Core.Bus;
using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Runtime;
using OptimaJet.Workflow.MySQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WorkflowApp
{
    public class WorkflowInit
    {
        private static volatile WorkflowRuntime _runtime;
        private static readonly object _sync = new object();

        public static WorkflowRuntime Runtime
        {
            get
            {
                if (_runtime == null)
                {
                    lock (_sync)
                    {
                        if (_runtime == null)
                        {
                            //TODO CONNECTION STRING TO DATABASE
                            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                            var provider = new MySQLProvider(connectionString);
                            var builder = new WorkflowBuilder<XElement>(
                                provider,
                                new OptimaJet.Workflow.Core.Parser.XmlWorkflowParser(),
                                provider
                                ).WithDefaultCache();

                            _runtime = new WorkflowRuntime()
                                .WithBuilder(builder)
                                .WithPersistenceProvider(provider)
                                .WithTimerManager(new TimerManager())
                                .WithBus(new NullBus())
                                .SwitchAutoUpdateSchemeBeforeGetAvailableCommandsOn()
                                .Start();
                        }
                    }
                }

                return _runtime;
            }
        }
    }
}
