using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NestManagerLibrary
{
    static class FileHelper
    {
        public static IEnumerable<string> GetFiles(string path)
        {
            // http://stackoverflow.com/questions/929276/how-to-recursively-list-all-the-files-in-a-directory-in-c
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                if (files == null) continue;

                foreach (var t in files)
                {
                    yield return t;
                }
            }
        }

    }
}
