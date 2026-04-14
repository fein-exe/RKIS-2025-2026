using System;
using System.Text;
using TodoApp.Exceptions;
using TodoApp.Services;

namespace TodoApp.Commands
{
    public class ViewCommand : ICommand
    {
        private bool _showIndex;
        private bool _showStatus;
        private bool _showDate;

        public ViewCommand(bool showIndex = false, bool showStatus = false, bool showDate = false)
        {
            _showIndex = showIndex;
            _showStatus = showStatus;
            _showDate = showDate;
        }

        public void Execute()
        {
            if (AppInfo.CurrentProfile == null)
            {
                throw new AuthenticationException("Пользователь не авторизован");
            }

            var todos = AppInfo.GetCurrentTodos();
            if (todos.Count == 0)
            {
                Console.WriteLine("Список задач пуст.");
                return;
            }

            var table = new StringBuilder();
            int indexWidth = 5;
            int textWidth = 35;
            int statusWidth = 15;
            int dateWidth = 20;

            var columns = new System.Collections.Generic.List<int>();
            var headers = new System.Collections.Generic.List<string>();

            if (_showIndex)
            {
                columns.Add(indexWidth);
                headers.Add("INDEX");
            }

            columns.Add(textWidth);
            headers.Add("ЗАДАЧА");

            if (_showStatus) 
            { 
                columns.Add(statusWidth); 
                headers.Add("СТАТУС"); 
            }
            if (_showDate) 
            { 
                columns.Add(dateWidth); 
                headers.Add("ДАТА"); 
            }

            string BuildLine(char left, char mid, char right, char fill)
            {
                var sb = new StringBuilder();
                sb.Append(left);
                for (int i = 0; i < columns.Count; i++)
                {
                    sb.Append(new string(fill, columns[i] + 2));
                    sb.Append(i < columns.Count - 1 ? mid : right);
                }
                return sb.ToString();
            }

            table.AppendLine(BuildLine('╔', '╦', '╗', '═'));

            table.Append('║');
            for (int i = 0; i < columns.Count; i++)
                table.Append($" {headers[i].PadRight(columns[i])} ║");
            table.AppendLine();

            table.AppendLine(BuildLine('╠', '╬', '╣', '═'));

            int index = 0;
            foreach (var item in todos)
            {
                table.Append('║');
                int col = 0;

                if (_showIndex)
                    table.Append($" {(index + 1).ToString().PadRight(columns[col++])} ║");

                string shortText = item.GetShortInfo();
                table.Append($" {shortText.PadRight(columns[col++])} ║");

                if (_showStatus)
                    table.Append($" {item.Status.ToString().PadRight(columns[col++])} ║");

                if (_showDate)
                    table.Append($" {item.LastUpdate.ToString("yyyy-MM-dd HH:mm").PadRight(columns[col++])} ║");

                table.AppendLine();

                if (index < todos.Count - 1)
                    table.AppendLine(BuildLine('╠', '╬', '╣', '═'));

                index++;
            }

            table.AppendLine(BuildLine('╚', '╩', '╝', '═'));

            Console.WriteLine(table.ToString());
        }
    }
}