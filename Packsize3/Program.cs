using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packsize3
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // We're assuming that the courses are provided as separate arguments in the input. 
            var courses = Courses.Instance;
            foreach (var courseName in args)
            {
                var course = new Course(courseName);
                courses.AddCourse(course);
            }

            Console.WriteLine(courses.ConcatenateCourseLists());
            
        }
    }
}
