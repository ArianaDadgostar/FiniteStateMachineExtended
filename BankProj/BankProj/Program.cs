// See https://aka.ms/new-console-template for more information
// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design.Serialization;

using System;
using System.Transactions;
using System.Data;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

public enum Actions
{
    LoggedIn,
    NotLoggedIn,
    Login,
    Withdraw,
    Deposit,
    View
};

class Bank
{
    static public Dictionary<string, BankAccount> accounts = new Dictionary<string, BankAccount>()
    {
        {"timmy", new BankAccount("1234")},
        {"ella", new BankAccount(":D")},
        {"bart", new BankAccount("password")}
    };

    public bool Login(string username, string password)
    {
        if(!accounts.ContainsKey(username) || accounts[username].password != password) return true;

        accounts[username].state = Actions.LoggedIn;
        return true;
    }

    public bool Deposit(string username, int deposit)
    {
        if(!accounts.ContainsKey(username)) return false;

        BankAccount account = accounts[username];
        account.wanted = Actions.Deposit;

        return account.transitions[(account.state, account.wanted)](string.Empty, deposit);
    }

    public bool View(string username)
    {
        if(!accounts.ContainsKey(username)) return false;

        BankAccount account = accounts[username];
        account.wanted = Actions.View;

        return account.transitions[(account.state, account.wanted)](username, 0);
    }
    
    public bool Withdraw(string username, int amount)
    {
        if(!accounts.ContainsKey(username)) return false;

        BankAccount account = accounts[username];
        account.wanted = Actions.Withdraw;

        return account.transitions[(account.state, account.wanted)](username, amount);
    }
}

public class BankAccount
{
    public Actions state;
    public Actions wanted;

    public string password;

    int balance;

    public Dictionary<(Actions, Actions), Func<string, int, bool>> transitions;

    public BankAccount(string password)
    {
        state = Actions.NotLoggedIn;
        wanted = Actions.Login;

        this.password = password;
        
        transitions = new Dictionary<(Actions, Actions), Func<string, int, bool>>()
        {
            {(Actions.LoggedIn, Actions.Withdraw), this.Withdraw},
            {(Actions.LoggedIn, Actions.Deposit), this.Deposit},
            {(Actions.LoggedIn, Actions.View), this.View},

            {(Actions.NotLoggedIn, Actions.Withdraw), new Func<string, int, bool> ((s, i) => false)},
            {(Actions.NotLoggedIn, Actions.Deposit), this.Deposit},
            {(Actions.NotLoggedIn, Actions.View), new Func<string, int, bool> ((s, i) => false)},
        };
    }

    public bool Withdraw(string blank, int amount)
    {
        balance -= amount;
        return true;
    }

    public bool Deposit(string blank, int amount)
    {
        balance += amount;
        return true;
    }

    public bool View(string blank, int blank2)
    {
        Console.WriteLine($"Your balance is: {balance}");
        return true;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Bank bank = new();

        bank.Login("timmy", "1234");
        bank.Deposit("timmy", 100);
        bank.View("timmy");
        bank.Login("ella", ":");
        Console.WriteLine(bank.Withdraw("ella", 50));
    }
}