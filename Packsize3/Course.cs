using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packsize3
{
    
    [DebuggerDisplay("Name = {Name}, DependencyString = {DependencyString}")]
    public class Course
    {
        public Course()
        {
        }

        private string _dependencyString;
        private string _name;
        public int ID { get; set; }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value.Trim(); }
        }

        public virtual string DependencyString
        {
            get
            {
                if (string.IsNullOrEmpty(_dependencyString))
                {
                    return "";
                }
                return _dependencyString;
            }
            set { _dependencyString = value.Trim(); }
        }

        public bool HasDependencies
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(this.DependencyString))
                {
                    return true;
                }
                return false;
            }
        } // could return false if DependencyString is null or empty string.

        //public Course(string name, string dependencyString)
        //{
        //    Name = name;
        //    DependencyString = dependencyString;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameString">This string is expected in the "name: dep1, dep2,..., depN" format.</param>
        public Course(string nameString)
        {
            // An interpreter should be used to parse the nameString... This could be injected via Method Injection.

            // Check if ":" character exists in string.
            // If so, split string on ":" character
            // set variable "courseName" to index 0 of resulting array.
            var doesCourseDelimiterExist = nameString.Contains(':');
            //if (doesCourseDelimiterExist)
            //{
            var courseNameParts = nameString.Split(':');
            this.Name = courseNameParts[0];
            if (courseNameParts.Length == 1)
            {
                this.Name = courseNameParts[0];
            }
            if (courseNameParts.Length > 1 && courseNameParts.Length <= 2) // Then there may be dependencies.
            {
                this.Name = courseNameParts[0];
                this.DependencyString = courseNameParts[1];
            }
            if (courseNameParts.Length > 2)
            {
                throw new ArgumentException(nameString + " is in an invalid format! Too many ':' characters!");
            }
            //}
            //else
            //{
            //    // else, assume that the string format is an error. (We could also provide an emum to make this error throwing optional.)
            //    throw new FormatException(nameString + " is not in the correct format. Expecting \"name: dep1, dep2,..., depN\" format");
            //}
            // We could create a property that would store a list of Courses that are dependent, but that would waste memory 
            // and have other issues and would be better replaced with a lazy lookup to a Singleton or Repository with data storage.
            // This could be implemented with an extension method to keep separation of concerns.
        }

        public string[] GetDependentCourseNames()
        {
            if (this.DependencyString.Contains(',')) // Then more than one course is in the csv list.
            {
                // ideally, we should use a regular expression to check to ensure that the comma is not inside of a string...
                var dependentCourses = this.DependencyString.Split(',').Select(t => t.Trim()).ToArray();
                return dependentCourses;

            }
            return new string[] { this.DependencyString };
        }

        public IEnumerable<int> GetDependentCourseIDs(Courses courses)
        {
            var dependentCourseNames = this.GetDependentCourseNames().ToList().Select(t => courses.GetCourseIdFromName(t));
            return dependentCourseNames;
        }
        public IEnumerable<Course> GetDependentCourses(Courses courses)
        {
            var dependentCourseNames = this.GetDependentCourseNames().ToList().Select(t => courses[t]);
            return dependentCourseNames;
        }
        /// <summary>
        /// We can follow the Mediator pattern if we inject an abstract Mediator class instead of the Courses repository.
        /// </summary>
        /// <param name="courses"></param>
        public void Save(Courses courses)
        {
            courses.AddCourse(this);
        }
    }

    //public abstract class CourseMediator
    //{
    //    public virtual void Save();
    //}
}
