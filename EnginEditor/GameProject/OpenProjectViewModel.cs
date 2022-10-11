using EnginEditor.GameProject.Common;
using EnginEditor.GameProject.Utilites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace EnginEditor.GameProject
{
    
    [DataContract]
    public class ProjectData
    {
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public string ProjectPath { get; set; }
        [DataMember]
        public DateTime Date { get; set; }

        public string FullPath { get => $"{ProjectPath}{ProjectName}{ProjectInstance.Extension}"; }

        public byte[] Icon { get; set;}
        public byte[] Screenshot { get; set; }    
    }
    public class ProjectDataList
    {
        public List<ProjectData> Projects { get; set; }
    }
    public class OpenProjectViewModel : ViewModelBase
    {
        private static readonly string _applicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\EnginEditor\";
        private static string _projectDataPath;
        private static readonly ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();  
        public static ReadOnlyObservableCollection<ProjectData> Projects 
        { get; set; }

        /// <summary>
        /// Record when and where we create project and save data into a file 
        /// </summary>
        static OpenProjectViewModel()
        {
            try
            {
                //Find if dataInfo exists in appdata folder 
                if (!Directory.Exists(_applicationDataPath))
                {
                    Directory.CreateDirectory(_applicationDataPath);               
                }
                _projectDataPath = $@"{_applicationDataPath}ProjectData.xml";
                Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);
                ReadProjectData();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Logger.Log(MessageType.Error, $"Failed to read project data");

            }
        }

        /// <summary>
        /// Read appData which contain project creating data
        /// </summary>
        private static void ReadProjectData()
        {
            if (File.Exists(_projectDataPath))
            {
                var projects = Serializer.FromFile<ProjectDataList>(_projectDataPath).Projects.OrderByDescending(x => x.Date);
                _projects.Clear();
                foreach (var project in projects)
                {
                    if (File.Exists(project.FullPath))
                    {
                        project.Icon = File.ReadAllBytes($@"{project.ProjectPath}\.Primal\Icon.png");
                        project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}\.Primal\Screenshot.png");
                        _projects.Add(project);
                    }
                }
            }
        }

        /// <summary>
        /// Update prpject last modify date in appData and order them
        /// </summary>
        private static void WriteProjectData()
        {
            var projects = _projects.OrderBy(x => x.Date).ToList();
            Serializer.ToFile(new ProjectDataList() { Projects = projects }, _projectDataPath);
        }



        public static ProjectInstance Open(ProjectData data)
        {
            ReadProjectData();
            //Find the selected project 
            var project = _projects.FirstOrDefault(x => x.FullPath == data.FullPath);

            //Set date data of right now
            if(project != null) 
            {
                project.Date = DateTime.Now;
            }
            else
            {
                project = data;
                project.Date = DateTime.Now;
                _projects.Add(project);
            }
            WriteProjectData();

            return ProjectInstance.Load(project.FullPath);
        }

      
    }
}
