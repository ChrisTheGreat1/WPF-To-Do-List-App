using System;

namespace ToDo_App.Models
{
    public class ToDoListItem
    {
        public ToDoListItem()
        {
            CreatedDateTime = DateTime.Now;
        }

        public string? Contents { get; set; }
        public DateTime CreatedDateTime { get; private set; }
        public DateTime? Date { get; set; }
        public int Id { get; private set; } // This field is set automatically by SQLite db upon insertion of new item
        public string? Title { get; set; }
    }
}