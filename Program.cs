using System;
using System.Linq;
using System.Net;
using System.Reflection;
using AutoFrameWork;
using AutoFrameWork.Schema;
using AutoFrameWork.Utility;

namespace AutoFrameWork
{
    class Program
    {
        static void Main(string[] args)
        {
         
            new Launch().Config(t =>
                         {

                             t.LogFolderPath = "Result";

                         }).Start();

        }



    }


}
