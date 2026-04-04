// See https://aka.ms/new-console-template for more information
// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design.Serialization;

using System;
using System.IO;
using System.Transactions;
using System.Data;
using System.Security;
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.ExceptionServices;

public class BPEImplementator : IEqualityComparer<List<char>>
{
    public bool Equals(List<char> x, List<char> y)
    {
        if (x.Count != y.Count) return false;
        for(int i = 0; i < x.Count; i++)
        {
            if (x[i] != y[i]) return false;
        }
        return true;
    }

    public int GetHashCode(List<char> obj)
    {
        int hash = 17;
        foreach(char c in obj)
        {
            hash = hash * 31 + c.GetHashCode();
        }
        return hash;
    }

    internal class Token
    {
        public int index;
        public int size;
    }
    internal class Pair
    {
        public LinkedListNode<Token> first;
        public LinkedListNode<Token> second;

        public Pair(LinkedListNode<Token> first, LinkedListNode<Token> second)
        {
            this.first = first;
            this.second = second;
        }
    }
    internal class TrackingNode
    {
        public List<Pair> pairs;
        public int amount;

        public TrackingNode(Pair pair)
        {
            pairs = new List<Pair>();
            pairs.Add(pair);
            amount = 1;
        }
    }

    LinkedList<Token> tokens;
    Dictionary<List<char>, TrackingNode> tracker;
    PriorityQueue<TrackingNode, int> trackerQueue;

    public BPEImplementator(int length)
    {
        tokens = new LinkedList<Token>();
        trackerQueue = new PriorityQueue<TrackingNode, int>();
        tracker = new Dictionary<List<char>, TrackingNode>();

        for(int i = 0; i < length; i++)
        {
            tokens.AddLast(new Token() { index = i, size = 1 });
        }
    }

    public bool Tokenization(string text)
    {
        LinkedListNode<Token> node = tokens.First;
        while(node != tokens.Last)
        {
            List<char> possiblePair = (text[node.Value.index..(node.Next.Value.index + node.Next.Value.size)]).ToList<char>();
            bool repeated = false;
            foreach(List<char> trackerKey in tracker.Keys)
            {
                if(!Equals(trackerKey, possiblePair)) continue;
                
                Pair repeatedPair = new Pair(node, node.Next);
                tracker[trackerKey].amount++;
                tracker[trackerKey].pairs.Add(repeatedPair);
                node = node.Next;
                repeated = true;
                break;
            }

            if(repeated) continue;

            Pair pair = new Pair(node, node.Next);
            tracker[possiblePair] = new TrackingNode(pair);
            node = node.Next;
        }

        foreach(TrackingNode trackingNode in tracker.Values)
        {
            trackerQueue.Enqueue(trackingNode, -trackingNode.amount);
        }

        if(trackerQueue.Peek().amount > 1) return true;
        return false;
    }

    public void Sort()
    {
        List<Pair> pairs = trackerQueue.Dequeue().pairs;
        foreach(Pair pair in pairs)
        {
            pair.first.Value.size += pair.second.Value.size;
            tokens.Remove(pair.second);
        }

        trackerQueue.Clear();
        tracker.Clear();
    }

    public void PrintTokens(string text)
    {
        LinkedListNode<Token> node = tokens.First;
        while(node != null)
        {
            Console.WriteLine(text[node.Value.index..(node.Value.index + node.Value.size)]);
            node = node.Next;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        string fileContent = File.ReadAllText("../../../file.txt");

        string text = "bigger higher";
        BPEImplementator implementator = new BPEImplementator(fileContent.Length);
        while(implementator.Tokenization(fileContent))
        {
            implementator.Sort();
        }

        implementator.PrintTokens(fileContent);
    }
}