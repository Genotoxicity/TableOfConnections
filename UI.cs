using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using KSPE3Lib;

namespace KSP.E3.TableOfConnections
{
    class UI : Window
    {
        private Grid grid;
        private Label label;
        private Button button;
        private TextBlock block;
        private Action scriptAction;

        public UI(Project project, Action action) : base()
        {
            grid = GetGrid();
            button = GetButton("Сделать");
            block = GetTextBlock();
            label = GetLabel(block);
            SetElementInGrid(button, 1, 0);
            SetElementInGrid(label, 0, 0);
            Content = grid;
            Title = "Таблица соединений";
            Height = 140;
            Width = 250;
            MinHeight = Height;
            MinWidth = Width;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (project.Status == Status.Selected)
            {
                block.Text = project.Name;
                scriptAction = action;
                button.Click += button_Click;
            }
            else
            {
                block.Text = "Нет выбранных проектов" + Environment.NewLine + "Откройте проекты и перезапустите скрипт";
                button.IsEnabled = false;
            }
        }

        private Grid GetGrid()
        {
            Grid grid = new Grid();
            RowDefinition topRow = new RowDefinition();
            RowDefinition bottomRow = new RowDefinition();
            topRow.Height = new GridLength(100, GridUnitType.Star);
            bottomRow.Height = new GridLength(60, GridUnitType.Pixel);
            ColumnDefinition column = new ColumnDefinition();
            column.Width = new GridLength(100, GridUnitType.Star);
            grid.RowDefinitions.Add(topRow);
            grid.RowDefinitions.Add(bottomRow);
            grid.ColumnDefinitions.Add(column);
            return grid;
        }

        private Button GetButton(string text)
        {
            Button button = new Button();
            button.Content = text;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.VerticalAlignment = VerticalAlignment.Center;
            button.Height = 30;
            button.Width = 60;
            return button;
        }

        private Label GetLabel(TextBlock block)
        {
            Label label = new Label();
            label.HorizontalContentAlignment = HorizontalAlignment.Center;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.Content = block;
            return label;
        }

        private TextBlock GetTextBlock()
        { 
            TextBlock block = new TextBlock();
            block.TextTrimming = TextTrimming.CharacterEllipsis;
            block.TextAlignment = TextAlignment.Center;
            return block;
        }

        private void SetElementInGrid(UIElement element, int row, int column, int rowSpan = 0, int columnSpan = 0)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
            if (rowSpan > 0)
                Grid.SetRowSpan(element, rowSpan);
            if (columnSpan > 0)
                Grid.SetColumnSpan(element, columnSpan);
            grid.Children.Add(element);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            scriptAction.Invoke();
        }

    }

}
