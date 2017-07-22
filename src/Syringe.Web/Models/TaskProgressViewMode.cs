using System;

namespace Syringe.Web.Controllers
{
    public class TaskProgressViewMode
    {
        public int TaskId { get; set; }
        public int CurrentItem { get; set; }
        public int TotalTests { get; set; }
        public bool IsFinished { get; set; }
        public Guid? ResultGuid { get; set; }
    }
}