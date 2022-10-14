using EnginEditor.Components;
using EnginEditor.GameProject;
using EnginEditor.GameProject.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EnginEditor.Editors
{
    /// <summary>
    /// Interaction logic for GameEntityView.xaml
    /// </summary>
    public partial class GameEntityView : UserControl
    {
        private Action _undoAction;
        private string _propertyName;
        public static GameEntityView Instance { get; private set; }
        public GameEntityView()
        {
            InitializeComponent();
            DataContext = null;
            Instance = this;
            DataContextChanged += (_, __) =>
            {
                if(DataContext != null)
                {
                    (DataContext as MSEntity).PropertyChanged += (s, e) => _propertyName = e.PropertyName;
                }
            };
        }
        private void OnName_TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var vm = DataContext as MSEntity;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
            var undoAction = new Action(
            () =>
            {
                selection.ForEach(item => item.entity.Name = item.Name);
                (DataContext as MSEntity).Refresh();
            });

            _undoAction = undoAction;
        }

        private void OnName_TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(_propertyName == nameof(MSEntity.Name) && _undoAction != null)
            {
                var vm = DataContext as MSEntity;
                var selection = vm.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
                var redoAction = new Action(() =>
                {
                    selection.ForEach(item => item.entity.Name = item.Name);
                    (DataContext as MSEntity).Refresh();
                });
                ProjectInstance.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, "Rename game Entity"));
                _propertyName = null;
            }
            _undoAction = null;
        }

        private void OnEnable_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MSEntity;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.IsEnable)).ToList();
            _undoAction = new Action(() =>
            {
                selection.ForEach(item => item.entity.IsEnable = item.IsEnable);
                (DataContext as MSEntity).Refresh();
            });
            vm.IsEnable = (sender as CheckBox).IsChecked == true;

            var redoAction = new Action(() =>
            {
                selection.ForEach(item => item.entity.IsEnable = item.IsEnable);
                (DataContext as MSEntity).Refresh();
            });

            ProjectInstance.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, vm.IsEnable == true ? "Enable game Entity" : "Disable game Entity"));
            
        }
    }
}
