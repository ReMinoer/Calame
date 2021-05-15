using System;
using System.Windows.Input;

namespace Calame.DocumentContexts
{
    public class SelectionCommand : ICommand
    {
        private readonly ISelectionContext _context;

        public SelectionCommand(ISelectionContext context)
        {
            _context = context;
        }

        public event EventHandler CanExecuteChanged
        {
            add => _context.CanSelectChanged += value;
            remove => _context.CanSelectChanged -= value;
        }

        public bool CanExecute(object parameter) => _context.CanSelect(parameter);
        public void Execute(object parameter) => _context.SelectAsync(parameter).Wait();
    }

    public class SelectionCommand<T> : ICommand
    {
        private readonly ISelectionContext<T> _context;

        public SelectionCommand(ISelectionContext<T> context)
        {
            _context = context;
        }

        public event EventHandler CanExecuteChanged
        {
            add => _context.CanSelectChanged += value;
            remove => _context.CanSelectChanged -= value;
        }

        public bool CanExecute(object parameter) => _context.CanSelect(parameter);
        public void Execute(object parameter) => _context.SelectAsync(parameter).Wait();
    }
}