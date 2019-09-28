using Microsoft.VisualStudio.Shell;
using SystemTask = System.Threading.Tasks.Task;

namespace SampleInjector.Interface
{
    public interface ISampleInjector
	{
        SystemTask InjectSampleAsync(string sampleId, string solutionFilePath, IAsyncServiceProvider serviceProvider);
	}
}
