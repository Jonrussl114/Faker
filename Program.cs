﻿using System;
using System.Threading;
using System.Collections.Generic;

class Program {
    static Dictionary<string, List<string>> userPosts = new();
    static List<Thread> userThreads = new();
    static Dictionary<string, bool> activeUsers = new();
    static string? currentUser = null;
    static object userPostLock = new();  // Lock for userPosts
    static object activeUserLock = new();

    static void Main(string[] args) {
        Console.WriteLine("[Welcome to Faker]\n");
        
        while (true)
        {   
            if (!userThreads.Any()) {
                Console.WriteLine("You have not created an account.");
                CreateAccount();
            } /** else if (userThreads.Any())
            {
                Console.Write("\nCreate new account (new)\nSwitch account\n (new/switch/stop all): ");
                string command = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (command == "stop all")
                {
                    Console.WriteLine("\nStopping all users...");
                    foreach (var key in activeUsers.Keys)
                        activeUsers[key] = false;
                    break;
                }
                else if (command == "new")
                {
                    Console.Write("Create an account.\nUsername: @");
                    string userName = Console.ReadLine();
                    if (userName == null || userName == "")
                        continue;

                    if (userName.ToLower() == "stop all")
                    {
                        Console.WriteLine("\nStopping all users...");
                        foreach (var key in activeUsers.Keys)
                            activeUsers[key] = false;

                        break;
                    } else if (!userPosts.ContainsKey(userName)) {
                        userPosts[userName] = new List<string>(); // Initialize user’s post list
                        activeUsers[userName] = true;

                        Thread userThread = new(() => UserThreadTask(userName));
                        userThreads.Add(userThread);
                        userThread.Start();
                    }
                }
                else if (command == "switch")
                {
                    //SwitchAccount();
                }
            }
            */
        }
        
    }
    static void CreateAccount() {
        Console.Write("\nEnter new username: @");
        string username = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrEmpty(username) || userPosts.ContainsKey(username))
        {
            Console.WriteLine("Invalid username or username already exists.");
            SwitchAccount();
            return;
        }

        Console.WriteLine($"Account '{username}' created! Switching to {username}...");
        lock (userPostLock) // Lock 1 (Locks userPosts)
        {
            userPosts[username] = new List<string>();
            lock (activeUserLock) // Lock 2 (Locks activeUsers)
            {
                activeUsers[username] = true;
            }
        }

        userPosts[username] = new List<string>(); // Initialize posts
        activeUsers[username] = true;
        Thread userThread = new(() => UserThreadTask(username));
        userThreads.Add(userThread);
        userThread.Start();

        
        currentUser = username;

    }

    static void UserThreadTask(string userName)
    {
        Console.WriteLine($"\n[{userName} is currently active] Type 5 to finish.\n");
        
        while (activeUsers[userName]) {
            if (currentUser != userName)
            {
                Thread.Sleep(100); // Prevents unnecessary CPU usage when switched
                continue;
            }

            Console.Write("Options:\n1. Create a post\n2. See all your post\n3. See all post\n4. Switch account\n5. Exit\nSelection: ");
            string options = Console.ReadLine();
            Console.WriteLine();
            if (options == "1") {
                Console.Write($"{userName}: Create a post: ");
                string post = Console.ReadLine();

                if (post == null) {
                    Console.WriteLine("Invalid Input.");
                    continue;
                }
                lock (activeUserLock) // Lock 1 (Locks activeUsers)
                {
                    lock (userPostLock) // Lock 2 (Locks userPosts)
                    {
                        userPosts[userName].Add(post);
                        Console.WriteLine($"\n[{userName}] Has posted: {post}");
                    }
                }
            } else if (options == "2") {
                if (!userPosts[userName].Any())
                {
                    Console.WriteLine("Create a post first!");
                } else {
                    List<string> reversePosts = userPosts[userName];
                    reversePosts.Reverse();
                    foreach (var posts in reversePosts)
                    {
                        Console.WriteLine($"[You] posted: {posts}");
                    }
                }
                
            } else if (options == "3")
            {
                foreach (var users in userPosts)
                {
                    if (users.Key.Equals(userName)) {
                        List<string> reversePosts = new List<string>();
                        reversePosts = userPosts[userName];
                        reversePosts.Reverse();
                        foreach (var posts in reversePosts)
                        {
                            Console.WriteLine($"[You] posted: {posts}");
                        }
                    } else {
                        List<string> reversePosts = new List<string>();
                        reversePosts = userPosts[users.Key];
                        reversePosts.Reverse();
                        foreach (var posts in reversePosts)
                        {
                            Console.WriteLine($"[{users.Key}] posted: {posts}");
                        }
                    }
                }
            }
            else if (options == "4")
            {
                SwitchAccount();
            }
            if (options == "5")
            {
                activeUsers[userName] = false;
                Console.WriteLine($"[{userName}] has been logged off.");
                CreateAccount();
                break;
            }
            Console.WriteLine();
        }
    }

    static void SwitchAccount() {
        Console.Write("\nEnter username to switch to: ");
        string username = Console.ReadLine()?.Trim() ?? "";
        if (currentUser != username)
            {
                Thread.Sleep(100); // Prevents unnecessary CPU usage when switched
            }

        if (!userPosts.ContainsKey(username))
        {
            Console.WriteLine("User not found.");
            return;
        }
        lock (activeUserLock) // Lock 1 (Locks activeUsers)
        {
            lock (userPostLock) // Lock 2 (Locks userPosts)
            {
                activeUsers[currentUser] = false;
                activeUsers[username] = true;
            }
        }
        currentUser = username;
        Console.WriteLine($"Switched to {username}. You can start posting.");
        UserThreadTask(username);
        
    }

}