using System;
using TodoApp.Interfaces;

namespace TodoApp.Services
{
    public class SystemClock : IClock
    {
        public DateTime Now => DateTime.Now;
    }
}