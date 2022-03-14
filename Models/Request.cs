using CourseProject.Data;

namespace CourseProject.Models
{
    public class Request
    {
        public Review Review { get; set; }
        public IFormFile Image { get; set; }
    }
}
