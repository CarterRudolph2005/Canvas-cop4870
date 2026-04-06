using Canvas.Library.Model;
using Canvas.Library.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Canvas.MAUI.ViewModels
{
    internal class ContentPageViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("courseId", out var cId) && int.TryParse(cId?.ToString(), out int courseId))
                _courseId = courseId;

            if (query.TryGetValue("contentId", out var conId) && int.TryParse(conId?.ToString(), out int contentId))
            {
                _contentId = contentId;
                LoadContent();
            }
        }

        private int _courseId;
        private int _contentId;

        private string title;
        public string Title
        {
            get => title;
            set { title = value; OnPropertyChanged(); }
        }

        private string body;
        public string Body
        {
            get => body;
            set { body = value; OnPropertyChanged(); }
        }

        private void LoadContent()
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            if (course == null) return;

            var content = course.Modules?
                .SelectMany(m => m.ModuleContents ?? new List<ModuleContent>())
                .FirstOrDefault(c => c.Id == _contentId);

            if (content == null) return;

            Title = content.Name ?? string.Empty;
            Body = content.Body ?? string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}