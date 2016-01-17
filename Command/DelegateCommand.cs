using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamOatesLibrary.WPF.Command
{
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> m_canExecute;
        private readonly Action<object> m_execute;
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="execute"></param>
        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            m_execute = execute;
            m_canExecute = canExecute;
        }

        /// <summary>
        /// Check to see if the command can be executed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (m_canExecute == null)
            {
                return true;
            }

            return m_canExecute(parameter);
        }

        /// <summary>
        /// Execute the delegate command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            m_execute(parameter);
        }

        /// <summary>
        /// Raise can execute event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
