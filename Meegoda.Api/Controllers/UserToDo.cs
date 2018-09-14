using System;

namespace Meegoda.Api.Controllers
{
    public class UserToDo
    {
        public int TodoId { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string TodoTitle { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
    }
}