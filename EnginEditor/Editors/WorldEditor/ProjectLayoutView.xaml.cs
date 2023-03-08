using EnginEditor.Components;
using EnginEditor.GameProject;
using EnginEditor.GameProject.Utilites;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EnginEditor.Editors
{
    /// <summary>
    /// Interaction logic for ProjectLayoutView.xaml
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void AddGameEntityButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var vm = btn.DataContext as Scene;
            vm.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty game entity" });
        }

        private void OnGameEntities_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Clean the preview Instance
            GameEntityView.Instance.DataContext = null;
            var listBox = sender as ListBox;

            /*if (e.AddedItems.Count > 0)
            {
                GameEntityView.Instance.DataContext = listBox.SelectedItems[0];
            }*/

            var newSelection = listBox.SelectedItems.Cast<GameEntity>().ToList();
            var previousSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList();

            ProjectInstance.UndoRedo.Add(new UndoRedoAction(
                () => { //Undo Action

                    listBox.UnselectAll();
                    previousSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
                }, 
                () => { //Redo Action

                    listBox.UnselectAll();
                    newSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);

                }, 
                "Selection changed"
                ));

            //Clean current multi select gameEntity
            MSGameEntity MSEntity = null;
            //if newSelection has element
            if (newSelection.Any())
            {
                //Set the new select GameEntity as MSGameEntity
                MSEntity = new MSGameEntity(newSelection);
            }
            GameEntityView.Instance.DataContext = MSEntity;


        }

        
    }
}
