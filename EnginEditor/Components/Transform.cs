using EnginEditor.GameProject.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EnginEditor.Components
{
    [DataContract]
    public class Transform : Component
    {
        [DataMember]
        private Vector3 _position;
        [DataMember]
        private Vector3 _rotation;
        [DataMember]
        private Vector3 _scale;

        public Vector3 Position
        {
            get => _position;
            set { 

                if(_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        public Vector3 Rotation
        {
            get => _rotation;
            set
            {

                if (_rotation != value)
                {
                    _rotation = value;
                    OnPropertyChanged(nameof(Rotation));
                }
            }
        }

        public Vector3 Scale
        {
            get => _scale;
            set
            {

                if (_scale != value)
                {
                    _scale = value;
                    OnPropertyChanged(nameof(Scale));
                }
            }
        }

        public Transform(GameEntity entity) : base(entity)
        {
            _position = new Vector3(0);
            _rotation = new Vector3(0);
            _scale = new Vector3(1);
        }
    }
}
