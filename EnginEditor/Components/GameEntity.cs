using EnginEditor.GameProject;
using EnginEditor.GameProject.Common;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management;

namespace EnginEditor.Components
{
    public class GameEntity : ViewModelBase
    {
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
        public Scene ParentScene { get; private set; }

        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
        public ReadOnlyObservableCollection<Component> Components { get; }

        public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;

        }






    }

}
