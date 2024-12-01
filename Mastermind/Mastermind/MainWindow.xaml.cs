using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Mastermind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StringBuilder sb = new StringBuilder();
        Random rnd = new Random();
        ComboBox[] comboBoxes;
        Label[] labels;
        string[] imagePaths;
        BitmapImage[] imageArray = new BitmapImage[6];
        BitmapImage[] solutionImages = new BitmapImage[4];
        Brush[] solutionBrushes = new Brush[4];
        string[] colors = new string[4];
        string[] solution = new string[4];
        string[] options = { "Bulbasaur", "Charmander", "Eevee", "Meowth", "Pikachu", "Squirtle" };
        int attempts, currentRow, score;
        bool debugMode, hasWon, forceQuit;

        public MainWindow()
        {
            InitializeComponent();
            comboBoxes = new ComboBox[4] { ComboBoxOption1, ComboBoxOption2, ComboBoxOption3, ComboBoxOption4 };
            comboBoxes = AddComboBoxItems(comboBoxes);
            labels = new Label[4] { colorLabel1, colorLabel2, colorLabel3, colorLabel4, };
        }

        /// <summary>
        /// Handles the Window Loaded event. Initializes game data and loads images from the assets folder.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">Event data for the Loaded event.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartGame();

            // Get pictures from assets folder and put them in the BitmapImages array.
            imagePaths = Directory.GetFiles("../../assets", "*.png");

            for (int i = 0; i < imagePaths.Length; i++)
            {
                imageArray[i] = new BitmapImage(new Uri(imagePaths[i], UriKind.Relative));
            }
        }

        /// <summary>
        /// Initializes and starts a new game, resetting relevant variables and UI elements.
        /// </summary>
        private void StartGame()
        {
            attempts = 0;
            currentRow = 0;
            score = 100;
            debugMode = false;
            forceQuit = false;
            solutionTextBox.Visibility = Visibility.Hidden;
            hasWon = false;
            InitalizeColors();

            for (int i = 0; i < solutionImages.Length; i++)
            {
                solutionImages[i] = new BitmapImage();
            }

            UpdateLabels();
            ClearComboBoxSelection(labels);
            checkButton.Content = "Check code";
        }

        /// <summary>
        /// Adds the selectable items to the ComboBoxes.
        /// </summary>
        /// <param name="comboBoxes">An array of ComboBox elements to populate with selectable options.</param>
        /// <returns>An array of ComboBox elements with items added.</returns>
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

        /// <summary>
        /// Initializes the colors used for the solution by generating random colors.
        /// </summary>
        private void InitalizeColors()
        {
            GenerateRandomColor();
        }

        /// <summary>
        /// Toggles debug mode when the Control + F12 key combination is pressed.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The Key_Down event</param>
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

        /// <summary>
        /// Changes the background image of a label based on the ComboBox selection.
        /// </summary>
        /// <param name="ComboBox">The ComboBox whose selection determines the label background image.</param>
        /// <returns>A BitmapImage representing the selected background image.</returns>
        private BitmapImage ChangeLabelBackgroundColor(ComboBox ComboBox)
        {
            switch (ComboBox.SelectedIndex)
            {
                case 0:
                    return imageArray[0];
                case 1:
                    return imageArray[1];
                case 2:
                    return imageArray[2];
                case 3:
                    return imageArray[3];
                case 4:
                    return imageArray[4];
                case 5:
                    return imageArray[5];
                default:
                    return null;
            }
        }

        /// <summary>
        /// Updates the background of labels based on the corresponding selected ComboBox option.
        /// </summary>
        /// <param name="sender">The event sender (ComboBox).</param>
        /// <param name="e">The changing of the selected item of the ComboBox</param>
        private void ComboBoxOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (comboBox != null && comboBox.SelectedIndex >= 0 && comboBox.SelectedIndex < imageArray.Length)
            {
                if (comboBox == ComboBoxOption1)
                {
                    ImageBrush brush1 = new ImageBrush();
                    brush1.ImageSource = imageArray[comboBox.SelectedIndex];
                    colorLabel1.Background = brush1;
                }
                else if (comboBox == ComboBoxOption2)
                {
                    ImageBrush brush2 = new ImageBrush();
                    brush2.ImageSource = imageArray[comboBox.SelectedIndex];
                    colorLabel2.Background = brush2;
                }
                else if (comboBox == ComboBoxOption3)
                {
                    ImageBrush brush3 = new ImageBrush();
                    brush3.ImageSource = imageArray[comboBox.SelectedIndex];
                    colorLabel3.Background = brush3;
                }
                else
                {
                    ImageBrush brush4 = new ImageBrush();
                    brush4.ImageSource = imageArray[comboBox.SelectedIndex];
                    colorLabel4.Background = brush4;
                }
            }
            else
            {
                colorLabel1.Background = null;
                colorLabel2.Background = null;
                colorLabel3.Background = null;
                colorLabel4.Background = null;
            }
        }

        /// <summary>
        /// Updates the labels displaying the player's attemps and score.
        /// </summary>
        private void UpdateLabels()
        {
            attemptsLabel.Content = $"Attempt: {attempts} / 10";
            attemptsLabel.Foreground = attempts >= 8 ? Brushes.Red : attempts >= 5 ? Brushes.Orange : Brushes.Black;
            attemptsLabel.FontWeight = attempts >= 8 ? FontWeights.Bold : attempts >= 5 ? FontWeights.DemiBold : FontWeights.Normal;
            scoreLabel.Content = $"Score: {score} / 100";
            solutionTextBox.Text = String.Join(", ", solution);
        }

        /// <summary>
        /// Clears the selection of all ComboBoxes and resets the corresponding label borders.
        /// </summary>
        /// <param name="labels">An array of Label elements to reset.</param>
        private void ClearComboBoxSelection(Label[] labels)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                comboBoxes[i].SelectedValue = 2;
                labels[i].BorderBrush = null;
            }
        }

        /// <summary>
        /// Generates a random solution for the game by selecting random images within the amount of options.
        /// </summary>
        private void GenerateRandomColor()
        {
            for (int i = 0; i < 4; i++)
            {
                int random = rnd.Next(0, imageArray.Length);
                solution[i] = options[random];
                solutionImages[i] = imageArray[random];
            }
        }

        /// <summary>
        /// Handles the Check button click event to validate the player's guess and update the game state.
        /// </summary>
        /// <param name="sender">The button on which the user clicks.</param>
        /// <param name="e">The actual click of the button.</param>
        private void checkButton_Click(object sender, RoutedEventArgs e)
        {
            if (attempts + 1 != 11)
            {
                CheckIfPlayerHasWon();
                attempts++;
                CreateRow();
                UpdateLabels();

                if (attempts + 1 == 11 && !hasWon)
                {
                    checkButton.Content = "Game Over";
                    MessageBoxResult result = MessageBox.Show($"Game Over.\nThe code was:\n{String.Join(", ", solution)}\n\nTry again?", "Game over", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        ClearGridSection();
                        StartGame();
                    }
                    else
                    {
                        forceQuit = true;
                        this.Close();
                    }
                }
                else if (hasWon)
                {
                    checkButton.Content = "Victory";
                    forceQuit = true;
                    MessageBoxResult result = MessageBox.Show($"You won in {attempts} attempts, play again?", "You won", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        ClearGridSection();
                        StartGame();
                    }
                    else
                    {
                        this.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Checks the player's code by comparing the selected text in the ComboBox with the solution.
        /// Updates the image label's border and subtracts score based on the correctness of the guess.
        /// </summary>
        /// <param name="combobox">The ComboBox containing the player's selected color.</param>
        /// <param name="imageLabel">The Label representing the image slot in the game UI.</param>
        /// <param name="position">The position of the ComboBox in the solution sequence.</param>
        private void CheckCode(ComboBox combobox, Label imageLabel, int position)
        {
            if (combobox.Text == null || !solution.Contains(combobox.Text))
            {
                score -= 2;
                imageLabel.BorderThickness = new Thickness(0);
            }
            else if (solution.Contains(combobox.Text) && !ColorInCorrectPosition(combobox, position))
            {
                score -= 1;
                imageLabel.BorderBrush = Brushes.Wheat;
                imageLabel.BorderThickness = new Thickness(2);
            }
            else
            {
                imageLabel.BorderBrush = Brushes.DarkRed;
                imageLabel.BorderThickness = new Thickness(2);
            }
        }

        /// <summary>
        /// Checks if the text selected in the ComboBox is in the correct position within the solution.
        /// </summary>
        /// <param name="combobox">The ComboBox containing the player's selected color in text.</param>
        /// <param name="position">The expected position of the color in the solution sequence.</param>
        /// <returns>True if the color is in the correct position; otherwise, false.</returns>
        private bool ColorInCorrectPosition(ComboBox combobox, int position)
        {
            return combobox.Text == solution[position];
        }

        /// <summary>
        /// Creates a new row in the game history grid to display the player's guesses.
        /// Calls CheckCode() to validate each guess and updates the grid accordingly.
        /// </summary>
        private void CreateRow()
        {
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = GridLength.Auto;
            HistoryGrid.RowDefinitions.Add(rowDefinition);

            for (int i = 0; i < 4; i++)
            {
                ComboBox combobox = comboBoxes[i];
                Label playerGuess = new Label();
                playerGuess.Background = labels[i].Background;
                playerGuess.Height = 64;
                playerGuess.Width = 64;
                playerGuess.Margin = new Thickness(1);

                Grid.SetRow(playerGuess, currentRow);
                Grid.SetColumn(playerGuess, i);

                HistoryGrid.Children.Add(playerGuess);

                CheckCode(combobox, playerGuess, i);
            }
            currentRow++;
        }

        /// <summary>
        /// Clears the game history grid by removing all row definitions and child elements.
        /// This method resets the grid section, preparing it for a new game.
        /// </summary>
        private void ClearGridSection()
        {
            HistoryGrid.RowDefinitions.Clear();
            HistoryGrid.Children.Clear();
        }

        /// <summary>
        /// Checks if the player has guessed the correct solution sequence.
        /// </summary>
        /// <returns>True if the player's guess matches the solution; otherwise, false.</returns>
        private bool CheckIfPlayerHasWon()
        {
            if (ComboBoxOption1.Text == solution[0] &&
                ComboBoxOption2.Text == solution[1] &&
                ComboBoxOption3.Text == solution[2] &&
                ComboBoxOption4.Text == solution[3])
            {
                return hasWon = true;
            }
            else
            {
                return hasWon = false;
            }
        }

        /// <summary>
        /// Handles the Window Closing event. Displays a confirmation message if the player tries to quit before finishing the game.
        /// </summary>
        /// <param name="sender">The window that gets closed.</param>
        /// <param name="e">The actual clicking on the closing button of the window.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!forceQuit)
            {
                MessageBoxResult answer = MessageBox.Show($"Would you like to quit the game?\nYou still have {10 - attempts} attempts left.", "Exit Game", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (answer == MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            } else
            {
                e.Cancel = false;
            }

        }
    }
}
