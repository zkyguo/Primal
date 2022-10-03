using EnginEditor.Components;
using EnginEditor.GameProject.Common;
using EnginEditor.GameProject.Utilites;
using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace EnginEditor.GameProject
{
	[DataContract]
    public class Scene : ViewModelBase
    {
        #region Property
        private string _name;

        [DataMember]
        public string Name
		{
			get => _name;
			set {

				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}
        [DataMember]
        public ProjectInstance Project { get; private  set; }

		private bool _isActive;

		[DataMember]
		public bool IsActive { 
			get => _isActive;
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					OnPropertyChanged(nameof(_isActive));
				}
			}
		}
        #endregion

        #region GameEntity in Scene
        [DataMember(Name =nameof(GameEntities))]
		private readonly ObservableCollection<GameEntity> _gameEntities = new ObservableCollection<GameEntity>();
		public ReadOnlyObservableCollection<GameEntity> GameEntities { get; private set; }
		#endregion

		#region Game Entity Operation
		private void _addGameEntity(GameEntity entity)
		{
			Debug.Assert(!_gameEntities.Contains(entity) || entity != null);
			_gameEntities.Add(entity);
        }

		private void _removeGameEntity(GameEntity entity)
		{
			Debug.Assert(!_gameEntities.Contains(entity) || entity != null);
            _gameEntities.Remove(entity);

        }

        #endregion

        #region Commands
        public ICommand AddGameEntityCommand { get; private set; }
		public ICommand RemoveGameEntityCommand { get; private set; }	

        #endregion

        [OnDeserialized]
		private void OnDeserialization(StreamingContext context)
		{
			if(_gameEntities != null)
			{
				GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
				OnPropertyChanged(nameof(GameEntities));

			}
            AddGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                //Define AddScene Action
                _addGameEntity(x);
				var EntityIndex = _gameEntities.Count - 1;

				//Define UndoRedo action for AddScene command
				ProjectInstance.UndoRedo.Add(new UndoRedoAction(
					() => _removeGameEntity(x),
					() => _gameEntities.Insert(EntityIndex, x),
					$"Add {x.Name} to {Name}"
					)); ;
            });

            //Define Remove Scene Command
            RemoveGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                //
                var EntityIndex = _gameEntities.IndexOf(x);
                _removeGameEntity(x);

                ProjectInstance.UndoRedo.Add(new UndoRedoAction(
                    () => _gameEntities.Insert(EntityIndex, x),
                    () => _removeGameEntity(x),
					$"Remove {x.Name} of {Name}"
                    ));
            }, x => x != null);
        }

		public Scene(ProjectInstance project, string name)
		{
			Debug.Assert(project != null);
			Project = project;
			Name = name;
			OnDeserialization(new StreamingContext());
		}


	}
}