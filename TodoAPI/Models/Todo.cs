using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class Todo
    {        
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public bool IsComplete { get; set; } = false;

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
