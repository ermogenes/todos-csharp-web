using System;
using System.Collections.Generic;

namespace todos.db
{
    public partial class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool Done { get; set; }
    }
}
