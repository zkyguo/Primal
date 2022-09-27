using EnginEditor.GameProject.Common;
using EnginEditor.GameProject.Utilites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace EnginEditor.GameProject 
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }
        [DataMember]
        public byte[] Icon { get; set; }
        [DataMember]
        public byte[] Screenshot { get; set; }
        public string IconFilePath { get; set; }
        public string ScreenshotPath { get; set; }
        public string ProjectFilePath { get; set; }
    }

    class ProjectInstance : ViewModelBase
    {
        #region Path
        private static string _defaultProjectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\EnginProject\";
        private static string _templatePath = @"..\..\GameProject\ProjectTemplates\";
        #endregion

        #region Private value
        private string _projectName = "NewProject";
        private string _projectPath = _defaultProjectPath;
        
        #endregion

        #region Property
        public string ProjectName 
        {
            get => _projectName;
            set
            {
                if (_projectName != value)
                {
                    _projectName = value;
                    OnPropertyChanged(nameof(ProjectName));
                }
            }
        }

        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                if (_projectPath != value)
                {
                    _projectPath = value;
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }
        #endregion

        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        #region Constructor
        public ProjectInstance()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
            try
            {
                var templatesFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templatesFiles.Any());
                foreach (var file in templatesFiles)
                {
                    var template = Serializer.FromFile<ProjectTemplate>(file);          
                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    template.ScreenshotPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotPath);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));
                    _projectTemplates.Add(template);
                }
            }
            //TODO : Error handle
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            
        }
        #endregion
    }


}
