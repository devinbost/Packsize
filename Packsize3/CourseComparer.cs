using System.Collections.Generic;

namespace Packsize3
{
    public class CourseComparer : IEqualityComparer<Course>
    {
        public bool Equals(Course c1, Course c2)
        {
            return (c1.Name == c2.Name);
        }

        public int GetHashCode(Course c)
        {
            return c.ID;
        }
    }
}