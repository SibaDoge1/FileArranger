using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace FileArranger
{
    class FileArranger
    {
        public static void Main()
        {
            int index;
            int isOk;
            do
            {
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("뭐할거임");
                Console.WriteLine("1. 많으면 폴더 묶기");
                Console.WriteLine("2. 폴더있으면 넣기");
                Console.WriteLine("3. 폴더에서 꺼내기");
                Console.WriteLine("4. 끄읏" + "\n");
                index = int.Parse(Console.ReadLine());
                if (index == 4) break;

                Console.WriteLine("현재입력 : " + index);
                Console.WriteLine("진행할려면 1 입력");
                isOk = int.Parse(Console.ReadLine());
                if (isOk != 1) break;

                string path = readPath();
                if (path == null)
                {
                    Console.WriteLine("path.ini 파일이 없습니다");
                    break;
                }
                path.Replace(@"\", @"//");
                changePath(path);

                switch (index)
                {
                    case 1: makeDir(path); break;
                    case 2: checkAndMove(path); break;
                    case 3: getOut(path); break;
                    default: break;
                }
            } while (true);

            Console.WriteLine("아무키나 눌러 종료.");
            Console.ReadKey();
        }

        public static string readPath()
        {
            string path = "";
            try
            {
                StreamReader file = new StreamReader("path.ini");
                path = file.ReadLine();
                file.Close();
            }
            catch(FileNotFoundException e)
            {
                return null;
            }
            return path;
        }

        public static void changePath(string path)
        {
            Console.WriteLine("\n" + "현재 폴더 경로 : " + path);
            Console.WriteLine("안바꿀려면 1 입력");
            string customPath = Console.ReadLine();
            if (customPath.Equals("1") == false)
            {
                path = customPath.Replace(@"\", @"//");
            }
            Console.WriteLine("현재 폴더 경로 : " + path);
        }

        public static string choosePattern()
        {
            string regex;
            Console.WriteLine("구분을 위한 템플릿을 고르셈");
            Console.WriteLine("1. [작가] 작품명 ");
            Console.WriteLine("2. 작가 - 작품명 ");
            int index = int.Parse(Console.ReadLine());
            switch (index)
            {
                case 1: regex = @"(?<=^\[).*?(?=\])";  break;
                case 2: regex = @"^.*?(?=\s-\s.*)";  break;
                default: regex = null; break;
            }
            return regex;
        }

        public static void makeDir(string path)
        {
            int minNum;
            int cnt = 0;
            string name = "";
            Queue<String> queue = new Queue<string>();
            string author;

            string regex = choosePattern();
            if(regex == null)
            {
                Console.WriteLine("템플릿을 제대로 고릅시다.");
                return;
            }

            Console.WriteLine("몇개부터 폴더만듦?");
            minNum = int.Parse(Console.ReadLine());

            if (System.IO.Directory.Exists(path))
            {
                //DirectoryInfo 객체 생성
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

                foreach (var item in di.GetFiles().OrderBy(x => x.Name).ToArray())
                {
                    author = Regex.Match(item.Name, regex).Value;
                    if (author.Length == 0) continue;
                    Console.WriteLine(author);
                    if (author.ToLower().Equals(name.ToLower()))
                    {
                        cnt++;
                        queue.Enqueue(item.Name);
                    }
                    else
                    {
                        if (cnt >= minNum)
                        {
                            if (System.IO.Directory.Exists(path + "//" + name) == false)
                            {
                                Console.WriteLine("뭐여");
                                System.IO.Directory.CreateDirectory(path + "//" + name);
                            }
                            foreach (var str in queue)
                            {
                                File.Move(path + "//" + str, path + "//" + name + "//" + str);
                            }
                        }
                        queue.Clear();
                        name = author;
                        cnt = 1;
                        queue.Enqueue(item.Name);
                    }
                }
                if (cnt >= minNum)
                {
                    if (System.IO.Directory.Exists(path + "//" + name) == false)
                    {
                        Console.WriteLine("뭐여");
                        System.IO.Directory.CreateDirectory(path + "//" + name);
                    }
                    foreach (var str in queue)
                    {
                        File.Move(path + "//" + str, path + "//" + name + "//" + str);
                    }
                }
            }
            else
            {
                Console.WriteLine("폴더가 없습니다.");
            }
        }

        public static void checkAndMove(string path)
        {
            string author;

            string regex = choosePattern();
            if(regex == null)
            {
                Console.WriteLine("템플릿을 제대로 고릅시다.");
                return;
            }

            //해당 폴더가 존재하는지 확인
            if (System.IO.Directory.Exists(path))

            {
                //DirectoryInfo 객체 생성
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

                var dirs = di.GetDirectories();
                foreach (var item in di.GetFiles().OrderBy(x => x.Name).ToArray())
                {
                    author = Regex.Match(item.Name, regex).Value;
                    if (author.Length == 0) continue;
                    Console.WriteLine(author);
                    foreach (var dir in dirs)
                    {
                        if (dir.Name.ToLower().Equals(author.ToLower()))
                        {
                            File.Move(path + "//" + item.Name, path + "//" + dir.Name + "//" + item.Name);
                            Console.WriteLine(item.Name + " : Moved");
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine("폴더가 없습니다.");
            }
        }

        public static void getOut(string path)
        {
            string author;
            if (System.IO.Directory.Exists(path))

            {
                //DirectoryInfo 객체 생성
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
                //해당 폴더에 있는 파일이름을 출력
                foreach (var item in di.GetDirectories())
                {
                    author = item.Name;
                    Console.WriteLine(author);
                    foreach (var file in item.GetFiles())
                    {
                        File.Move(path + "//" + author + "//" + file.Name, path + "//" + file.Name);
                    }
                }

            }
            else
            {
                Console.WriteLine("폴더가 없습니다.");
            }
        }
    }
}
