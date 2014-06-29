using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Core
{
    public class RelayCommand : ICommand
    {
        private Action _action;
        public event EventHandler CanExecuteChanged;


        public RelayCommand(Action action)
        {
            _action = action;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }


        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                _action();
            }
            else
            {
                _action();
            }
        }

        #endregion
    }
}
