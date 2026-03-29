using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Canvas.MAUI.Models
{
    public class ModuleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public int Id { get; set; }
        public string ModuleName { get; set; }
        public List<string> Content { get; set; }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnPropertyChanged(); }
        }
    }
}