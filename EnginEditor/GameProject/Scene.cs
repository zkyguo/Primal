using EnginEditor.GameProject.Common;
using System.CodeDom;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace EnginEditor.GameProject
{
	[DataContract]
    public class Scene : ViewModelBase
    {
        
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
		public Scene(ProjectInstance project, string name)
		{
			Debug.Assert(project != null);
			Project = project;
			Name = name;
		}


	}
}