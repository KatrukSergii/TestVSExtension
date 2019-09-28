using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using SampleInjector;
using SampleInjector.Interface;
using TextExtension.Commands;
using Task = System.Threading.Tasks.Task;

namespace TestExtension
{
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[Guid(TestExtensionPackage.PackageGuidString)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	public sealed class TestExtensionPackage : AsyncPackage
	{
		/// <summary>
		/// TestExtensionPackage GUID string.
		/// </summary>
		public const string PackageGuidString = "445e1ffe-6736-44f2-ae6d-f26aada9b45d";

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			InversionContainer.Instance.Register<ISampleInjector>(new SampleInjector.SampleInjector());
			IMenuCommandService commandService = await this.GetServiceAsync(typeof(IMenuCommandService)) as IMenuCommandService;
			await CommandsProvider.InitializeCommandsAsync(commandService, this);
		}
	}
}
