using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit;
using NUnit.Framework;
using Packsize3;
using System.Data;
using System.Linq;

namespace Packsize3.Test
{
    [TestFixture]
    public class CourseTests
    {

        [TearDown]
        public void ClearData()
        {
            var courses = Courses.Instance;
            courses.ClearData();
        }
        
        [TestCase]
        public void Course_ConstructorReturnsValidObject()
        {
            var courseName = "Introduction to Paper Airplanes";
            var course = new Course(courseName);
            Assert.AreEqual(courseName, course.Name);

        }
        [TestCase]
        public void Course_GetExpectedCourseNameFromEntireString_ReturnsExpectedValue()
        {
            //Courses_ReturnsValidInstance();
            //Course_ConstructorReturnsValidObject();
            var testCourse = new Course("Advanced Throwing Techniques: Introduction to Paper Airplanes");
            Assert.AreEqual(testCourse.Name, "Advanced Throwing Techniques");
        }

        [TestCase]
        public void Course_GetDependentCourseNames_ReturnsExpectedValue()
        {
            //Courses_ReturnsValidInstance();
            //Course_ConstructorReturnsValidObject();
            var testCourse = new Course("Advanced Throwing Techniques: Introduction to Paper Airplanes");
            var dependentCourseNames = testCourse.GetDependentCourseNames();
            // The next step is that we need to eliminate whitespace around the string.... What's the best way to match ? A regex?
            var expectedDependentCourseNames = new string[] {"Introduction to Paper Airplanes"};
            Assert.AreEqual(dependentCourseNames, expectedDependentCourseNames);
        }
        
        [TestCase]
        public void Course_HasDependencies_ReturnsTrueWhenExpected()
        {
            var course = Substitute.For<Course>();
            course.DependencyString.Returns("Introduction to Paper Airplanes");
            Assert.AreEqual(true, course.HasDependencies);
        }

        [TestCase]
        public void Course_HasDependencies_ReturnsFalseWhenWhitespace()
        {
            var course = Substitute.For<Course>();
            course.DependencyString.Returns("   ");
            Assert.AreEqual(false, course.HasDependencies);
        }

        [TestCase]
        public void Program_TestEntireExecution()
        {
            var courses = Courses.Instance;
            var courseName = "Advanced Throwing Techniques: Introduction to Paper Airplanes";
           var course = new Course(courseName);
            courses.AddCourse(course);

            var output = courses.ConcatenateCourseLists();
            Console.WriteLine(output);

        }
        
    }

    [TestFixture]
    public class CoursesTests
    {

       
        [TestCase]
        public void Courses_ReturnsValidInstance()
        {
            var instance = Courses.Instance;
            Assert.IsInstanceOf<Courses>(instance);
        }

        [TearDown]
        public void ClearData()
        {
            var courses = Courses.Instance;
            courses.ClearData();
        }
        [TestCase]
        public void Courses_CourseAlreadyAdded()
        {
            var courseStub = Substitute.For<Course>();
            var courseName = "Introduction to Paper Airplanes";
            courseStub.Name.Returns(courseName);

            var courses = Courses.Instance;
            Assert.AreEqual(false, courses.CourseAlreadyAdded(courseName));
            courses.AddCourse(courseStub);
            Assert.AreEqual(true, courses.CourseAlreadyAdded(courseName));

        }
        [TestCase]
        public void Courses_ThrowsExceptionOnDuplicateCourse()
        {
            var testCourse1 = Substitute.For<Course>();
            var testCourse2 = Substitute.For<Course>();
            testCourse1.Name.Returns("Advanced Throwing Techniques");
            testCourse2.Name.Returns("Advanced Throwing Techniques");
            var courses = Courses.Instance;
            courses.AddCourse(testCourse1);
            Assert.Throws<DuplicateNameException>(
                () => { courses.AddCourse(testCourse2, Packsize3.ThrowErrorOnDuplicate.Yes); });


        }
        [TestCase]
        public void Courses_GetCourseIdFromName_ThrowsErrorOnMissing()
        {
            var course1 = Substitute.For<Course>();
            var courseName1 = "Introduction to Paper Airplanes";
            course1.Name.Returns(courseName1);
            var courses = Courses.Instance;
            Assert.Throws<NullReferenceException>(() => courses.GetCourseIdFromName(courseName1));
        }

        [TestCase]
        public void Courses_CourseIndexAddedWhenCourseIsAdded_IndexIsCreatedInCourseIndexes()
        {
            var course1 = Substitute.For<Course>();
            var course2 = Substitute.For<Course>();
            var courseName1 = "Introduction to Paper Airplanes";
            var courseName2 = "Advanced Throwing Techniques";
            course1.Name.Returns(courseName1);
            course2.Name.Returns(courseName2);
            var courses = Courses.Instance;
            courses.ClearData();
            courses.AddCourse(course1);
            courses.AddCourse(course2);
            var courseID1 = courses.GetCourseIdFromName(courseName1);
            var courseID2 = courses.GetCourseIdFromName(courseName2);
            Assert.AreEqual(1, courseID1);
            Assert.AreEqual(2, courseID2);
        }

        [TestCase]
        public void Courses_AddCourseAndDependencies_DependenciesCreatedAsExpected()
        {
            var course1 = Substitute.For<Course>();
            var course2 = Substitute.For<Course>();
            var courseName1 = "Introduction to Paper Airplanes";
            var courseName2 = "Advanced Throwing Techniques";
            course1.Name.Returns(courseName1);
            course1.DependencyString.Returns(courseName2);
            course2.Name.Returns(courseName2);
            var courses = Courses.Instance;
            courses.AddCourse(course1);
            // Since course2 is a dependency of course1, it should get added once we add course1
            var courseID1 = courses.GetCourseIdFromName(courseName1);
            var courseID2 = courses.GetCourseIdFromName(courseName2);
            Assert.AreEqual(1, courseID1);
            Assert.AreEqual(2, courseID2);
        }

        [TestCase]
        public void Courses_GetAdjacentCoursesFromCourseName_ReturnsExpectedCourses()
        {
            var courseStrings = new string[]
            {
                "Introduction to Paper Airplanes:", "Advanced Throwing Techniques: Introduction to Paper Airplanes",
                "History of Cubicle Siege Engines: Rubber Band Catapults 101",
                "Advanced Office Warfare: History of Cubicle Siege Engines ", "Rubber Band Catapults 101: ",
                "Paper Jet Engines: Introduction to Paper Airplanes"
            };
            var courses = Courses.Instance;
            foreach (var str in courseStrings)
            {
                var course = new Course(str);
                courses.AddCourse(course);
            }
            var expected = new string[] { "Introduction to Paper Airplanes" };
            var resulting = courses.GetAdjacentCourseNamesFromCourseName("Advanced Throwing Techniques");
            Assert.AreEqual(expected, resulting);
        }

        [TestCase]
        public void Courses_GetDistinctAcyclicCourseLists_ReturnsExpectedLists()
        {
            var courseStringsWithCycles = new[]
            {
                "Introduction to Paper Airplanes: Rubber Band Catapults 101",
                "Advanced Throwing Techniques: Introduction to Paper Airplanes",
                "History of Cubicle Siege Engines: ",
                "Advanced Office Warfare: History of Cubicle Siege Engines ", "Rubber Band Catapults 101: ",
                "Paper Jet Engines: Introduction to Paper Airplanes", "Intro to Arguing on the Internet: Godwin's Law",
                "Understanding Circular Logic: Intro to Arguing on the Internet",
                "Godwin's Law: Understanding Circular Logic"
            };
            var courses = Courses.Instance;
            courses.ClearData();
            foreach (var str in courseStringsWithCycles)
            {
                var course = new Course(str);
                courses.AddCourse(course);
            }


            var stringList = new List<string>();
            foreach (var list in courses.GetDistinctAcyclicCourseLists())
            {
                var csv = string.Join(", ", list.Select(t => t.Name));
                stringList.Add(csv);
                Console.WriteLine(csv);
            }
            Assert.AreEqual(stringList[0],
                "Rubber Band Catapults 101, Introduction to Paper Airplanes, Advanced Throwing Techniques, Paper Jet Engines");
            Assert.AreEqual(stringList[1], "History of Cubicle Siege Engines, Advanced Office Warfare");

            // From Courses, get a list of vertex names.
            // create a function on Courses that returns the dependencies for a given course
            // 
        }

        [TestCase]
        public void Courses_ConcatenateCourseLists_ReturnsExpectedString()
        {
            var courseStringsWithCycles = new[]
            {
                "Introduction to Paper Airplanes:", "Advanced Throwing Techniques: Introduction to Paper Airplanes",
                "History of Cubicle Siege Engines: Rubber Band Catapults 101",
                "Advanced Office Warfare: History of Cubicle Siege Engines ", "Rubber Band Catapults 101: ",
                "Paper Jet Engines: Introduction to Paper Airplanes"
            };
            var courses = Courses.Instance;
            courses.ClearData();
            foreach (var str in courseStringsWithCycles)
            {
                var course = new Course(str);
                courses.AddCourse(course);
            }
            var finalCsv = courses.ConcatenateCourseLists();
            Assert.AreEqual(finalCsv,
                "Introduction to Paper Airplanes, Advanced Throwing Techniques, Paper Jet Engines, Rubber Band Catapults 101, History of Cubicle Siege Engines, Advanced Office Warfare");

        }
    }
}
