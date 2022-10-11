using EnginEditor.GameProject.Common;
using EnginEditor.GameProject.Utilites;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;

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

        public string FullPath => $@"{Path}{Name}\{Name}{Extension}";
        #endregion

        #region Scene 
        

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

        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyCollection<Scene> Scenes { get; private set; }

        private void _addScene(string SceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(SceneName.Trim()));
            _scenes.Add(new Scene(this, SceneName));
        }

        private void _removeScene(Scene scene)
        {
            Debug.Assert(Scenes.Contains(scene));
            _scenes.Remove(scene);
        }
        #endregion

        #region UndoRedo
        public static UndoRedo UndoRedo { get; } = new UndoRedo();

        #endregion

        #region Commands
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand AddSceneCommand { get; private set; }
        public ICommand RemoveSceneCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        #endregion

        #region Save Project
        public static void Save(ProjectInstance project)
        {
            Serializer.ToFile(project, project.FullPath);
            Logger.Log(MessageType.Info, $"Project saved to {project.FullPath}");
        }
        #endregion

        public static ProjectInstance Current => Application.Current.MainWindow.DataContext as ProjectInstance;

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
            UndoRedo.Reset();
        }

       

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

            //Define Add Scene Command
            AddSceneCommand = new RelayCommand<Object>(x =>
            {
                //Define AddScene Action
                _addScene($"New Scene {_scenes.Count}");
                var newScene = _scenes.Last();
                var sceneIndex = _scenes.Count - 1;

                //Define UndoRedo action for AddScene command
                UndoRedo.Add(new UndoRedoAction(
                    () => _removeScene(newScene),
                    () => _scenes.Insert(sceneIndex, newScene),
                    $"Add {newScene.Name}"
                    ));
            });

            //Define Remove Scene Command
            RemoveSceneCommand = new RelayCommand<Scene>(x =>
            {
                //
                var sceneIndex = _scenes.IndexOf(x);
                _removeScene(x);

                UndoRedo.Add(new UndoRedoAction(
                    () => _scenes.Insert(sceneIndex, x),
                    () => _removeScene(x),
                    $"Remove {x.Name}"
                    ));
            }, x => !x.IsActive);

            UndoCommand = new RelayCommand<Object>(x => UndoRedo.Undo());
            RedoCommand = new RelayCommand<Object>(x => UndoRedo.Redo());
            SaveCommand = new RelayCommand<Object>(x => Save(this));

        }

        public ProjectInstance(string name, string path)
        {
            Name = name;
            Path = path;
            OnDeserialized(new StreamingContext());
        }

    }
}
