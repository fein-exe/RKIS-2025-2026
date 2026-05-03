using TodoApp.Models;

namespace TodoApp.Desktop.ViewModels;

public class StatusFilterOption
{
    public StatusFilterOption(string title, TodoStatus? status)
    {
        Title = title;
        Status = status;
    }

    public string Title { get; }

    public TodoStatus? Status { get; }
}
