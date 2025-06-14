using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFrameWork.Exception;
using AutoFrameWork.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using NPOI.Util;


namespace AutoFrameWork.Command
{
    internal class CommandCenter
    {


        internal static void Startup(List<System.Type> list, LaunchConfig config, string com, object input)
        {
            if (string.IsNullOrEmpty(com))
            {
                Console.WriteLine("command is blank!");
                return;
            }

            Assembly assembly=typeof(BaseCommand).Assembly;
            var command =assembly.GetTypes().FirstOrDefault(t => t.Name.ToLower()==$"{com}command" && t.GetInterface("IJsonCommand")!=null);
            if (command == null)
            {
                Console.WriteLine($"{com} is not an invalid command");
                return;
            }
            BaseCommand bc = (BaseCommand) Activator.CreateInstance(command,null);
            bc.List = list;
            bc.DataFile = config.DataFile;
            bc.SheetName = config.SheetName;
            bc.Upload = config._upload;
            try
            {
                ((IJsonCommand)bc).Run(input);
            }
            catch (NotImplementedException e)
            {
                Console.WriteLine("Invalid Command args");
            }
            catch(CommandPaseException e)
            {
                Console.WriteLine(e.Message);
            }
            catch(System.Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        internal static void Startup(List<System.Type> list, LaunchConfig config, string input)
        {
            bool continueflag = true;
            while (continueflag)
            {
                if (string.IsNullOrEmpty(input))
                {
                    input = Console.ReadLine().ToLower();
                }
                var inputs = input.Split(" ", 2, StringSplitOptions.RemoveEmptyEntries);
                if (inputs.Length == 0)
                {
                    continue;
                }
                string comm = inputs[0].Trim();
                Assembly assembly = typeof(BaseCommand).Assembly;
                var command = assembly.GetTypes().FirstOrDefault(t => t.Name.ToLower() == $"{comm}command" && t.BaseType.Name == "BaseCommand");
                if (command == null)
                {
                    Console.WriteLine($"{comm} is not an invalid command");
                    continue;
                }

                BaseCommand bc = (BaseCommand)Activator.CreateInstance(command, null);
                bc.List = list;
                bc.DataFile = config.DataFile;
                bc.SheetName = config.SheetName;
                bc.Upload = config._upload;
                try
                {
                    System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"(^|[ ]+)--help($|[ ]+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (inputs.Length == 1)
                    {
                        bc.Run();
                    }
                    else if (reg.IsMatch(inputs[1].Trim()))
                    {
                        bc.Help();
                    }
                    else //(!inputs[1].StartsWith("-"))
                    {
                        string inputargs = inputs[1].Trim();
                        var args = command.GetProperties().Where(t => t.GetCustomAttribute(typeof(CommandAttribute)) != null).ToList();
                        foreach (var arg in args)
                        {
                            Regex argreg = new Regex($"[ ]+--{arg.Name}[ ]*=[ ]*\\\"?", RegexOptions.IgnoreCase);
                            if (argreg.IsMatch(inputargs))
                            {
                                if (argreg.Match(inputargs).Value.Last() == '\"')
                                {
                                    Regex regdata = new Regex($"(?<=[ ]+--{arg.Name}[ ]*=[ ]*)\\\"[\\S ]+\\\"", RegexOptions.IgnoreCase);
                                    string data = regdata.Match(inputargs).Value;
                                    arg.SetValue(bc, data.Substring(1, data.Length - 2));

                                }
                                else
                                {
                                    Regex regdata = new Regex($"(?<=[ ]+--{arg.Name}[ ]*=[ ]*)[\\S]+", RegexOptions.IgnoreCase);
                                    string data = regdata.Match(inputargs).Value;
                                    arg.SetValue(bc, data);

                                }
                            }
                        }


                        continueflag = bc.Run(inputargs);
                    }

                }
                catch (NotImplementedException e)
                {
                    Console.WriteLine("Invalid Command args");
                    continue;
                }
                catch (CommandPaseException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }


            }

        }


    }


    public class CommandArgsModel
    {
        public string Name
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }
    }

}