using Canvas.MAUI.ViewModels;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.MAUI.Views;

public partial class QuizEditorPage : ContentPage, IQueryAttributable
{
    private QuizEditorViewModel _vm;

    public QuizEditorPage()
    {
        InitializeComponent();
        _vm = new QuizEditorViewModel();
        BindingContext = _vm;
    }

    // Shell calls this before OnNavigatedTo — forwards courseId / assignmentId to the VM
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _vm.ApplyQueryAttributes(query);
        Title = _vm.IsEditing ? "Edit Quiz" : "New Quiz";
    }

    // ── Toolbar ──────────────────────────────────────────────────────────────

    private async void CancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    // ── Questions ────────────────────────────────────────────────────────────

    private void AddQuestionClicked(object sender, EventArgs e)
    {
        _vm.AddQuestion();
    }

    private void RemoveQuestionClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is QuestionViewModel question)
            _vm.RemoveQuestion(question);
    }

    // ── Options ──────────────────────────────────────────────────────────────

    private void AddOptionClicked(object sender, EventArgs e)
    {
        // CommandParameter is bound to the QuestionViewModel (the parent DataTemplate)
        if (sender is Button btn && btn.CommandParameter is QuestionViewModel question)
            _vm.AddOption(question);
    }

    private void RemoveOptionClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is OptionViewModel option)
        {
            var question = _vm.Questions.FirstOrDefault(q => q.Options.Contains(option));
            if (question != null)
                _vm.RemoveOption(question, option);
        }
    }

    // ── Save ─────────────────────────────────────────────────────────────────

    private async void SaveClicked(object sender, EventArgs e)
    {
        var success = await _vm.Save();
        if (success)
            await Shell.Current.GoToAsync("..");
    }
}