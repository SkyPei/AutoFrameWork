using System;
using System.Collections.Generic;
using System.Reflection;
using ApiFrameWork.Exception;
using ApiFrameWork.Schema;
using System.Linq;
using System.Text.RegularExpressions;


namespace ApiFrameWork.Command
{
    internal class CommandCenter
    {
        internal static void Startup(List<System.Type> list, string dataFile, string sheetName)
        {
            while (true)
            {
                var input = Console.ReadLine().ToLower();
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
                bc.DataFile = dataFile;
                bc.SheetName = sheetName;
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
                                    arg.SetValue(bc,data.Substring(1,data.Length-2));

                                }
                                else
                                {
                                    Regex regdata = new Regex($"(?<=[ ]+--{arg.Name}[ ]*=[ ]*)[\\S]+", RegexOptions.IgnoreCase);
                                    string data = regdata.Match(inputargs).Value;
                                    arg.SetValue(bc,data);

                                }
                            }
                        }


                        bc.Run(inputargs);
                    }
                    // else
                    // {
                    //     string option = inputs[1].Substring(1);
                    //     List<string> optionValues = new List<string>();
                    //     for (int i = 2; i < inputs.Length; i++)
                    //     {
                    //         if (inputs[i].StartsWith("-"))
                    //         {
                    //             break;
                    //         }

                    //         optionValues.Add(inputs[i]);
                    //     }



                    //     var method = bc.GetType().GetMethods().FirstOrDefault(t => t.Name.ToLower() == $"{option}Option" && t.GetParameters().Length == optionValues.Count);
                    //     if (method == null)
                    //     {
                    //         Console.WriteLine($"-{option} is not an invalid command option");
                    //     }
                    //     else
                    //     {
                    //         if (optionValues.Count == 0)
                    //         {
                    //             method.Invoke(bc, null);
                    //         }
                    //         else
                    //         {
                    //             method.Invoke(bc, optionValues.ToArray());

                    //         }
                    //     }

                    // }
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