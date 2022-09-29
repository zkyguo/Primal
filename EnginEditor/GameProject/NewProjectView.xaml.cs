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

namespace EnginEditor.GameProject
{
    /// <summary>
    /// Interaction logic for NewProject.xaml
    /// </summary>
    public partial class NewProjectView : UserControl
    {
        public NewProjectView()
        {
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var ViewModel = DataContext as NewProjectViewModel;
            var projectPath = ViewModel.CreateProject(templateListBox.SelectedItem as ProjectTemplate);
            var win = Window.GetWindow(this);
            bool dialogResult = false;
            if (!string.IsNullOrEmpty(projectPath))
            {
                dialogResult = true;
                //While creating new project, Open Project Window should load the new project on the list box
                var project = OpenProjectViewModel.Open(new ProjectData() { ProjectPath = projectPath, ProjectName = ViewModel.ProjectName });
                win.DataContext = project;

            }
            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}
