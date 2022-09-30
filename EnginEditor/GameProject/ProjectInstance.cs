using EnginEditor.GameProject.Common;
using EnginEditor.GameProject.Utilites;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;

namespace EnginEditor.GameProject
{
    [DataContract(Name ="Game")]
     public class ProjectInstance : ViewModelBase
    {
        #region Property
        public static string Extension { get; } = ".primal";
        [DataMember]
        public string Name { get; private set; } 
        [DataMember]
        public string Path { get; private set; }

        public string FullPath => $"{Path}{Name}{Extension}";

        [DataMember(Name= "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyCollection<Scene> Scenes { get; private set; }

        private Scene _activeScene;

        public Scene ActiveScene
        {
            get => _activeScene; 
            set 
            {
                if (_activeScene != value)
                {
                    _activeScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }             
            }
        }
        #endregion

        public static ProjectInstance Current => Application.Current.MainWindow.DataContext as ProjectInstance;

        public void AddScene(string SceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(SceneName.Trim()));
            _scenes.Add(new Scene(this, SceneName));
        }

        /// <summary>
        /// Read ProjectInstance from file path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ProjectInstance Load(string filePath)
        {
            Debug.Assert(File.Exists(filePath));
            return Serializer.FromFile<ProjectInstance>(filePath);
        }

        public void Unload()
        {
            
        }

        public static void Save(ProjectInstance project)
        {
            Serializer.ToFile(project, project.FullPath);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            Scenes.FirstOrDefault(x => x.IsActive);
        }

        public ProjectInstance(string name, string path)
        {
            Name = name;
            Path = path;
            OnDeserialized(new StreamingContext());
        }
    }
}
