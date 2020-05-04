using MsbtEditor.UMSBT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMSBTextractor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Scripts folder unpacker for ACNL by @curoviyxru");
            Console.WriteLine("extract or repack or exit");
            while (true)
            {
                switch (Console.ReadLine())
                {
                    case "exit":
                        Console.WriteLine("goodbye");
                        return;
                    case "extract":
                        extract();
                        Console.WriteLine("extract or repack or exit");
                        break;
                    case "repack":
                        repack();
                        Console.WriteLine("extract or repack or exit");
                        break;
                    default:
                        Console.WriteLine("extract or repack or exit");
                        break;
                }
            }
        }

        private static void extract()
        {
            Console.Write("path to files (RomFS\\Scripts): ");
            string path = Console.ReadLine();
            if (!Directory.Exists(path)) {
                Console.WriteLine("path to files doesn't exists.");
                return;
            }
            Console.Write("extract to: ");
            string extract_path = Console.ReadLine();
            if (!Directory.Exists(extract_path))
            {
                Console.WriteLine("extract path doesn't exists.");
                return;
            }
            string temp_path = GetTemporaryDirectory();
            Console.Write("language id (0-4): ");
            int lang_id = -1;
            if (!int.TryParse(Console.ReadLine(), out lang_id)) lang_id = -1;
            if (lang_id < 0 || lang_id > 4)
            {
                Console.WriteLine("language id isn't valid.");
                return;
            }

            string[] files = Directory.GetFiles(path, "*.umsbt", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string file_path = file.Substring(path.Length);
                Console.WriteLine("processing " + file_path);
                UMSBT.Extract(file, temp_path);
                
                file_path = extract_path+file_path.Substring(0, file_path.Length - Path.GetFileName(file).Length);
                if (!Directory.Exists(file_path)) Directory.CreateDirectory(file_path);
                File.Copy(Path.Combine(temp_path, "0000000" + lang_id + ".msbt"), Path.Combine(file_path, Path.GetFileNameWithoutExtension(file)+".msbt"));
            }

            Directory.Delete(temp_path, true);
            Console.WriteLine("ok");
        }

        private static void repack()
        {
            Console.Write("path with original files (RomFS\\Scripts) or files need inject to: ");
            string path = Console.ReadLine();
            if (!Directory.Exists(path))
            {
                Console.WriteLine("path doesn't exists.");
                return;
            }
            Console.Write("path to modified/unpacked files: ");
            string extract_path = Console.ReadLine();
            if (!Directory.Exists(extract_path))
            {
                Console.WriteLine("path with unpacked files doesn't exists.");
                return;
            }
            string temp_path = GetTemporaryDirectory();
            Console.Write("language id (0-4): ");
            int lang_id = -1;
            if (!int.TryParse(Console.ReadLine(), out lang_id)) lang_id = -1;
            if (lang_id < 0 || lang_id > 4)
            {
                Console.WriteLine("language id isn't valid.");
                return;
            }

            string[] files = Directory.GetFiles(path, "*.umsbt", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string file_path = file.Substring(path.Length);
                Console.WriteLine("processing " + file_path);
                UMSBT.Extract(file, temp_path);
                var path_to = Path.Combine(temp_path, "0000000" + lang_id + ".msbt");
                File.Delete(path_to);
                file_path = extract_path + file_path.Substring(0, file_path.Length - Path.GetFileName(file).Length);
                File.Copy(Path.Combine(file_path, Path.GetFileNameWithoutExtension(file) + ".msbt"), path_to);
                File.Delete(file);
                UMSBT.Pack(file, temp_path);
            }

            Directory.Delete(temp_path, true);
            Console.WriteLine("ok");
        }

        public static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
