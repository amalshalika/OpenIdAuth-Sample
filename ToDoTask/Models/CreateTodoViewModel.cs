using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoTask.Models
{
    public class CreateTodoViewModel
    {
        public string OwnerName { get; set; }
        public string TodoTitle { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
    }
}
