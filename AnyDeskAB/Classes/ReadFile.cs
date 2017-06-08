using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyDeskAB.Classes {
    public static class ReadFile {
        public static IEnumerable<string> ReadLines(string fileName) {
            FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            using(var stream = file)
            using(var reader = new StreamReader(stream, Encoding.UTF8)) {
                string line;
                while((line = reader.ReadLine()) != null) {
                    yield return line;
                }
            }
        }
    }
}
