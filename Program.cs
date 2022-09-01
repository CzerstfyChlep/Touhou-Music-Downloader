using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace MusicDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create new playlist file? [new | append | <enter>]");
            bool append = false;
            string ans =  Console.ReadLine();
            if (ans == "new")
            {
                StreamReader sr = new StreamReader("TouhouPlaylist.txt");
                CreateToDownload(sr);
            }
            else if(ans == "append")
            {
                StreamReader sr = new StreamReader("TouhouPlaylist.txt");
                CreateToDownload(sr, true);
            }
            Console.Clear();
            string[] alllines = File.ReadAllLines("todownload.txt");

            string[] Files = Directory.GetFiles("Download");
            for (int a = 0; a < alllines.Length; a++)
            {
                string[] text = alllines[a].Split(' ');
                if (Files.Any(x => x.Contains(text[0] + ".webm")))
                    alllines[a] = text[0] + " true";
              
            }
            File.WriteAllLines("todownload.txt", alllines);

            int n = 0;
            for (int a = 0; a < alllines.Length; a++)
            {
                string[] text = alllines[a].Split(' ');
                WriteProgress(a, alllines.Length, 50, text[0]);
                
                if (text[1] == "false")
                {                   
                    n++;
                    ProcessStartInfo psi = new ProcessStartInfo("CMD.exe", $"/C youtube-dl.exe -xf 251 https://www.youtube.com/watch?v={text[0]} --write-thumbnail --output \"Download/{text[0]}.%(ext)s\"");
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;
                    Process p = Process.Start(psi);
                    p.WaitForExit();
                    if(Directory.GetFiles("Download").Any(x=> x.Contains(text[0] + ".webm")))
                        alllines[a] = text[0] + " true";
                    else
                        alllines[a] = text[0] + " error";                  
                }

                if(n % 5 == 0)
                {
                    File.WriteAllText("todownload.txt", "");
                    File.WriteAllLines("todownload.txt", alllines);                                     
                }               
            }
            File.WriteAllText("todownload.txt", "");
            File.WriteAllLines("todownload.txt", alllines);

        }


        //youtube-dl.exe -xf 251 https://www.youtube.com/watch?v=27hJm9jgoUg --write-thumbnail --output "Music/%(title)s.%(ext)s"

        static void WriteProgress(int val, int max, int lng, string link = "none")
        {
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            Console.Write('[');
            int count = (int)Math.Floor((double)val / max * lng);
            Console.Write(new string('▓', count));
            Console.Write(new string(' ', lng - count));
            Console.Write($"] {val}/{max} ({(int)Math.Floor((double)val / max * 100)}%)");
            if(link != "none")
            {
                Console.CursorTop = 1;
                Console.CursorLeft = 0;
                Console.Write("Currently downloading: " + link);
                Console.CursorTop = 0;
            }
            else
            {
                Console.CursorTop = 1;
                Console.CursorLeft = 0;
                Console.Write("                                       ");
                Console.CursorTop = 0;
            }
        }

        static void CreateToDownload(StreamReader sr, bool append = false)
        {
            string[] lines = null;
            if (!append)
            {
                File.WriteAllText("todownload.txt", "");
            }
            else
            {
               lines = File.ReadAllLines("todownload.txt");
            }
            while (true)
            {                
                string line = sr.ReadLine();     
                if (line == null)
                    break;            
               
                string link = line.Split('|')[1].Trim();
                if (append && lines.Any(x => x.Contains(link)))
                {
                    continue;
                }
                File.AppendAllText("todownload.txt", link + " false \n");
            }
        }
    }
}
