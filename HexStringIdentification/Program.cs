// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design.Serialization;

using System;
using System.Transactions;
using System.Data;

class Program
{
    enum CheckVal
    {
        Beginning,
        Contains1,
        Contains10,
        Contains101
    };

    enum HexState
    {
        ContainsNone,
        Contains1,
        Contains10,
        Contains101
    };

    enum ThirdFromLast
    {
        x000,
        x001,
        x010,
        x011,
        x100,
        x101,
        x110,
        x111
    };

    enum NFAState
    {
        None,
        First,
        Second,
        Final
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

    struct Hex2DTable
    {
        public HexState state;
        public char current;
    }

    static bool IsHex2DTable(string input, char[] hexChars, Hex2DTable hex2D)
    {
        Dictionary<Hex2DTable, HexState> transitions = new Dictionary<Hex2DTable, HexState>
        {
            { new Hex2DTable { state = HexState.ContainsNone, current = '0' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = '0' }, HexState.Contains10 },
            { new Hex2DTable { state = HexState.Contains10, current = '0' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains101, current = '0' }, HexState.Contains101 },

            { new Hex2DTable { state = HexState.ContainsNone, current = '1' }, HexState.Contains1 },
            { new Hex2DTable { state = HexState.Contains1, current = '1' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains10, current = '1' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains101, current = '1' }, HexState.Contains101 },

            // current = '2'
            { new Hex2DTable { state = HexState.Contains101, current = '2' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = '2' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = '2' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = '2' }, HexState.ContainsNone },

            // current = '3'
            { new Hex2DTable { state = HexState.Contains101, current = '3' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = '3' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = '3' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = '3' }, HexState.ContainsNone },

            // current = '4'
            { new Hex2DTable { state = HexState.Contains101, current = '4' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = '4' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = '4' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = '4' }, HexState.ContainsNone },

            // current = '5'
            { new Hex2DTable { state = HexState.Contains101, current = '5' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = '5' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = '5' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = '5' }, HexState.ContainsNone },

            // current = '6'
            { new Hex2DTable { state = HexState.Contains101, current = '6' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = '6' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = '6' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = '6' }, HexState.ContainsNone },

            // current = '7'
            { new Hex2DTable { state = HexState.Contains101, current = '7' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = '7' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = '7' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = '7' }, HexState.ContainsNone },

            // current = '8'
            { new Hex2DTable { state = HexState.Contains101, current = '8' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = '8' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = '8' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = '8' }, HexState.ContainsNone },

            // current = '9'
            { new Hex2DTable { state = HexState.Contains101, current = '9' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = '9' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = '9' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = '9' }, HexState.ContainsNone },

            // current = 'A'
            { new Hex2DTable { state = HexState.Contains101, current = 'A' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = 'A' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = 'A' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = 'A' }, HexState.ContainsNone },

            // current = 'B'
            { new Hex2DTable { state = HexState.Contains101, current = 'B' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = 'B' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = 'B' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = 'B' }, HexState.ContainsNone },

            // current = 'C'
            { new Hex2DTable { state = HexState.Contains101, current = 'C' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = 'C' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = 'C' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = 'C' }, HexState.ContainsNone },

            // current = 'D'
            { new Hex2DTable { state = HexState.Contains101, current = 'D' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = 'D' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = 'D' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = 'D' }, HexState.ContainsNone },

            // current = 'E'
            { new Hex2DTable { state = HexState.Contains101, current = 'E' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = 'E' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = 'E' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = 'E' }, HexState.ContainsNone },

            // current = 'F'
            { new Hex2DTable { state = HexState.Contains101, current = 'F' }, HexState.Contains101 },
            { new Hex2DTable { state = HexState.Contains10, current = 'F' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.Contains1, current = 'F' }, HexState.ContainsNone },
            { new Hex2DTable { state = HexState.ContainsNone, current = 'F' }, HexState.ContainsNone },
        };
        
        foreach(char c in input)
        {
            if(!hexChars.Contains(c)) return false;

            hex2D.current = c;
            hex2D.state = transitions[hex2D];
        }
        if(hex2D.state == HexState.Contains101) return true;

        return false;
    }

    struct ThirdFromLastTable
    {
        public ThirdFromLast state;
        public char current;
    }

    static bool IsThirdFromEnd(string input, ThirdFromLastTable value)
    {
        Dictionary<ThirdFromLastTable, ThirdFromLast> transitions = new Dictionary<ThirdFromLastTable, ThirdFromLast>
        {
            { new ThirdFromLastTable { state = ThirdFromLast.x000, current = '0' }, ThirdFromLast.x000 },
            { new ThirdFromLastTable { state = ThirdFromLast.x001, current = '0' }, ThirdFromLast.x010 },
            { new ThirdFromLastTable { state = ThirdFromLast.x010, current = '0' }, ThirdFromLast.x100 },
            { new ThirdFromLastTable { state = ThirdFromLast.x011, current = '0' }, ThirdFromLast.x110 },
            { new ThirdFromLastTable { state = ThirdFromLast.x100, current = '0' }, ThirdFromLast.x000 },
            { new ThirdFromLastTable { state = ThirdFromLast.x101, current = '0' }, ThirdFromLast.x010 },
            { new ThirdFromLastTable { state = ThirdFromLast.x110, current = '0' }, ThirdFromLast.x100 },
            { new ThirdFromLastTable { state = ThirdFromLast.x111, current = '0' }, ThirdFromLast.x110 },

            { new ThirdFromLastTable { state = ThirdFromLast.x000, current = '1' }, ThirdFromLast.x001 },
            { new ThirdFromLastTable { state = ThirdFromLast.x001, current = '1' }, ThirdFromLast.x011 },
            { new ThirdFromLastTable { state = ThirdFromLast.x010, current = '1' }, ThirdFromLast.x101 },
            { new ThirdFromLastTable { state = ThirdFromLast.x011, current = '1' }, ThirdFromLast.x111 },
            { new ThirdFromLastTable { state = ThirdFromLast.x100, current = '1' }, ThirdFromLast.x101 },
            { new ThirdFromLastTable { state = ThirdFromLast.x101, current = '1' }, ThirdFromLast.x011 },
            { new ThirdFromLastTable { state = ThirdFromLast.x110, current = '1' }, ThirdFromLast.x101 },
            { new ThirdFromLastTable { state = ThirdFromLast.x111, current = '1' }, ThirdFromLast.x111 },
        };

        value.state = ThirdFromLast.x000;

        foreach(char c in input)
        {
            if(c != '0' && c != '1') return false;

            value.current = c;
            value.state = transitions[value];
        }

        if(value.state == ThirdFromLast.x100 || value.state == ThirdFromLast.x101 || value.state == ThirdFromLast.x110 || value.state == ThirdFromLast.x111) return true;

        return false;
    }

    struct NFATable
    {
        public NFAState state;
        public char current;
    }

    static bool NFAThirdFromEnd(string input, NFATable nfaVar, int index)
    {
        if(nfaVar.state == NFAState.Final && index == input.Length) return true;
        else if(nfaVar.state == NFAState.Final && index < input.Length - 1) return false;
        else if(index >= input.Length || (input[index] != '0' && input[index] != '1')) return false;

        nfaVar.current = input[index];
        if(nfaVar.state == NFAState.None && input[index] == '1')
        {
            if(NFAThirdFromEnd(input, new NFATable { state = NFAState.First, current = input[index] }, index + 1)) return true;
            if(NFAThirdFromEnd(input, new NFATable { state = NFAState.None, current = input[index] }, index + 1)) return true;

            return false;
        }
        nfaVar.state = (nfaVar.state == NFAState.Second) ? NFAState.Final : nfaVar.state;
        nfaVar.state = (nfaVar.state == NFAState.First) ? NFAState.Second : nfaVar.state;
        if(NFAThirdFromEnd(input, nfaVar, index + 1)) return true;

        return false;
    }
    
    static void Main(string[] args)
    {
        char[] hexChars = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',
                         'a', 'b', 'c', 'd', 'e', 'f'};
        string input = "12345";
        string binaryInput = "101010101";
        //Console.WriteLine(IsHexString(input, hexChars));

        CheckVal checkVal = CheckVal.Beginning;
        //Console.WriteLine(IsBinaryUpdated(binaryInput, new char[] {'0', '1'}, checkVal));

        string hexInput = "10A2B3C";
        Hex2DTable hex2D = new Hex2DTable { state = HexState.ContainsNone, current = '\0' };
        // Console.WriteLine(IsHex2DTable(hexInput, hexChars, hex2D));

        string binaryThirdInput = "1010100";
        // Console.WriteLine(IsThirdFromEnd(binaryThirdInput, new ThirdFromLastTable { state = ThirdFromLast.x000, current = '\0' }));

        bool result = NFAThirdFromEnd(binaryThirdInput, new NFATable { state = NFAState.None, current = '\0' }, 0);
        Console.WriteLine(result);
    }
}