using Microsoft.VisualStudio.Shell;

namespace SampleInjector
{
    public interface ISampleInjector
	{
        System.Threading.Tasks.Task InjectSample(string sampleId, string solutionFilePath, IAsyncServiceProvider serviceProvider);
	}
}
