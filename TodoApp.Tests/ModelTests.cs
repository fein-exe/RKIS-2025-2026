using Xunit;
using TodoApp.Models;

namespace TodoApp.Tests;

public class TodoItemTests
{
    [Fact]
    public void Constructor_CreatesTaskWithNotStartedStatus()
    {
        var item = new TodoItem("Test");
        Assert.Equal("Test", item.Text);
        Assert.Equal(TodoStatus.NotStarted, item.Status);
    }

    [Fact]
    public void UpdateText_ChangesText()
    {
        var item = new TodoItem("Old");
        item.UpdateText("New");
        Assert.Equal("New", item.Text);
    }

    [Fact]
    public void SetStatus_ChangesStatus()
    {
        var item = new TodoItem("Test");
        item.SetStatus(TodoStatus.Completed);
        Assert.Equal(TodoStatus.Completed, item.Status);
    }

    [Theory]
    [InlineData("Short", "Short")]
    [InlineData("This is a very long text that should be truncated", "This is a very long text that ...")]
    public void GetShortInfo_TruncatesLongText(string input, string expected)
    {
        var item = new TodoItem(input);
        Assert.Equal(expected, item.GetShortInfo());
    }
}

public class ProfileTests
{
    [Fact]
    public void Constructor_CreatesProfile()
    {
        var profile = new Profile("user", "pass", "John", "Doe", 1990);
        Assert.Equal("user", profile.Login);
        Assert.Equal("pass", profile.Password);
        Assert.Equal("John", profile.FirstName);
        Assert.Equal("Doe", profile.LastName);
        Assert.Equal(1990, profile.BirthYear);
    }

    [Theory]
    [InlineData(1990, 36)]
    [InlineData(2000, 26)]
    public void GetInfo_ReturnsCorrectAge(int birthYear, int expectedAge)
    {
        var profile = new Profile("user", "pass", "John", "Doe", birthYear);
        Assert.Contains(expectedAge.ToString(), profile.GetInfo());
    }
}

public class TodoStatusTests
{
    [Fact]
    public void EnumHas5Values()
    {
        var values = Enum.GetValues<TodoStatus>();
        Assert.Equal(5, values.Length);
    }

    [Theory]
    [InlineData(TodoStatus.NotStarted)]
    [InlineData(TodoStatus.InProgress)]
    [InlineData(TodoStatus.Completed)]
    [InlineData(TodoStatus.Postponed)]
    [InlineData(TodoStatus.Failed)]
    public void AllStatusesCanBeSet(TodoStatus status)
    {
        var item = new TodoItem("Test");
        item.SetStatus(status);
        Assert.Equal(status, item.Status);
    }
}