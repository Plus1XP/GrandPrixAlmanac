using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GrandPrixAlmanac.Models
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Func<Task> execute;
        private bool isExecuting;

        public AsyncRelayCommand(Func<Task> execute) : this(execute, () => true)
        {
        }

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return !(this.isExecuting && this.canExecute());
        }


        public async void Execute(object parameter)
        {
            this.isExecuting = true;
            this.OnCanExecuteChanged();

            try
            {
                await this.execute();
            }
            finally
            {
                this.isExecuting = false;
                this.OnCanExecuteChanged();
            }
        }

        protected virtual void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }
    }
}