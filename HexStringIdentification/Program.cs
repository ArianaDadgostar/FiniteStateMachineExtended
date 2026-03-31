// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design.Serialization;

using System;

class Program
{
    enum CheckVal
    {
        Beginning,
        Contains1,
        Contains10,
        Contains101
    };

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

    static bool IsBinaryAndContainsVal(string input, char[] binaryChars)
    {
        bool contains1 = false;
        bool contains0 = false;
        foreach(char c in input)
        {
            if(!binaryChars.Contains(c)) return false;
            
            if(contains1 && contains0 && c == '1') return true;

            contains0 = (contains1 && !contains0 && c == '0') ? true : false;
            contains1 = (!contains1 && (c == '1' || !contains0)) ? true : contains1;
        }
        return false;
    }

    static bool IsBinaryUpdated(string input, char[] binaryChars, CheckVal checkVal)
    {
        foreach(char c in input)
        {
            if(!binaryChars.Contains(c)) return false;

            switch(checkVal)
            {
                case CheckVal.Beginning:
                    checkVal = (c == '1') ? CheckVal.Contains1 : CheckVal.Beginning;
                    break;

                case CheckVal.Contains1:
                    checkVal = (c == '0') ? CheckVal.Contains10 : CheckVal.Contains1;
                    break;
                case CheckVal.Contains10:
                    checkVal = (c == '1') ? CheckVal.Contains101 : CheckVal.Beginning;
                    break;
                case CheckVal.Contains101:
                    break;
            }
        }
        if(checkVal == CheckVal.Contains101) return true;
        return false;
    }
    
    static void Main(string[] args)
    {
        char[] hexChars = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',
                         'a', 'b', 'c', 'd', 'e', 'f'};
        //string input = "12345";
        string binaryInput = "101010101";
        //Console.WriteLine(IsHexString(input, hexChars));
        CheckVal checkVal = CheckVal.Beginning;
        Console.WriteLine(IsBinaryUpdated(binaryInput, new char[] {'0', '1'}, checkVal));
    }
}