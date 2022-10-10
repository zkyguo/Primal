﻿using EnginEditor.GameProject;
using EnginEditor.GameProject.Common;
using EnginEditor.GameProject.Utilites;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace EnginEditor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    public class GameEntity : ViewModelBase
    {
        #region Private
        private string _name;
        private bool _isEnable = true;
        #endregion

        #region Commands

        public ICommand RenameCommand { get; set; }
        public ICommand EnableCommand { get; set; }

        #endregion

        #region Property

        [DataMember]
		public string Name
		{
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        [DataMember]
        public Scene ParentScene { get; private set; }
        [DataMember(Name = nameof(Components))]
        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
        public ReadOnlyObservableCollection<Component> Components { get; private set; }

        [DataMember]
        public bool IsEnable { 
            
            get => _isEnable; 
            set 
            {
                if( _isEnable != value)
                {
                    _isEnable = value;
                    OnPropertyChanged(nameof(IsEnable));
                }

            }
        }

        #endregion

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if(_components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged(nameof(Components));
            }

            RenameCommand = new RelayCommand<string>(x =>
            {
                var oldName = _name;
                Name = x;

                ProjectInstance.UndoRedo.Add(new UndoRedoAction(nameof(Name), this, oldName, x, $"Rename entity {oldName} to {x}"));

            }, x => x != _name);

            EnableCommand = new RelayCommand<bool>(x =>
            {
                var previousValue = _isEnable;
                _isEnable = x;

                ProjectInstance.UndoRedo.Add(new UndoRedoAction(nameof(IsEnable), this, previousValue, x, x?$"Entity {Name} is Enable" : $"Entity {Name} is Disable"));

            });
        }

        public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;
            _components.Add(new Transform(this));
            OnDeserialized(new StreamingContext());
        }






    }

}
