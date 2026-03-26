using System.Threading.Tasks;
using Canvas.Library.Model;
using Canvas.Library.Services;
namespace Canvas.MAUI.Views
{
    [QueryProperty(nameof(StudentId), "studentId")]
    public partial class StudentDetailView : ContentPage
    {
        public StudentDetailView()
        {
            InitializeComponent();
        }    
        public int StudentId {get; set;}
        public List<Classification> ClassificationOptions =>
            Enum.GetValues(typeof(Classification)).Cast<Classification>().ToList();
        


        private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            if(StudentId == 0)
            {
                BindingContext = new Canvas.Library.Model.Student();
            }
            else
            {
                BindingContext = StudentServiceProxy.Current.GetById(StudentId) ?? new Student();
            }
        }

        private async void ExitClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TeacherHome/TeacherStudentManagement");
        }

        private async void SaveAndExitClicked(object sender, EventArgs e)
        {
            StudentServiceProxy.Current.AddOrUpdate(BindingContext as Student);
            await Shell.Current.GoToAsync("//TeacherHome/TeacherStudentManagement");
        }
    }
}