using CommandLine;
using IpCameraClient.Db;
using IpCameraClient.Model;
using IpCameraClient.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace IpCamera.Console
{
    class Program
    {
        static void Main()
        {
            //var args = "add 2 qwaszx zxasqw 1.23";

            //var args = "data 2 02.03.14";

            //Parser.Default.ParseArguments<Options, Options2>(args.Split(' '))
            //    .WithParsed<Options>(o => System.Console.WriteLine($"{o.IntValue} {o.StringSeq[0]} {o.StringSeq[1]} {o.DoubleValue}"))
            //    .WithParsed<Options2>(o => System.Console.WriteLine($"{o.IntValue} {o.StringSeq}"));


        }

        [Verb("add", HelpText = "Add file contents to the index.")]
        class Options
        {
            [Value(0)]
            public int IntValue { get; set; }

            [Value(1, Min = 1, Max = 3)]
            public List<string> StringSeq { get; set; }

            [Value(2)]
            public double DoubleValue { get; set; }
        }

        [Verb("data", HelpText = "Add file contents to the index.")]
        class Options2
        {
            [Value(0)]
            public int IntValue { get; set; }

            [Value(1)]
            public DateTime StringSeq { get; set; }

        }
    }
}