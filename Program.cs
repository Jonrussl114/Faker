using System;
using System.Threading;
using System.Collections.Generic;

class Program {
    static Dictionary<string, List<string>> userPosts = new();
    static List<Thread> userThreads = new();
    static Dictionary<string, bool> activeUsers = new();
    static string? currentUser = null;
    static object userPostLock = new();  // Lock for userPosts
    static object activeUserLock = new();
    static bool running = true;

    /**
     * Displays a welcome message and continuously checks if any user accounts exist.
     * If no accounts are found, prompts the user to create one.
     * The loop runs as long as the application is active.
     */
    static void Main(string[] args) {
        Console.WriteLine("[Welcome to Faker]\nType 'exit' to quit.\n");
        
        while (running)
        {   
            if (!userThreads.Any()) {
                Console.WriteLine("You have not created an account.");
                CreateAccount();
            }
            
        }
        
    }

    /**
     * Handles the creation of a new user account. Displays existing user posts before prompting the user for a new username.
     * Ensures the username is unique and not empty. If the user enters "exit", prompts for confirmation before terminating the application.
     * Properly shuts down all active user threads before exiting.
     */

    static void CreateAccount() {
        foreach (var users in userPosts)
        {
            List<string> reversePosts = new List<string>();
            reversePosts = userPosts[users.Key];
            reversePosts.Reverse();
            foreach (var posts in reversePosts)
            {
                Console.WriteLine($"[{users.Key}] posted: {posts}");
            }
        }

        Console.Write("\nEnter new username: @");
        string username = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrEmpty(username) || userPosts.ContainsKey(username))
        {
            Console.WriteLine("Invalid username or username already exists.");
            SwitchAccount();
            return;
        } else if (username == "exit")
        {   
            Console.Write("Are you sure you want to quit Faker? (yes/no): ");
            string confirmation = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (confirmation == "yes") {
                Console.WriteLine("Stopping Faker... Goodbye!");
                running = false;
                lock (activeUserLock)
                {
                    foreach (var user in activeUsers.Keys)
                    {
                        activeUsers[user] = false;
                    }
                }
                Thread.Sleep(500);

                foreach (var thread in userThreads)
                {
                    if (thread.IsAlive) 
                    {
                        thread.Join(1000);  // Wait for thread to complete
                    }
                }
                Console.WriteLine("All users have logged out. Goodbye!");
                Environment.Exit(0);
            } else
            {
                Console.WriteLine("Quit canceled. Returning to menu...");
            }
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
        
        userPosts[username] = new List<string>();
        activeUsers[username] = true;
        Thread userThread = new(() => UserThreadTask(username));
        userThreads.Add(userThread);
        userThread.Start();

        currentUser = username;
    }

    /**
     * Represents the main execution loop for a user's thread. Handles user interactions, such as creating posts,
     * viewing posts, switching accounts, and logging out. Ensures proper synchronization using locks
     * to prevent concurrent modification issues. Also provides an option to quit the application,
     * gracefully shutting down all active threads before exiting.
     */
    static void UserThreadTask(string userName)
    {
        Console.WriteLine($"\n[{userName} is currently active] Type 5 to finish.\n");
        
        while (running && activeUsers[userName] && activeUsers.ContainsKey(userName)) {
            if (!running || !activeUsers.ContainsKey(userName) || !activeUsers[userName]) break;
            if (currentUser != userName)
            {
                Thread.Sleep(100); // Prevents unnecessary CPU usage when switched
                continue;
            }

            Console.Write("Options:\n1. Create a post\n2. See all your post\n3. See all post\n4. Switch account\n5. Exit\n6. Quit Faker\nSelection: ");
            string options = Console.ReadLine()?.Trim().ToLower() ?? "";
            Console.WriteLine();
            if (options == "1") {
                Console.Write($"[{userName}]: Create a post: ");
                string post = Console.ReadLine()?.Trim().ToLower() ?? "";

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
                        Console.WriteLine();
                        foreach (var posts in reversePosts)
                        {
                            Console.WriteLine($"[You] posted: {posts}");
                        }
                        Console.WriteLine();
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
            } else if (options == "5")
            {
                activeUsers[userName] = false;
                Console.WriteLine($"[{userName}] has been logged off.");
                CreateAccount();
                break;
            } else if (options == "6")
            {
                Console.Write("Are you sure you want to quit Faker? (yes/no): ");
                string confirmation = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (confirmation == "yes") {
                    Console.WriteLine("Stopping Faker... Goodbye!");
                    running = false;
                    lock (activeUserLock)
                    {
                        foreach (var user in activeUsers.Keys)
                        {
                            activeUsers[user] = false;
                        }
                    }
                    Thread.Sleep(500);

                    foreach (var thread in userThreads)
                    {
                        if (thread.IsAlive) 
                        {
                            thread.Join(1000);  // Wait for thread to complete
                        }
                    }
                    Console.WriteLine("All users have logged out. Goodbye!");
                    Environment.Exit(0);
                } else
                {
                    Console.WriteLine("Quit canceled. Returning to menu...");
                }
            }
            Console.WriteLine();
        }
    }

    /**
     * Switches the current active user to another existing account. Ensures thread safety
     * by using locks to manage shared resources, such as active user states. If the provided 
     * username does not exist, an error message is displayed. Once switched, the new user 
     * session begins execution.
     */
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