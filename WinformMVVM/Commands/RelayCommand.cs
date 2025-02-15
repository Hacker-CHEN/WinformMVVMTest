using System;
using System.Windows.Input;

namespace WinformMVVM.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        // 构造函数，接受执行逻辑和判断是否可执行的逻辑
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // 当CanExecute的状态发生变化时，通知UI重新判断是否可执行
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        // 判断命令是否可以执行
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        // 执行命令
        public void Execute(object parameter) => _execute();
    }
}
