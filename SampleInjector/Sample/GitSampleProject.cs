using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using SampleInjector.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SampleInjector.Sample
{
    public class GitSampleProject : Interface.ISample
    {
        private readonly string gitProjectArchivePath;
        private string zipPath;
        private string projTempPath;

        public GitSampleProject(string projectZipPath) => this.gitProjectArchivePath = projectZipPath;

        public Dictionary<string, string> Settings
        {
            get;
            private set;
        }

        public Project SampleProject
        {
            get;
            private set;
        }

        public void Dispose()
        {
            this.TryDeleteTempFiles(this.zipPath, this.projTempPath);
        }

        public async Task InitializeAsync()
        {
            this.zipPath = Path.Combine(Path.GetTempPath(), "master.zip");
            this.projTempPath = Path.Combine(Path.GetTempPath(), "master");
            try
            {
                this.TryDeleteTempFiles(zipPath, projTempPath);

                using (var client = new HttpClient())
                {
                    var contents = await client.GetByteArrayAsync(this.gitProjectArchivePath);
                    File.WriteAllBytes(zipPath, contents);
                }

                ZipFile.ExtractToDirectory(zipPath, projTempPath);
                var getTemplateSolutionFile = Directory.GetFiles(projTempPath, "*.sln", SearchOption.AllDirectories).FirstOrDefault();
                this.Settings = ReadSettings(projTempPath);
                this.ConfigFile = Directory.GetFiles(projTempPath, "App.config", SearchOption.AllDirectories).FirstOrDefault();

                var templateMsWorkspace = MSBuildWorkspace.Create();
                var templateSolution = await templateMsWorkspace.OpenSolutionAsync(getTemplateSolutionFile);
                this.SampleProject = templateSolution.Projects.FirstOrDefault();
            }
            catch (Exception exception)
            {
                this.TryDeleteTempFiles(zipPath, projTempPath);
                throw new SampleInitException("Couldn't initialize exception", exception);
            }
        }

        public string ConfigFile
        {
            get;
            private set;
        }

        public string CopyconfigFile(string realProjPath)
        {
            var templConfigFile = Directory.GetFiles(this.projTempPath, "App.config", SearchOption.AllDirectories).FirstOrDefault();
            var realConfigFile = Directory.GetFiles(realProjPath, "App.config", SearchOption.AllDirectories).FirstOrDefault();
            if (realConfigFile == null)
            {
                string newConfigFile = Path.Combine(realProjPath, Path.GetFileName(templConfigFile));
                File.Copy(templConfigFile, newConfigFile);
                return newConfigFile;
            }
            try
            {
                var templDoc = XDocument.Load(templConfigFile);
                var realDoc = XDocument.Load(realConfigFile);
                realDoc.Root.Descendants().Union(templDoc.Root.Descendants());
                templDoc.Save(realConfigFile);
                return realConfigFile;
            }
            catch (Exception)
            {
                //
                // Ignnore.
                //
            }
            return string.Empty;
        }

        private Dictionary<string, string> ReadSettings(string projTempPath)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            try
            {
                var configFile = Directory.GetFiles(projTempPath, "App.config", SearchOption.AllDirectories).FirstOrDefault();
                var doc = XDocument.Load(configFile);
                XElement node = doc.Descendants().FirstOrDefault(element => element.Name == "appSettings");
                foreach (XElement element in node.Elements())
                {
                    var keyAttr = element.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "key");
                    var valAttr = element.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "value");
                    settings.Add(keyAttr.Value, valAttr.Value);
                }

            }
            catch (Exception)
            {
                //
                // Ignnore.
                //
            }
            return settings;
        }

        private void TryDeleteTempFiles(string zipPath, string projTempPath)
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);

            if (Directory.Exists(projTempPath))
                Directory.Delete(projTempPath, true);
        }
    }
}
