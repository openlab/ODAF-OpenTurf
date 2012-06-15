using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ODAF.WindowsPhone.Commands
{
    public class DelegateCommand : ICommand
    {
        private Func<object, bool> _canExecute;
        private Action<object> _executeAction;
        private bool _canExecuteCache;

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="executeAction">The delegate that the command will execute.</param>
        /// <param name="canExecute">The predicate the command will use to check if it can execute its associated delegate.</param>
        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            _executeAction = executeAction;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Call the predicate that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            bool temp = _canExecute(parameter);
            if (_canExecuteCache != temp)
            {
                _canExecuteCache = temp;
            }
            return _canExecuteCache;
        }

        /// <summary>
        /// Call the delegate of the command when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        { 
            var canExecuteChanged = CanExecuteChanged; 
            if (canExecuteChanged != null)      
                canExecuteChanged(this, e); 
        }

        public void RaiseCanExecuteChanged()
        { 
            OnCanExecuteChanged(EventArgs.Empty);
        }
    }
}
