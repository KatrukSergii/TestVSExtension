using System.Threading.Tasks;

namespace SampleInjector.Interface
{
    public interface ISampleInjector
	{
        Task InjectSampleAsync(string projFilePath);

    }
}
