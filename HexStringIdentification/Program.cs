// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design.Serialization;

using System;

class Program
{
    const bool VER1 = false;
    static bool IsHexString(string input, char[] hexChars)
    {
        foreach(char c in input)
        {
            #if VER1
            if(!Uri.IsHexDigit(c)) return false;
            #endif

            #if !VER1
            if(!hexChars.Contains(c)) return false;
            #endif
        }
        return true;
    }
    
    static void Main(string[] args)
    {
        char[] hexChars = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',
                         'a', 'b', 'c', 'd', 'e', 'f'};
        string input = "12345";
        Console.WriteLine(IsHexString(input, hexChars));
    }
}