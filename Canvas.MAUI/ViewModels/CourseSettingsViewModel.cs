using Canvas.Library.Model;
using Canvas.Library.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Canvas.MAUI.ViewModels
{
    internal class CourseSettingsViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); }
        }
        public void SaveSemesterDates()
        {
            CourseServiceProxy.Current.UpdateSemesterDates(_courseId, StartDate, EndDate);
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _courseId;

        public static readonly string[] AllLetters = new[]
        {
            "A+", "A", "A-",
            "B+", "B", "B-",
            "C+", "C", "C-",
            "D+", "D", "D-",
            "F"
        };

        public List<string> Letters { get; } = AllLetters.ToList();

        public ObservableCollection<LetterGradeRow> GradeRows { get; set; } = new();

        private LetterGradeRow _selectedRow;
        public LetterGradeRow SelectedRow
        {
            get => _selectedRow;
            set { _selectedRow = value; OnPropertyChanged(); }
        }

        private string _selectedLetter;
        public string SelectedLetter
        {
            get => _selectedLetter;
            set
            {
                _selectedLetter = value;
                OnPropertyChanged();
                SelectedRow = GradeRows.FirstOrDefault(r => r.Letter == value);
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("courseId", out var val) &&
                int.TryParse(val?.ToString(), out int id))
            {
                _courseId = id;
                LoadGradeScale();
                var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == id);
                StartDate = course?.SemesterTaught?.StartDate;
                EndDate = course?.SemesterTaught?.EndDate;
            }
        }

        public void LoadGradeScale()
        {
            var existing = CourseServiceProxy.Current.GetGradeScale(_courseId);
            GradeRows.Clear();

            foreach (var letter in AllLetters)
            {
                var match = existing.FirstOrDefault(g => g.Letter == letter);
                GradeRows.Add(new LetterGradeRow
                {
                    Id = match?.Id ?? 0,
                    Letter = letter,
                    Min = match?.MinPercentage.ToString() ?? string.Empty,
                    Max = match?.MaxPercentage.ToString() ?? string.Empty,
                    HexColor = match?.HexColor ?? "#888888"
                });
            }

            OnPropertyChanged(nameof(GradeRows));
        }

        public async Task SaveGradeScale()
        {
            foreach (var row in GradeRows)
            {
                if (!double.TryParse(row.Min, out double min) ||
                    !double.TryParse(row.Max, out double max))
                    continue;

                var grade = new LetterGrade
                {
                    Id = row.Id,
                    CourseId = _courseId,
                    Letter = row.Letter,
                    MinPercentage = min,
                    MaxPercentage = max,
                    HexColor = row.HexColor
                };

                var saved = CourseServiceProxy.Current.AddOrUpdateLetterGrade(_courseId, grade);
                if (saved != null)
                    row.Id = saved.Id;
            }
        }

        // ── Row model ────────────────────────────────────────────────────────

        public class LetterGradeRow : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged([CallerMemberName] string name = null)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

            public int Id { get; set; }
            public string Letter { get; set; }

            private string _min;
            public string Min
            {
                get => _min;
                set
                {
                    _min = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(RangeDisplay));
                }
            }

            private string _max;
            public string Max
            {
                get => _max;
                set
                {
                    _max = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(RangeDisplay));
                }
            }

            private string _hexColor;
            public string HexColor
            {
                get => _hexColor;
                set
                {
                    _hexColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ColorPreview));
                }
            }

            public string RangeDisplay =>
                string.IsNullOrWhiteSpace(Min) || string.IsNullOrWhiteSpace(Max)
                    ? "Not set"
                    : $"{Min}% – {Max}%";

            public Color ColorPreview
            {
                get
                {
                    try { return Color.FromArgb(HexColor); }
                    catch { return Colors.Gray; }
                }
            }
        }
    }
}