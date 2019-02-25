using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Syntax;
using BCake.Parser.Syntax.Types;

namespace BCake {
    public class Application {
        static List<FileInfo> Files = new List<FileInfo>();

        public static int Main(string[] args) {
            Console.WriteLine("BananaCake Compiler Version 0.1.0");
            Console.WriteLine("Copyright PixelSnake 2019, all rights reserved");
            Console.WriteLine();

            ParseArguments(args);

            Namespace globalNamespace = null;
            var parsers = new List<Parser.Parser>();

            foreach (var f in Files) {
                Console.WriteLine(f.FullName);
                parsers.Add(new Parser.Parser(f.FullName));
            }

            try {
                foreach (var p in parsers) globalNamespace = p.ParseRoot();

                var namespaces = globalNamespace.Scope.AllMembers.Where(elem => elem is Namespace).Cast<Namespace>();
                foreach (var ns in namespaces) ns.ParseInner();

                namespaces = namespaces.Append(globalNamespace);
                foreach (var ns in namespaces) {
                    foreach (var m in ns.Scope.AllMembers.Where(elem => elem is ComplexType).Where(elem => !(elem is Namespace)).Cast<ComplexType>()) {
                        m.ParseInner();
                    }
                }
                foreach (var ns in namespaces) {
                    foreach (var m in ns.Scope.AllMembers.Where(elem => elem is ClassType)) {
                        foreach (var f in m.Scope.AllMembers.Where(elem => elem is FunctionType).Cast<FunctionType>()) {
                            f.ParseInner();
                        }
                    }
                }
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