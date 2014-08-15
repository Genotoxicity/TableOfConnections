using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Reflection;
using System.IO;
using KSPE3Lib;

namespace KSP.E3.TableOfConnections
{
    class UI : Window
    {
        private Action<ScriptType> scriptAction;
        private GridLength fullGridLength = new GridLength(100, GridUnitType.Star);
        private GridLength halfGridLength = new GridLength(50, GridUnitType.Star);
        private ScriptType scriptType;

        public UI(E3ApplicationInfo applicationInfo, Action<ScriptType> startScriptAction) : base()
        {
            Menu menu = GetMenu();
            RichTextBox richTextBox = GetRichTextBox();
            GroupBox projectGroup = GetGroupBox("Проект:", richTextBox);
            RadioButton radioButtonConnection = GetRadioButton("По соединениям", "ScriptType", ScriptType.ByConnections);
            RadioButton radioButtonCore = GetRadioButton("По проводам/кабелям", "ScriptType", ScriptType.ByCores);
            radioButtonConnection.IsChecked = true;
            SetElementInGrid(radioButtonConnection, 0, 0);
            SetElementInGrid(radioButtonCore, 0, 1);
            Grid radioButtonsGrid = GetRadioButtonsGrid();
            radioButtonsGrid.Children.Add(radioButtonConnection);
            radioButtonsGrid.Children.Add(radioButtonCore);
            GroupBox radioButtonsGroup = GetGroupBox("Тип скрипта:", radioButtonsGrid);
            Button button = GetButton("Сделать");
            SetElementInGrid(menu, 0, 0);
            SetElementInGrid(projectGroup, 1, 0);
            SetElementInGrid(radioButtonsGroup, 2,0);
            SetElementInGrid(button, 3, 0);
            Grid grid = GetGrid();
            grid.Children.Add(menu);
            grid.Children.Add(button);
            grid.Children.Add(projectGroup);
            grid.Children.Add(radioButtonsGroup);
            Content = grid;
            Title = "Таблица соединений";
            Height = 190;
            Width = 300;
            MinHeight = Height;
            MinWidth = Width;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (applicationInfo.Status == SelectionStatus.Selected)
            {
                richTextBox.AppendText(applicationInfo.MainWindowTitle);
                scriptAction = startScriptAction;
                button.Click += button_Click;
            }
            else
            {
                richTextBox.AppendText(applicationInfo.StatusReasonDescription);
                button.IsEnabled = false;
            }
        }

        private static Menu GetMenu()
        {
            MenuItem instructionMenuItem = new MenuItem();
            instructionMenuItem.Header = "_СПРАВКА";
            instructionMenuItem.Click += instructionMenuItem_Click;
            Menu menu = new Menu();
            menu.IsMainMenu = true;
            menu.Items.Add(instructionMenuItem);
            menu.Background = new SolidColorBrush(Colors.LightGray);
            menu.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            menu.BorderThickness = new Thickness(0, 0, 0, 2);
            return menu;
        }

        private static RichTextBox GetRichTextBox()
        {
            FlowDocument document = new FlowDocument();
            Paragraph paragraph = new Paragraph();
            paragraph.FontSize = 14;
            paragraph.TextAlignment = TextAlignment.Center;
            document.Blocks.Add(paragraph);
            RichTextBox richTextBox = new RichTextBox();
            richTextBox.Document = document;
            richTextBox.IsHitTestVisible = false;
            richTextBox.BorderThickness = new Thickness(0);
            richTextBox.VerticalAlignment = VerticalAlignment.Center;
            return richTextBox;
        }

        private static GroupBox GetGroupBox(string header, object content)
        {
            GroupBox groupBox = new GroupBox();
            groupBox.Header = header;
            groupBox.Content = content;
            groupBox.SnapsToDevicePixels = true;
            groupBox.Margin = new Thickness(0, 1, 0, 0);
            return groupBox;
        }

        private RadioButton GetRadioButton(string text, string group, ScriptType radioButtonScriptType)
        {
            RadioButton radioButton = new RadioButton();
            radioButton.Content = text;
            radioButton.HorizontalAlignment = HorizontalAlignment.Center;
            radioButton.VerticalAlignment = VerticalAlignment.Center;
            radioButton.GroupName = group;
            radioButton.SnapsToDevicePixels = true;
            radioButton.Checked += radioButton_Checked;
            radioButton.Tag = radioButtonScriptType;
            return radioButton;
        }

        private void SetElementInGrid(UIElement element, int row, int column, int rowSpan = 0, int columnSpan = 0)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
            if (rowSpan > 0)
                Grid.SetRowSpan(element, rowSpan);
            if (columnSpan > 0)
                Grid.SetColumnSpan(element, columnSpan);
        }

        private Grid GetRadioButtonsGrid()
        {
            Grid grid = new Grid();
            RowDefinition row = new RowDefinition();
            row.Height = fullGridLength;
            ColumnDefinition leftColumn = new ColumnDefinition();
            ColumnDefinition rightColumn = new ColumnDefinition();
            leftColumn.Width = halfGridLength;
            rightColumn.Width = halfGridLength;
            grid.RowDefinitions.Add(row);
            grid.ColumnDefinitions.Add(leftColumn);
            grid.ColumnDefinitions.Add(rightColumn);
            return grid;
        }

        private Button GetButton(string text)
        {
            Button button = new Button();
            button.Content = text;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.VerticalAlignment = VerticalAlignment.Center;
            button.Height = 25;
            button.Width = 60;
            return button;
        }

        private Grid GetGrid()
        {
            Grid grid = new Grid();
            RowDefinition menuRow = new RowDefinition();
            RowDefinition topRow = new RowDefinition();
            RowDefinition bottomRow = new RowDefinition();
            RowDefinition centerRow = new RowDefinition();
            menuRow.Height = new GridLength(23, GridUnitType.Pixel);
            topRow.Height = fullGridLength;
            centerRow.Height = new GridLength(50, GridUnitType.Pixel);
            bottomRow.Height = new GridLength(40, GridUnitType.Pixel);
            ColumnDefinition column = new ColumnDefinition();
            column.Width = fullGridLength;
            grid.RowDefinitions.Add(menuRow);
            grid.RowDefinitions.Add(topRow);
            grid.RowDefinitions.Add(centerRow);
            grid.RowDefinitions.Add(bottomRow);
            grid.ColumnDefinitions.Add(column);
            return grid;
        }

        private static void ShowHelp()
        {
            string helpFile = Assembly.GetExecutingAssembly().GetName().Name + ".chm";
            if (File.Exists(helpFile))
                System.Windows.Forms.Help.ShowHelp(null, helpFile);
            else
                MessageBox.Show("Файл справки \"" + helpFile + "\" не найден в папке со скриптом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            scriptAction.Invoke(scriptType);
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            scriptType = ((ScriptType)((RadioButton)sender).Tag);
        }

        private static void instructionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowHelp();
        }
    }

}
