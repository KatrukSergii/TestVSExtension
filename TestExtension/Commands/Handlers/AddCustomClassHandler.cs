using EnvDTE;
using Extension.Core;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SampleInjector;
using SampleInjector.Interface;
using System;
using System.Threading.Tasks;
using System.Windows.Interop;
using TestExtension;
using ViewModel;
using VSIXProject2.Commands.Handlers;
using WpfComponents;
using WpfComponents.Windows;
using Task = System.Threading.Tasks.Task;

namespace TextExtension.Commands.Handlers
{
    public sealed class AddCustomClassHandler : CommandHandlerBase
	{
		private readonly AsyncPackage package;

		public AddCustomClassHandler(AsyncPackage package)
		{
			this.package = package;
		}

		protected override async Task<bool> CanExecute()
		{
			return true;
		}

		public override async Task ExecuteAsync()
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
			DTE dte = await this.package.GetServiceAsync(typeof(DTE)) as DTE;
			//SelectClassToInsertWindow window = new SelectClassToInsertWindow();
			//var vm = new AddClassViewModel(new WindowViewService(window));
			//window.DataContext = vm;
			//var result = this.ShowDialog(window, dte);
			//if (result)
			{
				var solService = await this.package.GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
				Assumes.Present(solService);
                var proj = this.GetSelectedProject();
                
                Class1 class1 = new Class1();
                class1.Exec(proj.FullName);
                //int res = solService.GetSolutionInfo(out string solutionDirectory, out string solutionFile, out string userOptsFile);
                ISampleInjector sampleInjector = InversionContainer.Instance.Reslove<ISampleInjector>();

                //await sampleInjector.InjectSampleAsync(proj.FullName);

            }
		}

        public Project GetSelectedProject()
        {
            Project activeProject = null;
            var dte = this.package.GetServiceAsync(typeof(DTE)).Result as DTE; 
            Array activeSolutionProjects = dte.ActiveSolutionProjects as Array;
            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
            {
                activeProject = activeSolutionProjects.GetValue(0) as Project;
            }

            return activeProject;
        }

        public bool ShowDialog(System.Windows.Window dialog, DTE dte)
		{
			var hwnd = dte.MainWindow.HWnd;

			var helper = new WindowInteropHelper(dialog);

			helper.Owner = new IntPtr(hwnd);

			return dialog.ShowDialog() ?? false;
		}
	}
}