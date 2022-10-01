using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EnginEditor.GameProject.Common
{
    internal class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute; // what need to be done
        private readonly Predicate<T> _canExecute; // Check if can be done

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }    
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;

        }

        public void Execute(object parameter)
        {
            _execute((T)parameter); 
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
           
        }
    }
}
