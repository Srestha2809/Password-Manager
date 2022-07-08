/*
 * Program:         PasswordManager.exe
 * Module:          PasswordManager.cs
 * Date:            2022-05-31
 * Authors:          Mitchell Hughes - Srestha Bharadwaj
 * Description:     Some free starting code for INFO-3138 project 1, the Password Manager
 *                  application. All it does so far is demonstrate how to obtain the system date 
 *                  and how to use the PasswordTester class provided.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;            
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace PasswordManager
{
    class Program
    {
        //file strings
        private static readonly string DATA_FILE = ".\\JSON\\accounts.json";
        private static readonly string SCHEMA_FILE = ".\\JSON\\account_schema.json";

        //json strings
        private static string json_schema;
        private static string json_data;

        private static DateTime date = DateTime.Now;
        static void Main(string[] args)
        {
            //Introduce the program
       
            Console.WriteLine("PASSWORD MANAGEMENT SYSTEM");
            Console.WriteLine("Implemented by Mitchell Hughes and Srestha Bharadwaj");
            Console.WriteLine("Date: " + date.ToLongDateString());
            Console.WriteLine("============================================");

            List<Account> accounts = new List<Account>();
            // if the application's JSON file is populated with data, read and deserialze it to the collection
            if(ReadFile(SCHEMA_FILE, out json_schema) && ReadFile(DATA_FILE, out json_data)){

                string json = File.ReadAllText(DATA_FILE);
                if (json != "")
                {
                    Console.WriteLine($"Data file found: \"{DATA_FILE}\"");
                    Console.WriteLine("Verifying data...");
  
                    JArray json_objects = JArray.Parse(json);
                    foreach (var account_object in json_objects)
                    {
                        if (ValidateJSON(account_object.ToString(), json_schema))
                        {
                            accounts.Add(JsonConvert.DeserializeObject<Account>(account_object.ToString()));
                            Console.WriteLine("+ Added valid account to list");
                        }
                        else
                        {
                            Console.WriteLine("- Invalid account not added to list");
                        }
                       
                    }
                    Console.WriteLine($"Added all valid accounts to list from file.");
                }
               
                bool done = false;
                do { 
                    Console.WriteLine("============================================");
                    if (accounts.Count > 0) 
                    {
                        Console.WriteLine("Account List");
                        for (int i = 0; i < accounts.Count; i++)
                        {
                            Console.WriteLine((i + 1) + ". " + accounts[i].description);
                        }
                        Console.WriteLine("============================================\n");
                        Console.WriteLine("Press any number from the list above to view the account information");
                    }

                    //user prompts
                    Console.WriteLine("Press A to add a new entry to the list.");
                    Console.WriteLine("Press X to exit the program.");
                    Console.Write("\nEnter a command: ");
                    string command = Console.ReadLine();

                    if (Char.IsDigit(command[0]) && accounts.Count > 0)
                    {
                        int selection = (int)Char.GetNumericValue(command[0]);
                        if (selection > 0 && selection <= accounts.Count)
                        {
                            Console.WriteLine("Editing account " + selection);
                            Console.WriteLine("============================================\n");
                            EditEntry(selection, accounts);
                        }
                        else Console.WriteLine("Invalid number, please try again.");
                    }

                    else
                    {
                        switch (command.ToLower())
                        {
                            case "a": 
                                Console.WriteLine("Adding new entry...");
                                Console.WriteLine("============================================\n");
                                AddEntry(accounts);
                                break;

                            case "x": 
                                Console.WriteLine("Exiting...");
                                done = true;
                                break;

                            default: 
                                Console.WriteLine("Invalid entry, please try again."); 
                                break;
                        }
                    }
                } while(!done);
            }//end main method

            //write all the new data to the json file
            string all_json = JsonConvert.SerializeObject(accounts);
            try
            {
                File.WriteAllText(DATA_FILE, all_json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /* Method:  ReadFile
         * Inputs:  string of path, out string of the json to return
         * Outputs: boolean value of true or false
         * Purpose: Reads a file and outputs it to a JSON string, returns false if it fails
         */
        private static bool ReadFile(string path, out string json)
        {
            try
            {
                // Read JSON file data 
                json = File.ReadAllText(path);
                return true;
            }
            catch
            {
                json = null;
                return false;
            }
        } // end ReadFile()

        /* Method:  ValidateJSON
         * Inputs:  string of json data, string of a json schema
         * Outputs: boolean value of true or false
         * Purpose: validates a set of json data against the given schema
         */
        private static bool ValidateJSON(string json_data, string json_schema)
        { 
            JSchema schema = JSchema.Parse(json_schema);
            JObject new_account = JObject.Parse(json_data);
            return new_account.IsValid(schema);
        }

        /* Method:  AddEntry
         * Inputs:  an account list
         * Outputs: void
         * Purpose: enters the submenu to add an account 
         */
        public static void AddEntry(List<Account> account_list)
        {
            Account account = new Account();
            Password password = new Password();

            bool valid = false;
            do
            {
                Console.WriteLine("Enter all of the information below. ");
                try
                {
                    Console.Write("Account description: ");
                    account.description = Console.ReadLine();
                    if (account.description == "") account.description = null;

                    Console.Write("User ID: ");
                    account.userid = Console.ReadLine();
                    if (account.userid == "") account.userid = null;

                    Console.Write("Password: ");
                    string password_text = Console.ReadLine();
                    account.password = (password_text != "") ? CreatePassword(password_text) : null;

                    Console.Write("Login URL: ");
                    account.loginurl = Console.ReadLine();
                    if (account.loginurl == "") account.loginurl = null;

                    Console.Write("Account Number: ");
                    account.accountnum = Console.ReadLine();

                    string account_json = JsonConvert.SerializeObject(account);
                    valid = ValidateJSON(account_json, json_schema);

                    if (!valid) Console.WriteLine("ERROR: Invalid JSON data has been entered.\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error has occurred, trying once more...");
                }
            } while(!valid);

            account_list.Add(account);
            Console.WriteLine("\nAccount added to list!");
        } // end AddEntry

        /* Method:  EditEntry
         * Inputs:  an integer account number, an account list
         * Outputs: void
         * Purpose: allows the user to edit the password or delete the account
         */
        public static void EditEntry(int account_number, List<Account> list)
        {
            Account account = list[account_number - 1];

            bool done = false;
            do
            {
                try
                {
                //display information
                Console.WriteLine("Account Name: " + account.description);
                Console.WriteLine("User ID: " + account.userid);
                Console.WriteLine("Password: " + account.password.value);
                Console.WriteLine("Password Strength: " + account.password.strengthtext + "(" + account.password.strengthnum + "%)");
                Console.WriteLine("Last Reset: " + account.password.lastreset);
                Console.WriteLine("\nLogin URL: " + account.loginurl);
                Console.WriteLine("Account Number: " + account.accountnum);

                Console.WriteLine("============================================");
               
                Console.WriteLine("Press P to change the password for this account.");
                Console.WriteLine("Press D to delete this account.");
                Console.WriteLine("Press M to return to the main menu.");

                //prompt user
                Console.Write("\nEnter a command: ");
                string command = Console.ReadLine();
                Password password = new Password();

                switch (command.ToLower())
                {
                        case "p":
                            Console.WriteLine("Changing password...");
                            Console.WriteLine("============================================\n");
                            Console.Write("Enter new password: ");
                            string password_text = Console.ReadLine();
                            if (password_text != "") account.password = CreatePassword(password_text);
                            else Console.WriteLine("Password cannot be blank!\n");
                        break;

                        case "d":
                            Console.WriteLine("Are you sure you want to delete this account? (Y/N)");
                            command = Console.ReadLine();

                            if(command.ToLower() == "y")
                            {
                                list.Remove(account);
                                done = true;
                                Console.WriteLine("\nDeleting account...");
                            }
                            break;

                        case "m":
                            Console.WriteLine("\nReturning to menu...");
                            done = true;
                            break;

                        default:
                            Console.WriteLine("\nInvalid entry, please try again.");
                            break;
                }
                Console.WriteLine("============================================\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unexpected error occurred, trying again...");
                }

            } while (!done);

        }//end EditEntry

        /* Method:  CreatePassword
         * Inputs:  a string with the given password text
         * Outputs: a password object
         * Purpose: creates and returns a password object given the password text
         */
        public static Password CreatePassword(string password_text)
        {
            Password password = new Password();
            PasswordTester tester = new PasswordTester(password_text);
            password.value = password_text;
            password.strengthtext = tester.StrengthLabel;
            password.strengthnum = tester.StrengthPercent;
            password.lastreset = date.ToShortDateString();
            return password;
        } // end CreatePassword
    } // end class

}
