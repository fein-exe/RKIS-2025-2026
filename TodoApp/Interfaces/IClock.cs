using System;

namespace TodoApp.Interfaces
{
    public interface IClock
    {
        DateTime Now { get; }
    }
}