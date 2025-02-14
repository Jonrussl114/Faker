using System;
using System.Threading;
using System.Collections.Generic;
using ConsoleApp1;

class Program {
    static Dictionary<string, List<string>> userPosts = new();
    static List<Thread> userThreads = new();
    static Dictionary<string, bool> activeUsers = new();
    static void Main(string[] args) {
        Console.WriteLine("[Welcome to Faker]\n");
        while (true)
        {
            if (!userThreads.Any()) {
                Console.WriteLine("You have not created an account.");
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
            } else {
                Console.WriteLine($"{userName} is already active.");
            } 
            /**
             else
                {
                    CreateAccount(userName);
                    break;
                }
            */
           //HHello
            }
        }
        
        Console.WriteLine("next");
    }
    static void CreateAccount(string userName) {
        userPosts[userName] = new List<String>();
        activeUsers[userName] = true;

        Thread userThread = new(() => UserThreadTask(userName));
        userThreads.Add(userThread);
        userThread.Start();
    }

    static void UserThreadTask(string userName)
    {
        Console.WriteLine($"\n[{userName} is currently active] Type 'exit' to finish.");
        
        while (activeUsers[userName]) {
            Console.Write("Options:\n1. Create a post\n2. See all your post\n3. See all post\nSelection: ");
            string options = Console.ReadLine();
            Console.WriteLine();
            if (options == "1") {
                Console.Write($"\n{userName}: Create a post: ");
                string post = Console.ReadLine();

                if (post == null) {
                    Console.WriteLine("Invalid Input.");
                    continue;
                }
                if (post.ToLower() == "exit")
                {
                    activeUsers[userName] = false;
                    Console.WriteLine($"[{userName}] has been logged off.");
                    break;
                }
                lock (userPosts)
                {
                    userPosts[userName].Add(post);
                    Console.WriteLine($"[{userName}] Has posted: {post}");
                }
            } else if (options == "2") {
                if (!userPosts[userName].Any())
                {
                    Console.WriteLine("Create a post first!");
                } else {
                    List<string> reversePosts = new List<string>();
                    reversePosts = userPosts[userName];
                    reversePosts.Reverse();
                    foreach (var posts in userPosts[userName])
                    {
                        Console.WriteLine($"[You] posted: {posts}");
                    }
                }
                
            }
            Console.WriteLine();
        }
    }

    static void SwitchAccount(string userName) {
        //for ()
        //return false;
    }
    static Boolean OneAccount() {
        return false;
    }


}