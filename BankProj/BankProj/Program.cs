// See https://aka.ms/new-console-template for more information
// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design.Serialization;

using System;
using System.Transactions;
using System.Data;
using System.Diagnostics;

enum Actions
{
    LoggedIn,
    NotLoggedIn,
    Login,
    Withdraw,
    Deposit,
    View
};

class BankAccount
{
    public Actions state;
    public Actions wanted;
    int balance;
    
    public int getBalance()
    {
        return balance;
    }

    public void deposit(int deposit)
    {
        balance += deposit;
    }
    
    public void withdraw(int withdrawal)
    {
        balance -= withdrawal;
    }
}

class Program
{
    static Dictionary<string, string> credentials = new Dictionary<string, string>()
    {
        {"timmy", "1234"},
        {"ella", ":D"},
        {"bart", "bart"}
    };

    static void Login(ref BankAccount account)
    {
        Console.WriteLine("username:");
        string username = Console.ReadLine();
        Console.WriteLine("password:");
        string password = Console.ReadLine();

        account.state = (credentials.ContainsKey(username) && credentials[username] == password) ? Actions.LoggedIn : Actions.NotLoggedIn;
    }

    static void Deposit(ref BankAccount account)
    {
        Console.WriteLine("How much?");
        int deposit = int.Parse(Console.ReadLine());

        account.deposit(deposit);
    }

    static void Withdraw(ref BankAccount account)
    {
        Console.WriteLine("How much?");
        int withdrawal = int.Parse(Console.ReadLine());

        account.withdraw(withdrawal);
    }

    static void View(ref BankAccount account)
    {
        Console.WriteLine($"Your balance is: {account.getBalance()}");
    }
    
    static void BankAction(ref BankAccount account)
    {
        Dictionary<(Actions, Actions), Action<BankAccount>> transitions = new Dictionary<(Actions, Actions), Action< BankAccount>>
        {
            {(Actions.LoggedIn, Actions.Withdraw), (acc) => Withdraw(ref acc)},
            {(Actions.LoggedIn, Actions.Deposit), (acc) => Deposit(ref acc)},
            {(Actions.LoggedIn, Actions.View), (acc) => View(ref acc)},
            {(Actions.LoggedIn, Actions.Login), (acc) => Login(ref acc)},

            {(Actions.NotLoggedIn, Actions.Withdraw), (acc) => Login(ref acc)},
            {(Actions.NotLoggedIn, Actions.Deposit), (acc) => Deposit(ref acc)},
            {(Actions.NotLoggedIn, Actions.View), (acc) => Login(ref acc)},
            {(Actions.NotLoggedIn, Actions.Login), (acc) => Login(ref acc)}
        };
        Dictionary<string, Actions> wanted = new Dictionary<string, Actions>
        {
            {"Login", Actions.Login},
            {"View", Actions.View},
            {"Deposit", Actions.Deposit},
            {"Withdraw", Actions.Withdraw}
        };

        Login(ref account);

        Actions previous = account.state;

        while(true)
        {
            Console.WriteLine("What next? (Login, Withdraw, Deposit, View)");

            account.wanted = wanted[Console.ReadLine()];

            transitions[(account.state, account.wanted)](account);

            account.state = previous;
        }
    }

    static void Main(string[] args)
    {
        BankAccount account = new BankAccount();

        account.state = Actions.Login;

        BankAction(ref account);
    }
}