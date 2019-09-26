using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using SampleInjector;
using TestExtension;
using ViewModel;
using VSIXProject2;
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
			SelectClassToInsertWindow window = new SelectClassToInsertWindow();
			var vm = new AddClassViewModel(new WindowViewService(window));
			window.DataContext = vm;
			var result = this.ShowDialog(window, dte);
			if (result)
			{
				var solService = await this.package.GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
				Assumes.Present(solService);
				int res = solService.GetSolutionInfo(out string solutionDirectory, out string solutionFile, out string userOptsFile);
				ISampleInjector sampleInjector = InversionContainer.Instance.Reslove<ISampleInjector>();

				var componentModel = await this.package.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
				var packageInstaller = componentModel.GetService<IVsPackageInstaller2>();
				Projects projects = dte.Solution.Projects;
				foreach (Project proj in projects)
				{
					packageInstaller.InstallLatestPackage(null, proj, "DocuSign.eSign.dll", false, false);
				}

				await sampleInjector.InjectSample(vm.SelectedClass, solutionFile);

			}
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