using Canvas.MAUI.ViewModels;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace Canvas.MAUI.Views;

public partial class QuizAttemptPage : ContentPage, IQueryAttributable
{
    private QuizAttemptViewModel _vm;

    public QuizAttemptPage()
    {
        InitializeComponent();
        _vm = new QuizAttemptViewModel();
        BindingContext = _vm;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _vm.ApplyQueryAttributes(query);
    }

    private async void BackClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");

    private void OptionTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is QuizAttemptOptionViewModel option && _vm.CanAttempt)
            option.IsSelected = true;
    }

    private async void SubmitClicked(object sender, EventArgs e)
    {
        var success = await _vm.Submit();
        if (success)
            await DisplayAlert("Submitted!", _vm.SubmittedScoreText, "OK");
    }
}