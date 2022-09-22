
using ZaminEducation.Domain.Commons;

namespace ZaminEducation.Domain.Entities.Courses
{
    public class CourseTarget : Auditable
    {
        public string Name { get; set; }
        public long CourseId { get; set; }
        public Course Course { get; set; }
    }
}