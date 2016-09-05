using System;

namespace Syringe.Service.Controllers.Hubs
{
    public class CompletedTaskInfo
    {
        public bool Success { get; set; }
        public int Position { get; set; }
    }
}