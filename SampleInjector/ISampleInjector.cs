using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleInjector
{
	public interface ISampleInjector
	{
		Task InjectSample(string sampleId, string solutionFilePath);
	}
}
