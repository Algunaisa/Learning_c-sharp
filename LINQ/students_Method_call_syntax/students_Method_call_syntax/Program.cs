// Lists, students, and books will be used as our collections for this example
List<StudentInfo> students = new()
{
    new StudentInfo {StudentID = 1, Name = "Jonathan", Grade1 = 95, Grade2 = 90},
    new StudentInfo {StudentID = 2, Name = "Maria", Grade1 = 92, Grade2 = 85},
    new StudentInfo {StudentID = 3, Name = "Marcos", Grade1 = 81, Grade2 = 91}
};

// Example 1: Where Clause
// Display grades greater than 90 using where clause of Method Call Syntax
IEnumerable<StudentInfo> students1 = students.Where(o => o.Grade1 > 90);

Console.WriteLine("Displaying grade greater than 90...");
foreach (var student in students1)
{
    Console.WriteLine($"Name: {student.Name} Grade: {student.Grade1}");
}
// Output: 
// Displaying grades greater than 90...
// Name: Jonathan Grade: 95 
// Name: Maria Grade: 92

// Example 2: Where and Select clause
// Selecting a column using Select clause of Method Call Syntax
IEnumerable<string> students2 = students.Where(o => o.Grade1 > 90).Select(o => o.Name);

Console.WriteLine("\nDisplaying names having grade greater than 90...");
foreach (var name in students2)
{
    Console.WriteLine($"Name: {name}");
}
// Output:
// Displaying names having grade greater than 90...
// Name: Jonathan
// Name: Maria

// Example 3: OrderBy and Select clause
// Display named in sorted order using Select and OrderBy clause of Method Call Syntax
IEnumerable<string> students3 = students.OrderBy(o => o.Name).Select(o => o.Name);

Console.WriteLine("\nDisplaying names in alphabetically sorted order...");
foreach (var name in students3)
{
    Console.WriteLine($"Name: {name}");
}
// Output:
// Displaying names in alphabetically sorted order..."
// Name: Jonathan
// Name: Marcos
// Name: Maria


class StudentInfo
{
    public int StudentID { get; set; }
    public string Name { get; set; }
    public int Grade1 { get; set; }
    public int Grade2 { get; set; }
}