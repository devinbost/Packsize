using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packsize3
{
    /// <summary>
    /// Each Course is keyed by its name. 
    /// </summary>
    public sealed class Courses : Dictionary<string,Course>, ICourseRepository
    {
        private static int m_Counter = 0;
        private static readonly Courses instance = new Courses();
        private static Dictionary<int, string> _CourseIndexes = new Dictionary<int, string>();
        private Courses() { }

        public static Courses Instance
        {
            get
            {
                return instance;
            }
        }

        public static Dictionary<int, string> CourseIndexes
        {
            get { return _CourseIndexes; }
        }

        public void AddCourse(Course course)
        {
            this.AddCourse(course, ThrowErrorOnDuplicate.No);
        }
        
        /// <summary>
        /// Use this method to add a new course to the set of Courses. 
        /// If the course has already been added, this method does nothing and returns no error.
        /// We could change this method to take an enum to throw an error if desired.
        /// </summary>
        /// <param name="course"></param>
        /// <param name="throwErrorOnDuplicate"></param>
        public void AddCourse(Course course, ThrowErrorOnDuplicate throwErrorOnDuplicate = ThrowErrorOnDuplicate.No)
        {
           this.AddCourseInstance(course, throwErrorOnDuplicate);
           this.AddCourseDependencies(course);
        }

        private void AddCourseInstance(Course course, ThrowErrorOnDuplicate throwErrorOnDuplicate = ThrowErrorOnDuplicate.No)
        {
            if (!CourseAlreadyAdded(course.Name))
            {
                course.ID = System.Threading.Interlocked.Increment(ref m_Counter);
                base.Add(course.Name, course);
                CourseIndexes.Add(course.ID, course.Name); // TO DO: Writes need to be made thread-safe.
            }
            else
            {
                if (base[course.Name].DependencyString == null)
                {
                    if (course.DependencyString != null)
                    {
                        base[course.Name].DependencyString = course.DependencyString;
                    }
                }
                if (throwErrorOnDuplicate == ThrowErrorOnDuplicate.Yes)
                {
                    throw new DuplicateNameException("This course has already been added!");
                }
            }
        }
        private void AddCourseDependencies(Course course)
        {
            if (course.HasDependencies)
            {
                course.GetDependentCourseNames().ToList().ForEach(t =>
                {
                    if (!CourseAlreadyAdded(t))
                    {
                        var newCourse = new Course(t);
                        this.AddCourse(newCourse);
                    }
                });
            }
            
        }
        [Obsolete("Use AddCourse(Course course, ThrowErrorOnDuplicate throwErrorOnDuplicate) instead if possible.")]
        public new void Add(string courseName, Course course)
        { // The goal here is to hide the underlying dictionary's method for adding a key,value pair because we want to control what the key is.
            // Alternatively, we could have implemented IDictionary, but that turned out to be more work than it seemed to be worth.
           AddCourse(course); 
        }
     
        public bool CourseAlreadyAdded(Course course)
        {
            return CourseAlreadyAdded(course.Name);
        }
        public bool CourseAlreadyAdded(string courseName)
        {
            if (base.ContainsKey(courseName))
            {
                return true;
            }
            return false;
        }

        // We need methods that get the IDs of the set of dependent courses of a given course.
        // These IDs will be used as input for constructing the graph relationships.
        public int GetCourseIdFromName(string courseName, ThrowErrorOnMissing throwErrorOnMissing = ThrowErrorOnMissing.Yes)
        {
            if (!CourseAlreadyAdded(courseName)) // i.e. if Course is missing from Courses.
            {
                if (throwErrorOnMissing == ThrowErrorOnMissing.Yes)
                {
                    throw new NullReferenceException(courseName + " has not been added to Courses!");   
                }
                return -1;
            }
            else
            {
                return this[courseName].ID;
            }
        }

        public string[] GetAdjacentCourseNamesFromCourseName(string courseName, ThrowErrorOnMissing throwErrorOnMissing = ThrowErrorOnMissing.Yes)
        {
            if (!CourseAlreadyAdded(courseName)) // i.e. if Course is missing from Courses.
            {
                if (throwErrorOnMissing == ThrowErrorOnMissing.Yes)
                {
                    throw new NullReferenceException(courseName + " has not been added to Courses!");
                }
                return new string[] {};
            }
            else
            {
                if (this[courseName].HasDependencies)
                {
                    return this[courseName].GetDependentCourseNames();
                }
                return new string[] { };
            }
        }
        public IEnumerable<Course> GetAdjacentCoursesFromCourseName(string courseName, ThrowErrorOnMissing throwErrorOnMissing = ThrowErrorOnMissing.Yes)
        {
            if (!CourseAlreadyAdded(courseName)) // i.e. if Course is missing from Courses.
            {
                if (throwErrorOnMissing == ThrowErrorOnMissing.Yes)
                {
                    throw new NullReferenceException(courseName + " has not been added to Courses!");
                }
                return new List<Course>();
            }
            else
            {
                if (this[courseName].HasDependencies)
                {
                    return this[courseName].GetDependentCourses(this);
                }
                return new List<Course>();
            }
        }

        public IEnumerable<int> GetAdjacentCourseIDsFromCourseName(string courseName, ThrowErrorOnMissing throwErrorOnMissing = ThrowErrorOnMissing.Yes)
        {
            if (!CourseAlreadyAdded(courseName)) // i.e. if Course is missing from Courses.
            {
                if (throwErrorOnMissing == ThrowErrorOnMissing.Yes)
                {
                    throw new NullReferenceException(courseName + " has not been added to Courses!");
                }
                return new List<int> { };
            }
            else
            {
                if (this[courseName].HasDependencies)
                {
                    return this[courseName].GetDependentCourseIDs(this);
                }
                return new List<int> { };
            }
        }

        public IEnumerable<int> GetAdjacentCourseIDsFromCourseId(int courseId, ThrowErrorOnMissing throwErrorOnMissing = ThrowErrorOnMissing.Yes)
        {
            if (!CourseIndexes.ContainsKey(courseId))
            {
                if (throwErrorOnMissing == ThrowErrorOnMissing.Yes)
                {
                    throw new NullReferenceException(courseId + " has not been added to Course indexes!");
                }
                return new List<int> { };
            }
            else
            {
                var courseName = CourseIndexes[courseId];
                var course = this[courseName];
                if (course.HasDependencies)
                {
                    return this[courseName].GetDependentCourseIDs(this);
                }
                return new List<int> { };
            }
        }
        public void ClearData()
        {
            this.Clear();
            CourseIndexes.Clear();
            m_Counter = 0;
        }

        public void getCycles()
        {
            foreach (var course in Values)
            {
                
            }
        }

        public List<List<Course>> GetAcyclicCourseLists()
        {
            var lists = new List<List<Course>>();
            foreach (var course in this)
            {
                var myList = this.DepthFirstTraversal(course.Value).ToList();
                if (!myList.Last().GetDependentCourseNames().Contains(myList.First().Name))
                {
                    myList.Reverse();
                    lists.Add(myList);
                }
            }
            return lists;
        }

        public List<List<Course>> GetDistinctAcyclicCourseLists()
        {
            var lists = new List<List<Course>>();
            var filteredLists = new List<List<Course>>();
            lists = this.GetAcyclicCourseLists();
            var grouped = lists
                .GroupBy(t => t[0].ID)
                .Select(t => t.ToList())
                .ToList();
            foreach (var group in grouped)
            {
                var resultList = new List<Course>();
                foreach (var list in group)
                {
                    resultList = resultList.Union(list, new CourseComparer()).ToList();
                }
                filteredLists.Add(resultList);
            }
            return filteredLists;
        }

        public string ConcatenateCourseLists()
        {
            var stringList = new List<string>();
            foreach (var list in this.GetDistinctAcyclicCourseLists())
            {
                var csv = string.Join(", ", list.Select(t => t.Name));
                stringList.Add(csv);
            }
            var finalCsv = string.Join(", ", stringList);
            return finalCsv;
        }
    }
}
