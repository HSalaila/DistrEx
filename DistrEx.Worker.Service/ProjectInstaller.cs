﻿using System.ComponentModel;
using System.Configuration.Install;

namespace DistrEx.Worker.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            //ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            //ServiceInstaller serviceInstaller = new ServiceInstaller();

            //Service Account Information 
            //serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            //serviceProcessInstaller.Username = null;
            //serviceProcessInstaller.Password = null; 

            //Service Inforamtion 
            //serviceInstaller.DisplayName = "WorkerServiceDisplayName";
            //serviceInstaller.StartType = ServiceStartMode.Automatic;
            //serviceInstaller.ServiceName = "WorkerServiceName";

            //Installers.Add(serviceProcessInstaller); 
            //Installers.Add(serviceInstaller);
        }
    }
}
