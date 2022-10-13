using EnginEditor.GameProject;
using EnginEditor.GameProject.Common;
using EnginEditor.GameProject.Utilites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.Serialization;
using System.Windows.Input;
using System.Windows.Markup;

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

    /// <summary>
    /// Base class of an Mutil-selectable Entity
    /// </summary>
    abstract class MSEntity : ViewModelBase
    {
        private bool _enableUpdate = false;
        private bool? _isEnable;
        public bool? IsEnable
        {
            get => _isEnable;
            set 
            {
                if(_isEnable != value)
                {
                    _isEnable = value;
                    OnPropertyChanged(nameof(IsEnable));
                }
            }
        }

        private string _name;
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

        private readonly ObservableCollection<IMSComponent> _components = new ObservableCollection<IMSComponent>();
        public ReadOnlyObservableCollection<IMSComponent> Components { get; } 

        public List<GameEntity> SelectedEntities { get; }

        public MSEntity(List<GameEntity> entities)
        {
            Debug.Assert(entities?.Any() == true);
            Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
            SelectedEntities = entities;
            PropertyChanged += (s,e) => {

                if(_enableUpdate)
                 updateGameEntities(e.PropertyName);

            };

        }

        protected virtual bool updateGameEntities(string propertyName)
        {
            switch(propertyName)
            {
                case nameof(IsEnable): SelectedEntities.ForEach(x => x.IsEnable = IsEnable.Value); return true;
                case nameof(Name): SelectedEntities.ForEach(x => x.Name = Name); return true;
            }

            return false;
        }

        protected virtual bool UpdateMSGameEntity()
        {
            IsEnable = GetMixedValues(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnable));
            Name = GetMixedValues(SelectedEntities, new Func<GameEntity, string>(x => x.Name));

            return true;
        }

        public static float? GetMixedValues(List<GameEntity> entities, Func<GameEntity, float> getProperty)
        {
            var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1))
            {
                if(value.IsTheSameAs(getProperty(entity)))
                {
                    return null;
                }
            }
            return value;
        }

        public static bool? GetMixedValues(List<GameEntity> entities, Func<GameEntity, bool> getProperty)
        {
            var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1))
            {
                if (value != (getProperty(entity)))
                {
                    return null;
                }
            }
            return value;
        }

        public static string GetMixedValues(List<GameEntity> entities, Func<GameEntity, string> getProperty)
        {
            var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1))
            {
                if (value != (getProperty(entity)))
                {
                    return null;
                }
            }
            return value;
        }

        public void Refresh()
        {
            _enableUpdate = false;
            UpdateMSGameEntity();
            _enableUpdate = true;
        }
    }

    /// <summary>
    /// Define all action while Multi-selecting a GameEntity
    /// </summary>
    class MSGameEntity : MSEntity
    {
        public MSGameEntity(List<GameEntity> entities) : base(entities)
        {
            Refresh(); 
        }
    }

}
