using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace EnginEditor.Dictionaries
{
    public partial class ControlTemplates : ResourceDictionary
    {
        private void OnTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var tBox = sender as TextBox;
            var exp = tBox.GetBindingExpression(TextBox.TextProperty);
            if (exp == null) return;
            if(e.Key == Key.Enter)
            {
                if(tBox.Tag is ICommand command && command.CanExecute(tBox.Text))
                {
                    command.Execute(tBox.Text);
                }
                else
                {
                    exp.UpdateSource();
                }
                Keyboard.ClearFocus();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape) 
            {
                exp.UpdateTarget();
                Keyboard.ClearFocus();
            }

        }
    }
}
