using System.Linq;// C# 10 comes with ImplicitUsings, meaning the most popular using namespaces such as this one are already built-in so there is no need to declare it

// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");


// Lists, students, and books will be used as our collections for this example
List<StudentInfo> students = new()
{
    new StudentInfo {StudentID = 1, Name = "Jonathan", Grade1 = 95, Grade2 = 90},
    new StudentInfo {StudentID = 2, Name = "Maria", Grade1 = 92, Grade2 = 85},
    new StudentInfo {StudentID = 3, Name = "Marcos", Grade1 = 81, Grade2 = 91}
};

List<BookInfo> books = new()
{
    new BookInfo {StudentID = 1, BookName = "C# Fundamentals"},
    new BookInfo {StudentID = 1, BookName = "Microsoft Magazine"},
    new BookInfo {StudentID = 2, BookName = "C# Fundamentals"}
};

// Minimal query expression
IEnumerable<StudentInfo> students1 = from o in students select o;

// Where specifies which items to use, in this example all Grade1 greater than 90
IEnumerable<StudentInfo> students2 =    from o in students
                                        where o.Grade1 > 90
                                        select o;

// Display grades greater than 90
Console.WriteLine("Displaying grade greater than 90...");
foreach (var student in students2)
{
    Console.WriteLine($"Name: {student.Name} Grade: {student.Grade1}");
}
// Output: 
// Name: Jonathan Grade: 95 
// Name: Maria Grade: 92

// Let creates derived range variables
IEnumerable<string> studentAverages =   from o in students
                                        let averageGrade = (o.Grade1 + o.Grade2) / 2
                                        select $"{o.Name} grade average: {averageGrade}";

// Join combines two collections together based on a condition
var inUseBooks =    from o in students
                    join b in books on o.StudentID equals b.StudentID // Inner join
                    select new { o.Name, b.BookName }; // New collection using these types

// Display books that are checked out
Console.WriteLine("\nDisplaying books that are checked out...");
foreach (var inUseBook in inUseBooks)
{
    Console.WriteLine($"{inUseBook.Name} checked out book: {inUseBook.BookName}");
}
// Output: 
// Jonathan checked out book: C# Fundamentals
// Jonathan checked out book: Microsoft Magazine 
// Maria checked out book: C# Fundamentals

// OrderBy sorts all items in a collection
IEnumerable<StudentInfo> students3 =    from o in students
                                        orderby o.Name // Ascending by default
                                        select o;

// Groups bundle elements into groups based on the information specified
var bookGroups =    from o in books
                    group o by o.BookName;

// Display information about the groups
Console.WriteLine("\nDisplaying information about the groups...");
foreach (var bookGroup in bookGroups)
{
    Console.WriteLine($"Group Key: {bookGroup.Key}");
    foreach (var o in bookGroup)
    {
        Console.WriteLine($"Book: {o.BookName} is being borrowed by studentID: {o.StudentID}");
    }
    Console.WriteLine();
}
// Output: 
// Group Key: C# Fundamentals
// Book: C# Fundamentals is being borrowed by studentID: 1
// Book: C# Fundamentals is being borrowed by studentID: 2 
//
// Group Key: Microsoft Magazine
// Book: Microsoft Magazine is being borrowed by studentID: 1

class StudentInfo
{
    public int StudentID { get; set; }
    public string Name { get; set; }
    public int Grade1 { get; set; }
    public int Grade2 { get; set; }
}

class BookInfo
{
    public int StudentID { get; set; } = 0;
    public string BookName { get; set; }
}