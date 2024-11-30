using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mastermind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Variables
        StringBuilder sb = new StringBuilder();
        Random rnd = new Random();
        ComboBox[] comboBoxes;
        Label[] labels;
        string[] colors = new string[4];
        string[] solution = new string[4];
        string[] options = { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
        int attempts;
        int currentRow;
        bool debugMode;

        public MainWindow()
        {
            InitializeComponent();

            // Insert all ComboBoxes into an array of ComboBoxes and generate 6 avaliable colors for each ComboBox (from the options array variable).
            comboBoxes = new ComboBox[4] { ComboBoxOption1, ComboBoxOption2, ComboBoxOption3, ComboBoxOption4 };
            comboBoxes = AddComboBoxItems(comboBoxes);

            // Insert all Labels into an array of Labels
            labels = new Label[4] { colorLabel1, colorLabel2, colorLabel3, colorLabel4, };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            // Set all values to starting values.
            attempts = 0;
            currentRow = 0;
            debugMode = false;

            // Generate (new) random Code
            solution = InitalizeColors();

            // Set (new) solution in the hidden TextBox.
            solutionTextBox.Text = String.Join(", ", solution);

            // Set all layout to starting layout.
            UpdateAttempts();
            SetAttemptLabelLayout();
            ClearComboBoxSelection(labels);
            checkButton.Content = "Check code";
        }
        private ComboBox[] AddComboBoxItems(ComboBox[] comboBoxes)
        {
            for (int i = 0; i < comboBoxes.Length; i++)
            {
                for (int j = 0; j < options.Length; j++)
                {
                    comboBoxes[i].Items.Add(options[j]);
                }
            }
            return comboBoxes;
        }

        private string[] InitalizeColors()
        {
            // Randomize colors.
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = GenerateRandomColor();
            }

            return solution = new string[] { colors[0], colors[1], colors[2], colors[3] };
        }

        private void ToggleDebug(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.F12 && !debugMode)
            {
                solutionTextBox.Visibility = Visibility.Visible;
                debugMode = true;
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.F12 && debugMode)
            {
                solutionTextBox.Visibility = Visibility.Hidden;
                debugMode = false;
            }
        }

        private Brush ChangeLabelBackgroundColor(ComboBox ComboBox)
        {
            switch (ComboBox.SelectedIndex)
            {
                case 0:
                    return (Brush)new BrushConverter().ConvertFromString(options[0]);

                case 1:
                    return (Brush)new BrushConverter().ConvertFromString(options[1]);

                case 2:
                    return (Brush)new BrushConverter().ConvertFromString(options[2]);

                case 3:
                    return (Brush)new BrushConverter().ConvertFromString(options[3]);

                case 4:
                    return (Brush)new BrushConverter().ConvertFromString(options[4]);

                case 5:
                    return (Brush)new BrushConverter().ConvertFromString(options[5]);

                default:
                    return Brushes.White;
            }
        }

        private void ComboBoxOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (comboBox == ComboBoxOption1)
            {
                colorLabel1.Background = ChangeLabelBackgroundColor(comboBox);
            }
            else if (comboBox == ComboBoxOption2)
            {
                colorLabel2.Background = ChangeLabelBackgroundColor(comboBox);
            }
            else if (comboBox == ComboBoxOption3)
            {
                colorLabel3.Background = ChangeLabelBackgroundColor(comboBox);
            }
            else
            {
                colorLabel4.Background = ChangeLabelBackgroundColor(comboBox);
            }
        }

        private void UpdateAttempts()
        {
            attemptsLabel.Content = $"Attempt: {attempts} / 10";
        }

        private void SetAttemptLabelLayout()
        {
            if (attempts >= 8)
            {
                attemptsLabel.Foreground = Brushes.Red;
                attemptsLabel.FontWeight = FontWeights.Bold;
            }
            else if (attempts >= 5)
            {
                attemptsLabel.Foreground = Brushes.Orange;
                attemptsLabel.FontWeight = FontWeights.DemiBold;
            }
            else
            {
                attemptsLabel.Foreground = Brushes.Black;
                attemptsLabel.FontWeight = FontWeights.Regular;
            }
        }

        private void ClearComboBoxSelection(Label[] labels)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                comboBoxes[i].SelectedValue = null;
                labels[i].BorderBrush = null;
            }
        }

        private string GenerateRandomColor()
        {
            return options[rnd.Next(0, options.Length)];
        }

        private void checkButton_Click(object sender, RoutedEventArgs e)
        {

            if (attempts + 1 != 11)
            {
                CheckCode(ComboBoxOption1, colorLabel1, 0);
                CheckCode(ComboBoxOption2, colorLabel2, 1);
                CheckCode(ComboBoxOption3, colorLabel3, 2);
                CheckCode(ComboBoxOption4, colorLabel4, 3);

                attempts++;
                UpdateAttempts();
                SetAttemptLabelLayout();

                if (attempts + 1 == 11)
                {
                    checkButton.Content = "Game Over";
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Game Over, try again?", "Game over", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    StartGame();
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void CheckCode(ComboBox comboBox, Label colorLabel, int position)
        {
            if (comboBox.Text == null || !solution.Contains(comboBox.Text))
            {
                colorLabel.BorderThickness = new Thickness(0);
            }
            else if (solution.Contains(comboBox.Text) && !ColorInCorrectPosition(comboBox, position))
            {
                colorLabel.BorderBrush = Brushes.Wheat;
                colorLabel.BorderThickness = new Thickness(5);
            }
            else
            {
                colorLabel.BorderBrush = Brushes.DarkRed;
                colorLabel.BorderThickness = new Thickness(5);
            }
        }

        private bool ColorInCorrectPosition(ComboBox comboBox, int position)
        {
            return comboBox.Text == solution[position].ToString() ? true : false;

        }
    }
}
