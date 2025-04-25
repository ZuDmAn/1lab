using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AvaloniaLinearListApp.ViewModels
{
    public class LinearListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string? _newItem;
        public string? NewItem
        {
            get => _newItem;
            set
            {
                _newItem = value;
                OnPropertyChanged();
                AddItemCommand.RaiseCanExecuteChanged();
            }
        }

        private string? _currentItem;
        public string? CurrentItem
        {
            get => _currentItem;
            set
            {
                _currentItem = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _items = new();
        public ObservableCollection<string> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
                MoveNextCommand.RaiseCanExecuteChanged();
                MoveToStartCommand.RaiseCanExecuteChanged();
                ResetListCommand.RaiseCanExecuteChanged();
            }
        }

        private string? _selectedItem;
        public string? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                RemoveItemCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AddItemCommand { get; }
        public RelayCommand RemoveItemCommand { get; }
        public RelayCommand MoveNextCommand { get; }
        public RelayCommand MoveToStartCommand { get; }
        public RelayCommand ResetListCommand { get; }

        public LinearListViewModel()
        {
            AddItemCommand = new RelayCommand(_ => AddItem(), _ => !string.IsNullOrWhiteSpace(NewItem));
            RemoveItemCommand = new RelayCommand(_ => RemoveItem(), _ => SelectedItem != null);
            MoveNextCommand = new RelayCommand(_ => MoveNext(), _ => CanMoveNext());
            MoveToStartCommand = new RelayCommand(_ => MoveToStart(), _ => Items.Count > 0);
            ResetListCommand = new RelayCommand(_ => ResetList(), _ => Items.Count > 0);
        }

        private void AddItem()
        {
            if (!string.IsNullOrWhiteSpace(NewItem))
            {
                Items.Add(NewItem);
                NewItem = string.Empty;
                MoveNextCommand.RaiseCanExecuteChanged();
                MoveToStartCommand.RaiseCanExecuteChanged();
                ResetListCommand.RaiseCanExecuteChanged();
            }
        }

        private void RemoveItem()
        {
            if (SelectedItem != null)
            {
                Items.Remove(SelectedItem);
                SelectedItem = null;
                RemoveItemCommand.RaiseCanExecuteChanged();
                MoveNextCommand.RaiseCanExecuteChanged();
                MoveToStartCommand.RaiseCanExecuteChanged();
                ResetListCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanMoveNext()
        {
            if (Items.Count == 0 || string.IsNullOrEmpty(CurrentItem))
                return false;

            int index = Items.IndexOf(CurrentItem);
            return index >= 0 && index < Items.Count - 1;
        }

        private void MoveNext()
        {
            if (CanMoveNext())
            {
                int index = Items.IndexOf(CurrentItem);
                CurrentItem = Items[index + 1];
                MoveNextCommand.RaiseCanExecuteChanged();
            }
        }

        private void MoveToStart()
        {
            if (Items.Count > 0)
            {
                CurrentItem = Items[0];
                MoveNextCommand.RaiseCanExecuteChanged();
            }
        }

        private void ResetList()
        {
            Items.Clear();
            CurrentItem = null;
            SelectedItem = null;
            RemoveItemCommand.RaiseCanExecuteChanged();
            MoveNextCommand.RaiseCanExecuteChanged();
            MoveToStartCommand.RaiseCanExecuteChanged();
            ResetListCommand.RaiseCanExecuteChanged();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;
        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object? parameter) => _execute(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}