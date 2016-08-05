using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packsize3
{
    public static class CycleDetector
    {
        public static IEnumerable<Course> DepthFirstTraversal(this Courses courses, Course start)
        {
            var visited = new HashSet<Course>();
            var stack = new Stack<Course>();

            stack.Push(start);

            while (stack.Count != 0)
            {
                var current = stack.Pop();

                if (!visited.Add(current))
                    continue;

                yield return current;

                var neighbours = courses.GetAdjacentCoursesFromCourseName(current.Name)
                                      .Where(n => !visited.Contains(n));

                // If you don't care about the left-to-right order, remove the Reverse
                foreach (var neighbour in neighbours.Reverse())
                    stack.Push(neighbour);
            }
        }
    }
}
