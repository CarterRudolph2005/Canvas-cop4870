using Canvas.Library.Model;
using Canvas.Library.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Canvas.MAUI.ViewModels
{
    internal class TeacherStudentManagementViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Student> Students
        {
            get
            {
                return new ObservableCollection<Student>(StudentServiceProxy.Current.Students);
            }
        }

        public Student? SelectedStudent {get; set;}

        public event PropertyChangedEventHandler? PropertyChanged;
        
        public async Task<Student> Delete()
        {
            var student = await StudentServiceProxy.Current.DeleteStudent(SelectedStudent.Id);
            SelectedStudent = null;
            CourseServiceProxy.Current.InvalidateCache();
            Refresh();
            return student;
        }
        public void Refresh()
        {
            NotifyPropertyChanged(nameof(Students)); 
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}