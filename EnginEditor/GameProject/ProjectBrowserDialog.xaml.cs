﻿using System;
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
using System.Windows.Shapes;

namespace EnginEditor.GameProject
{
    /// <summary>
    /// Interaction logic for ProjectBrowserDiagl.xaml
    /// </summary>
    public partial class ProjectBrowserDialog : Window
    {
        public ProjectBrowserDialog()
        {
            InitializeComponent();
            Loaded += OpenProjectBrowserDialogLoaded;
        }

        private void OpenProjectBrowserDialogLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OpenProjectBrowserDialogLoaded;
            if(!OpenProjectViewModel.Projects.Any())
            {
                openProjectButton.IsEnabled = false;
                openProjectView.Visibility = Visibility.Hidden;
                onToggleButton_Click(createProjectButton, new RoutedEventArgs());
            }

        }

        private void onToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == openProjectButton)
            {
                if (createProjectButton.IsChecked == true)
                {
                    createProjectButton.IsChecked = false;
                    BrowserContent.Margin = new Thickness(0);
                }
                openProjectButton.IsChecked = true;
            }
            else if (sender == createProjectButton)
            {
                if (openProjectButton.IsChecked == true)
                {
                    openProjectButton.IsChecked = false;
                    BrowserContent.Margin = new Thickness(-800,0,0,0);
                }
                createProjectButton.IsChecked = true;
            }
        }
    }
}
