using System;
using System.IO;
using System.Text.RegularExpressions;
using AutoFrameWork.Schema;

namespace AutoFrameWork.Command
{
    public class GenCommand : BaseCommand
    {
        [Command]
        public string output
        {
            get; set;
        }

        public override void Help()
        {
            Console.WriteLine("Generates template files of this API Framework.");
            Console.WriteLine("");
            Console.WriteLine("gen [type] [name] {--output=directory path}");
            Console.WriteLine("");
            Console.WriteLine("    [type]");
            Console.WriteLine("          Specifies the type of template files, now only support \"batch\" for batch scripts run.");
            Console.WriteLine("    [name]");
            Console.WriteLine("          Specifies the name of template file. Extension name is not required.");
            Console.WriteLine("");
            Console.WriteLine("  --output                 Specifies the output directory path, by default is current directory path.");

        }

        public override bool Run()
        {
            throw new System.NotImplementedException();
        }

        public override bool Run(string args)
        {
            string data = string.Empty;
            Regex regjson1 = new Regex("(^|[ ]+)batch[ ]+\\\"[\\S ]+\\\"", RegexOptions.IgnoreCase);
            Regex regjson2 = new Regex("(^|[ ]+)batch[ ]+[\\S]+($|(?= ))", RegexOptions.IgnoreCase);
            if (regjson1.IsMatch(args))
            {
                string matchedvalue = regjson1.Match(args).Value;
                int ind = matchedvalue.IndexOf("\"");
                data = matchedvalue.Substring(ind + 1, matchedvalue.Length - ind - 2);
            }
            else if (regjson2.IsMatch(args))
            {
                string matchedvalue = regjson2.Match(args).Value;
                data = matchedvalue.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1];

            }
            else
            {
                throw new Exception.CommandPaseException($"Invalid command format.");
            }

            Regex regjson3 = new Regex(@"^[\S ]+(?=.json)", RegexOptions.IgnoreCase);
            if (!regjson3.IsMatch(data))
            {
                data = data + ".json";
            }

            Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings();
            setting.Formatting = Newtonsoft.Json.Formatting.Indented;
            Schema.BatchTemplate template=  new Schema.BatchTemplate();
            template.Batch=new System.Collections.Generic.List<BatchGroup>();
            BatchGroup group = new BatchGroup();
            group.Groups = new System.Collections.Generic.List<BatchGroupItem>();
            template.Batch.Add(group);
            BatchGroupItem item = new BatchGroupItem();
            group.Groups.Add(item);
            string content = Newtonsoft.Json.JsonConvert.SerializeObject(template, setting);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(content);
            if (!string.IsNullOrEmpty(output))
            {
                if (System.IO.Directory.Exists(output))
                {
                    data = output + "\\" + data;
                }
                else
                {
                    throw new Exception.CommandPaseException($"Invalid path for output directory.");
                }

            }


            using (FileStream fs = File.Create(data))
            {
                fs.Write(bytes);
            }

            Console.WriteLine("Template is generated.");
            return true;
        }
    }
}