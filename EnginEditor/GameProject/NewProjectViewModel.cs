using EnginEditor.GameProject.Common;
using EnginEditor.GameProject.Utilites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Shapes;
using Path = System.IO.Path;

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

    class NewProjectViewModel : ViewModelBase
    {
        #region Path
        private static string _defaultProjectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\EnginProject\";
        private static string _templatePath = $@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}\GameProject\ProjectTemplates";
        #endregion

        #region Private value
        private string _projectName = "NewProject";
        private string _projectPath = _defaultProjectPath;
        private bool _isValid;
        private string _errrorMsg;
        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
       

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
                    ValidateProjectPath();
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
                    ValidateProjectPath(); 
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        public bool IsValid
        {
            get { return _isValid; }
            set
            {

                if (_isValid != value)
                    _isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public string ErrorMsg
        {
            get { return _errrorMsg; }
            set {
                if (_errrorMsg != value)
                    _errrorMsg = value;
                OnPropertyChanged(nameof(ErrorMsg));
            }
        }

        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        #endregion

        #region Constructor
        public NewProjectViewModel()
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
                ValidateProjectPath();
            }
            //TODO : Error handle
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            
        }
        #endregion

        #region Method
        private bool ValidateProjectPath()
        {
            var path = ProjectPath;
            IsValid = false;
            if (!Path.EndsInDirectorySeparator(path))
            {
                path += @"\";
            }
            path += $@"{ProjectName}\";
            if (string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                ErrorMsg = "Type in a project name.";
            }
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMsg = "Invalide character(s) used in project name.";
            }
            else if (string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMsg = "Select a valid project folder";
            }
            else if (ProjectName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMsg = "Invalide character(s) used in project path.";
            }
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMsg = "Selected project folder already exists and is not empty";
            }
            else
            {
                ErrorMsg = string.Empty;
                IsValid = true;
            }

            return IsValid;
        }

        public string CreateProject(ProjectTemplate template)
        {
            ValidateProjectPath();
            if (!IsValid)
            {
                return string.Empty;
            }

            if (!Path.EndsInDirectorySeparator(ProjectPath))
            {
                ProjectPath += @"\";
            }

            //concatenate path with project name
            var path = $@"{ProjectPath}{ProjectName}\";

            try
            {
                //if Directory doesnt exist => create Directory
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Create basic folders
                foreach (var folder in template.Folders)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path),folder)));
                }
                //Create info file and hid it
                var DirInfo = new DirectoryInfo(path + @".Primal\");
                DirInfo.Attributes |= FileAttributes.Hidden;
                //Copy Icon ans past it to info file
                File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(DirInfo.FullName, "Icon.png")));
                File.Copy(template.ScreenshotPath, Path.GetFullPath(Path.Combine(DirInfo.FullName, "Screenshot.png")));

                //Read project info file 
                var projectXML  = File.ReadAllText(template.ProjectFilePath);

                //Replace {0} and {1} of .Primal by ProjectName and ProjectPath
                projectXML = string.Format(projectXML, ProjectName, ProjectPath);
                var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{ProjectInstance.Extension}"));

                //Write the .Primal of template into project info file  
                File.WriteAllText(projectPath, projectXML);
                return path;
                
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
                return string.Empty;
            }
        }

        #endregion
    }


}
