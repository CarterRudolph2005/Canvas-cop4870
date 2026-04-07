using System.Collections.Generic;
using System.ComponentModel;
using Canvas.Library.Model;
using System.Net.Mime;
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

    private List<ModuleContent> moduleContents;
    public List<ModuleContent> ModuleContents
    {
        get => moduleContents;
        set { moduleContents = value; OnPropertyChanged(); }
    }

    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set { _isExpanded = value; OnPropertyChanged(); }
    }

    public void RefreshContents()
    {
        // Force the binding to re-evaluate by reassigning the list
        ModuleContents = new List<ModuleContent>(ModuleContents);
    }
}
}