﻿using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace NBitcoin.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (args.Length == 0)
                ShowUsage(options);
            if (Parser.Default.ParseArguments(args, options))
            {
                var network = options.Network.Equals("mainnet", StringComparison.InvariantCultureIgnoreCase)
                                        ? Network.Main :
                              options.Network.Equals("testnet", StringComparison.InvariantCultureIgnoreCase)
                              ? Network.TestNet : null;

                if (network == null)
                {
                    ShowUsage(options);
                    return;
                }
                if (options.GenerateKey)
                {
                    var key = new Key().GetBitcoinSecret(network);
                    Console.WriteLine("Private Key : " + key);
                    Console.WriteLine("Bitcoin Address : " + key.GetAddress());
                }

                if (options.GenerateEncryptedKey)
                {
                    Console.WriteLine("Password ? (input hidden from screen)");
                    var password = SecretReadLine();
                    while (true)
                    {
                        Console.WriteLine("Confirm, type the password again");
                        if (password == SecretReadLine())
                        {
                            break;
                        }
                        Console.WriteLine("You typed a different one");
                    }
                    var key = new Key().GetBitcoinSecret(network);
                    Console.WriteLine("Encrypted Key : " + key.Encrypt(password));
                    Console.WriteLine("Bitcoin Address : " + key.GetAddress());
                }

                if (options.DecryptEncryptedKey)
                {
                    Console.WriteLine("Encrypted Key ?");
                    BitcoinEncryptedSecretNoEC key = null;
                    while (true)
                    {
                        var encrypted = Console.ReadLine();
                        try
                        {
                            key = new BitcoinEncryptedSecretNoEC(encrypted);
                            break;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Wrong format");
                        }
                    }
                    Console.WriteLine("Password ? (input hidden from screen)");
                    BitcoinSecret secret = null;
                    while (true)
                    {
                        try
                        {
                            var password = SecretReadLine();
                            secret = key.GetSecret(password);
                            Console.WriteLine("Decrypted successfully");
                            Console.WriteLine("Bitcoin address : " + secret.GetAddress());
                            break;
                        }
                        catch (SecurityException ex)
                        {
                            Console.WriteLine("Wrong password");
                        }
                    }

                    Console.WriteLine("Print the decrypted key on screen ? (o/n)");
                    var r = Console.ReadLine();
                    if (r.Equals("o", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.WriteLine(secret);
                    }
                }
            }
        }

        private static string SecretReadLine()
        {
            String str = "";
            Boolean asd = true;
            while (asd)
            {
                char s = Console.ReadKey(true).KeyChar;
                if (s == '\r')
                {
                    asd = false;
                }
                else
                {
                    str = str + s.ToString();
                }
            }
            return str;
        }

        private static void ShowUsage(Options options)
        {
            System.Console.WriteLine(options.GetUsage());
        }
    }
}
