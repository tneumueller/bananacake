using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BCake {
    public class Application {
        static List<FileInfo> Files = new List<FileInfo>();

        public static int Main(string[] args) {
            Console.WriteLine("BananaCake Compiler Version 0.1.0");
            Console.WriteLine("Copyright PixelSnake 2019, all rights reserved");
            Console.WriteLine();

            ParseArguments(args);

            var parsers = new List<Parser.Parser>();
            var namespaces = new List<Parser.Syntax.Namespace>();
            var complexTypes = new List<Parser.Syntax.Types.ComplexType>();

            foreach (var f in Files) {
                Console.WriteLine(f.FullName);
                parsers.Add(new Parser.Parser(f.FullName));
            }

            try {
                foreach (var p in parsers) namespaces.AddRange(p.ParseNamespaces());
                foreach (var ns in namespaces) complexTypes.AddRange(ns.ParseSymbols(namespaces.ToArray()));
                foreach (var t in complexTypes) t.ParseInner(namespaces.ToArray(), complexTypes.ToArray());
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            return 0;
        }

        private static void ParseArguments(string[] args) {
            for (var i = 0; i < args.Length; ++i) {
                var arg = args[i];

                Files.Add(new FileInfo(arg));
            }
        }
    }
}