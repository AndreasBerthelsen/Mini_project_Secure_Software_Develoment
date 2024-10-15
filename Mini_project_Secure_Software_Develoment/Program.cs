using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mini_project_Secure_Software_Develoment.Helpers;
using Mini_project_Secure_Software_Develoment.Model;
using Mini_project_Secure_Software_Develoment.Repositories;
using Mini_project_Secure_Software_Develoment.Repositories.Interfaces;
using Mini_project_Secure_Software_Develoment.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Set up configuration to read appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Set up dependency injection
        var serviceProvider = new ServiceCollection()
    .AddDbContext<PasswordManagerContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
    .AddScoped<IMasterPasswordRepo, MasterPasswordRepo>() // Register repository
    .AddScoped<IPasswordRepo, PasswordRepo>() // Register repository
    .AddScoped<MasterPasswordService, MasterPasswordService>() // Register service
    .AddScoped<EncryptionService, EncryptionService>() // Register encryption service
    .AddScoped<PasswordService, PasswordService>() // Register password service
    .BuildServiceProvider();


        // Resolve services
        var masterPasswordService = serviceProvider.GetRequiredService<MasterPasswordService>();
        var encryptionService = serviceProvider.GetRequiredService<EncryptionService>();
        var passwordService = serviceProvider.GetRequiredService<PasswordService>();

        // Create the password manager and run the application
        var passwordManager = new PasswordManager(masterPasswordService, encryptionService, passwordService);
        await passwordManager.RunAsync();
    }
}

public class PasswordManager
{
    private readonly MasterPasswordService _masterPasswordService;
    private readonly EncryptionService _encryptionService;
    private readonly PasswordService _passwordService;

    public PasswordManager(MasterPasswordService masterPasswordService, EncryptionService encryptionService, PasswordService passwordService)
    {
        _masterPasswordService = masterPasswordService;
        _encryptionService = encryptionService;
        _passwordService = passwordService;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Welcome to Password Manager!");

        // Check if the master password is set
        if (!await _masterPasswordService.IsMasterPasswordSetAsync())
        {
            await SetMasterPasswordAsync();
        }
        else
        {
            // Validate master password and get user input
            await ValidateMasterPasswordAsync();
        }

        // Initialize encryption
        string hashedMasterPassword = await _masterPasswordService.GetHashedMasterPasswordAsync();

        Console.Write("Please enter your master password: ");
        string masterPasswordInput = Console.ReadLine();

        if (await _masterPasswordService.ValidateMasterPasswordAsync(masterPasswordInput))
        {
            Console.WriteLine("Welcome back.");
        }
        else
        {
            Console.WriteLine("Invalid master password. Exiting application.");
            return;
        }

        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. View stored passwords");
            Console.WriteLine("2. Add a new password");
            Console.WriteLine("3. Delete a password");
            Console.WriteLine("4. Delete master password and set new");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await DisplayStoredPasswordsAsync();
                    break;
                case "2":
                    await AddNewPasswordAsync();
                    break;
                case "3":
                    await DeletePasswordAsync();
                    break;
                case "4":
                    await DeleteMasterPasswordAsync();
                    break;
                case "5":
                    Console.WriteLine("Exiting application.");
                    return; // Exit the application
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }




    private async Task SetMasterPasswordAsync()
    {
        Console.Write("Set your new master password: ");
        string newPassword = ReadPassword(); // Use the new method

        Console.Write("Confirm your new master password: ");
        string confirmPassword = ReadPassword(); // Use the new method

        if (string.IsNullOrWhiteSpace(newPassword))
        {
            Console.WriteLine("Error: Password cannot be empty.");
            return;
        }

        if (newPassword != confirmPassword)
        {
            Console.WriteLine("Error: Passwords do not match.");
            return;
        }

        try
        {
            await _masterPasswordService.SetMasterPasswordAsync(newPassword);
            Console.WriteLine("Master password set successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while setting the master password: " + ex.Message);
        }
    }



    private async Task ValidateMasterPasswordAsync()
    {
        Console.Write("Enter your master password: ");
        string password = ReadPassword(); // Use the new method
        bool isValid = await _masterPasswordService.ValidateMasterPasswordAsync(password);

        if (!isValid)
        {
            Console.WriteLine("Invalid master password. Exiting application.");
            Environment.Exit(0);
        }
        Console.WriteLine("Master password validated successfully.");
    }


    private async Task DisplayStoredPasswordsAsync()
    {
        var passwords = await _passwordService.GetAllPasswordsAsync();
        Console.WriteLine("\nStored Passwords:");

        foreach (var entry in passwords)
        {
            Console.WriteLine($"-{entry.Id}. {entry.App}: {entry.Password}");
        }

        if (passwords == null || !passwords.Any())
        {
            Console.WriteLine("No passwords stored.");
        }
    }

    private async Task AddNewPasswordAsync()
    {
        Console.Write("Enter the name of the application: ");
        string appName = Console.ReadLine();

        Console.WriteLine("Choose how to create a password:");
        Console.WriteLine("1. Enter your own password");
        Console.WriteLine("2. Generate a random password");
        var choice = Console.ReadLine();

        string password;

        if (choice == "1")
        {
            Console.Write("Enter your password: ");
            password = Console.ReadLine();
        }
        else
        {
            password = GenerateRandomPassword();
            Console.WriteLine($"Generated password: {password}");
        }

        // Store the password
        var passwordEntry = new PasswordEntry { App = appName, Password = password };
        await _passwordService.AddingEncryption(passwordEntry);
        Console.WriteLine("Password added successfully.");
    }

    private async Task DeletePasswordAsync()
    {
        Console.Write("Enter the ID of the password entry you want to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Please enter a valid number.");
            return;
        }

        // Ask for master password confirmation
        Console.Write("Please enter your master password for confirmation: ");
        string masterPassword = ReadPassword(); // Use the new method

        if (await _masterPasswordService.ValidateMasterPasswordAsync(masterPassword))
        {
            await _passwordService.DeletePasswordAsync(id);
            Console.WriteLine($"Password entry with ID {id} has been deleted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid master password. Deletion aborted.");
        }
    }


    private async Task DeleteMasterPasswordAsync()
    {
        Console.Write("Please enter your master password for confirmation: ");
        string masterPassword = ReadPassword();

        if (await _masterPasswordService.ValidateMasterPasswordAsync(masterPassword))
        {
            var masterPasswordToDelete = await _masterPasswordService.GetMasterPasswordAsync();

            await _masterPasswordService.DeleteMasterPasswordAsync(masterPasswordToDelete.Id); // Call to delete the master password

            Console.WriteLine("Master password has been removed successfully.");

            // Restart the application after deletion
            RestartApplication();
        }
        else
        {
            Console.WriteLine("Invalid master password. Deletion aborted.");
        }
    }

    private void RestartApplication()
    {
        // Clear the console
        Console.Clear();

        Console.WriteLine("The master password has been deleted. You can set a new master password.");

        // Here, you can call the method to set a new master password
        SetMasterPasswordAsync().GetAwaiter().GetResult();
    }

    private string ReadPassword()
    {
        StringBuilder passwordBuilder = new StringBuilder();
        ConsoleKeyInfo keyInfo;

        do
        {
            keyInfo = Console.ReadKey(intercept: true); // Does not show the key pressed
            if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
            {
                passwordBuilder.Append(keyInfo.KeyChar);
                Console.Write("*"); // Display '*' instead of the actual character
            }
            else if (keyInfo.Key == ConsoleKey.Backspace && passwordBuilder.Length > 0)
            {
                passwordBuilder.Remove(passwordBuilder.Length - 1, 1); // Remove the last character
                Console.Write("\b \b"); // Handle backspace properly
            }
        } while (keyInfo.Key != ConsoleKey.Enter); // Stop reading when Enter is pressed

        Console.WriteLine(); // Move to the next line after pressing Enter
        return passwordBuilder.ToString(); // Return the actual password typed
    }


    private string GenerateRandomPassword(int length = 12)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
        StringBuilder result = new StringBuilder();
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] data = new byte[length];
            rng.GetBytes(data);

            foreach (var b in data)
            {
                result.Append(validChars[b % validChars.Length]);
            }
        }
        return result.ToString();
    }
}
