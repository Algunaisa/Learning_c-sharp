using System;

public class DemoClass
{
    public void Method1()
    {
        Console.WriteLine("Method 1");
    }
}

class DemoMain
{
    static void Main()
    {
        DemoClass myClass = new DemoClass();
        myClass.Method1();
    }
}