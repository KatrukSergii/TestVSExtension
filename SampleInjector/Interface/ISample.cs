using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleInjector.Interface
{
    public interface ISample : IDisposable
    {
        Dictionary<string, string> Settings
        {
            get;
        }

        Task InitializeAsync();

        Project SampleProject
        {
            get;
        }
    }
}