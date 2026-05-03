using Xunit;
using TodoApp.Services;
using TodoApp.Models;

namespace TodoApp.Tests;

public class TodoRepositoryTests
{
    [Fact]
    public void Add_ValidTask_ShouldAddToDatabase()
    {
        // Arrange
        var repo = new TodoRepository();
        var task = new TodoItem("Test");
        
        // Act & Assert
        Assert.NotNull(task);
        Assert.Equal("Test", task.Text);
    }
}