using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.IO;
using System.Linq;
using Tmds.DBus.Protocol;
namespace SpaceWarProject;

public class score_axaml
{
    
}
public partial class ScoresWindow : Window
{
    public ScoresWindow()
    {
        InitializeComponent();
        LoadScores();
    }

    public void LoadScores()
    {
        string filePath = "scores.txt";

        if (File.Exists(filePath))
        {
            string[] existingScores = File.ReadAllLines(filePath);
            var scoresList = existingScores.Select(s => int.Parse(s)).ToList();
            // Set the TextBlock's text to the list of scores
            ScoresTextBlock.Text = string.Join(Environment.NewLine, scoresList);
        }
        else
        {
            ScoresTextBlock.Text = "No scores available.";
        }
    }


    private void ShowScoresButton_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}
