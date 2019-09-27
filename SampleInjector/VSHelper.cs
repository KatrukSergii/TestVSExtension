using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using System;
using System.Runtime.InteropServices;

namespace SampleInjector
{
    public class VSHelper
    {
        private readonly IComponentModel componentModel;

        private readonly DTE dte;

        public VSHelper(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider)
        {
            this.componentModel = serviceProvider.GetServiceAsync(typeof(SComponentModel)).Result as IComponentModel; 
            this.dte = serviceProvider.GetServiceAsync(typeof(DTE)).Result as DTE;
        }

        public void SetupNugetPakadge(Project project, string packageName)
        {
            var packageInstaller = componentModel.GetService<IVsPackageInstaller2>();
            
            packageInstaller.InstallLatestPackage(null, project, packageName, false, false);
        }
        public Project GetSelectedProject()
        {
            Project activeProject = null;

            Array activeSolutionProjects = this.dte.ActiveSolutionProjects as Array;
            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
            {
                activeProject = activeSolutionProjects.GetValue(0) as Project;
            }

            return activeProject;
        }
    }
}