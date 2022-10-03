using EnginEditor.GameProject.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EnginEditor.Components
{
    [DataContract]
    public class Component : ViewModelBase
    {
        public GameEntity Owner { get; private set; }

        public Component(GameEntity entity)
        {
            Debug.Assert(entity != null);   
            Owner = entity;

        }
    }
}
