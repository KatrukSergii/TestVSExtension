//using EnvDTE;
//using Microsoft.VisualStudio;
//using Microsoft.VisualStudio.ComponentModelHost;
//using Microsoft.VisualStudio.Shell.Interop;
//using NuGet.VisualStudio;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;

//namespace SampleInjector
//{
//    public class VSHelper
//    {
//        private const string WebConfig = "web.config";
//        private const string AppConfig = "app.config";

//        private static readonly Dictionary<string, string> _knownNestedFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
//            { "web.debug.config", "web.config" },
//            { "web.release.config", "web.config" }
//        };
//        public string GetConfigurationFile(Project project)
//        {
//            return this.IsWebProject(project) ? WebConfig : AppConfig;
//        }

//        private readonly IComponentModel componentModel;

//        private readonly DTE dte;
//        private IVsSolution solution;

//        public VSHelper(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider)
//        {
//            this.componentModel = serviceProvider.GetServiceAsync(typeof(SComponentModel)).Result as IComponentModel; 
//            this.dte = serviceProvider.GetServiceAsync(typeof(DTE)).Result as DTE;
//            this.solution = serviceProvider.GetServiceAsync(typeof(IVsSolution)).Result as IVsSolution;
//        }

//        public void SetupNugetPakadge(Project project, string packageName)
//        {
//            var packageInstaller = componentModel.GetService<IVsPackageInstaller2>();
            
//            packageInstaller.InstallLatestPackage(null, project, packageName, false, false);
//        }
//        public Project GetSelectedProject()
//        {
//            Project activeProject = null;

//            Array activeSolutionProjects = this.dte.ActiveSolutionProjects as Array;
//            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
//            {
//                activeProject = activeSolutionProjects.GetValue(0) as Project;
//            }

//            return activeProject;
//        }

//        public bool IsWebProject(Project project)
//        {
//            string[] types = this.GetProjectTypeGuids(project);
//            return types.Contains(VsConstants.WebSiteProjectTypeGuid, StringComparer.OrdinalIgnoreCase) ||
//                   types.Contains(VsConstants.WebApplicationProjectTypeGuid, StringComparer.OrdinalIgnoreCase);
//        }

//        public string GetUniqueName(Project project)
//        {
//            try
//            {
//                return project.UniqueName;
//            }
//            catch (COMException)
//            {
//                return project.FullName;
//            }
//        }

//        public IVsHierarchy ToVsHierarchy(Project project)
//        {
//            IVsHierarchy hierarchy;

//            // Get the vs solution
//            int hr = this.solution.GetProjectOfUniqueName(this.GetUniqueName(project), out hierarchy);

//            if (hr != VSConstants.S_OK)
//            {
//                Marshal.ThrowExceptionForHR(hr);
//            }

//            return hierarchy;
//        }


//        public string[] GetProjectTypeGuids(Project project)
//        {
//            // Get the vs hierarchy as an IVsAggregatableProject to get the project type guids

//            var hierarchy = this.ToVsHierarchy(project);
//            var aggregatableProject = hierarchy as IVsAggregatableProject;
//            if (aggregatableProject != null)
//            {
//                string projectTypeGuids;
//                int hr = aggregatableProject.GetAggregateProjectTypeGuids(out projectTypeGuids);

//                if (hr != VSConstants.S_OK)
//                {
//                    Marshal.ThrowExceptionForHR(hr);
//                }

//                return projectTypeGuids.Split(';');
//            }
//            else if (!String.IsNullOrEmpty(project.Kind))
//            {
//                return new[] { project.Kind };
//            }
//            else
//            {
//                return new string[0];
//            }
//        }
//    }
//    internal static class VsConstants
//    {
//        // Project type guids
//        internal const string WebApplicationProjectTypeGuid = "{349C5851-65DF-11DA-9384-00065B846F21}";
//        internal const string WebSiteProjectTypeGuid = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
//        internal const string CsharpProjectTypeGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
//        internal const string VbProjectTypeGuid = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
//        internal const string CppProjectTypeGuid = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
//        internal const string FsharpProjectTypeGuid = "{F2A71F9B-5D33-465A-A702-920D77279786}";
//        internal const string JsProjectTypeGuid = "{262852C6-CD72-467D-83FE-5EEB1973A190}";
//        internal const string WixProjectTypeGuid = "{930C7802-8A8C-48F9-8165-68863BCCD9DD}";
//        internal const string LightSwitchProjectTypeGuid = "{ECD6D718-D1CF-4119-97F3-97C25A0DFBF9}";
//        internal const string NemerleProjectTypeGuid = "{edcc3b85-0bad-11db-bc1a-00112fde8b61}";
//        internal const string InstallShieldLimitedEditionTypeGuid = "{FBB4BD86-BF63-432a-A6FB-6CF3A1288F83}";
//        internal const string WindowsStoreProjectTypeGuid = "{BC8A1FFA-BEE3-4634-8014-F334798102B3}";
//        internal const string SynergexProjectTypeGuid = "{BBD0F5D1-1CC4-42fd-BA4C-A96779C64378}";
//        internal const string NomadForVisualStudioProjectTypeGuid = "{4B160523-D178-4405-B438-79FB67C8D499}";

//        // Copied from EnvDTE.Constants since that type can't be embedded
//        internal const string VsProjectItemKindPhysicalFile = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
//        internal const string VsProjectItemKindPhysicalFolder = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";
//        internal const string VsProjectItemKindSolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";
//        internal const string VsProjectItemKindSolutionItem = "{66A26722-8FB5-11D2-AA7E-00C04F688DDE}";
//        internal const string VsWindowKindSolutionExplorer = "{3AE79031-E1BC-11D0-8F78-00A0C9110057}";
//        internal const string VsProjectKindMisc = "{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}";

//        // All unloaded projects have this Kind value
//        internal const string UnloadedProjectTypeGuid = "{67294A52-A4F0-11D2-AA88-00C04F688DDE}";

//        internal const string NuGetSolutionSettingsFolder = ".nuget";

//        // HResults
//        internal const int S_OK = 0;
//    }
//}