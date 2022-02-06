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
            Console.OutputEncoding = System.Text.Encoding.Default;
            int index;
            int isOk;
            do
            {
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("뭐할거임");
                Console.WriteLine("1. 많으면 폴더 묶기");
                Console.WriteLine("2. 폴더있으면 넣기");
                Console.WriteLine("3. 폴더에서 꺼내기");
                Console.WriteLine("4. 파일에 상위 폴더이름붙이기");
                Console.WriteLine("5. 파일에 상위 폴더이름붙이기(재귀적)");
                Console.WriteLine("6. 이름 지우기");
                Console.WriteLine("0. 끄읏" + "\n");
                try
                {
                    index = int.Parse(Console.ReadLine());
                }catch(Exception e)
                {
                    continue;
                }
                if (index == 0) break;

                Console.WriteLine("현재입력 : " + index);
                Console.WriteLine("진행할려면 1 입력");
                try
                {
                    isOk = int.Parse(Console.ReadLine());
                }
                catch (Exception e)
                {
                    continue;
                }
                if (isOk != 1) continue;

                string path = readPath();
                if (path == null)
                {
                    Console.WriteLine("path.ini 파일이 없습니다");
                    changePath(path);
                    continue;
                }
                if (path != null)
                {
                    path.Replace(@"\", @"//");
                }
                path = changePath(path);

                switch (index)
                {
                    case 1:
                        makeDir(path);
                        break;
                    case 2:
                        checkAndMove(path);
                        break;
                    case 3:
                        getOut(path);
                        break;
                    case 4:
                        nameFolder(path);
                        break;
                    case 5:
                        nameFolderRecur(path, null);
                        break;
                    case 6:
                        nameFolderRecur2(path);
                        break;
                    default:
                        Console.WriteLine("잘못된 번호입니다.");
                        continue;
                }
                Console.WriteLine("");
            } while (true);

            Console.WriteLine("아무키나 눌러 종료.");
            Console.ReadKey();
        }

        public static string readPath()
        {
            string path = "";
            try
            {
                StreamReader file = new StreamReader("path.ini", Encoding.Default);
                path = file.ReadLine();
                file.Close();
            }
            catch (FileNotFoundException e)
            {
                return null;
            }

            return path;
        }

        public static string changePath(string path)
        {
            Console.WriteLine("\n" + "현재 폴더 경로 : " + path);
            Console.WriteLine("안바꿀려면 1 입력");
            Console.WriteLine("바꿀려면 경로를 입력하세요");
            string customPath = Console.ReadLine();
            int isOk=0;

            try
            {
                isOk = int.Parse(customPath);
            }
            catch (Exception e)
            {
                path = customPath.Replace(@"\", @"//");
                path = changePath(path);
                return path;
            }
            if (isOk == 1) return path;
            else
            {
                path = customPath.Replace(@"\", @"//");
                path = changePath(path);
                return path;
            }

        }

        public static string choosePattern()
        {
            string regex;
            Console.WriteLine("구분을 위한 템플릿을 고르세요");
            Console.WriteLine("1. [작가] 작품명 ");
            Console.WriteLine("2. 작가 - 작품명 ");
            Console.WriteLine("3. 작가space작품명(as much as longer");
            int index;
            try
            {
                index = int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                regex = choosePattern();
                return regex;
            }
            switch (index)
            {
                case 1:
                    regex = @"(?<=^\[).*?(?=\])";
                    break;
                case 2:
                    regex = @"^.*?(?=\s-\s.*)";
                    break;
                case 3:
                    regex = @"^.*(?=\s.*)";
                    break;
                default:
                    regex = choosePattern();
                    break;
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
            if (regex == null)
            {
                Console.WriteLine("템플릿을 제대로 고릅시다.");
                return;
            }

            Console.WriteLine("몇개부터 폴더만들까요?");
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
            if (regex == null)
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
            string filename;
            int duplicateNum;
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
                        duplicateNum = 0;
                        filename = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
                        while (true)
                        {
                            try
                            {
                                if (duplicateNum == 0)
                                {
                                    File.Move(path + "//" + author + "//" + file.Name, path + "//" + file.Name);
                                    break;
                                }
                                else
                                {
                                    File.Move(path + "//" + author + "//" + file.Name,
                                        path + "//" + filename + " (" + duplicateNum + ")" + file.Extension);
                                    duplicateNum = 0;
                                    break;
                                }
                            }
                            catch (IOException e)
                            {
                                duplicateNum++;
                            }
                        }
                    }
                    foreach (var dir in item.GetDirectories())
                    {
                        duplicateNum = 0;
                        filename = dir.Name;
                        while (true)
                        {
                            try
                            {
                                if (duplicateNum == 0)
                                {
                                    Directory.Move(path + "//" + author + "//" + filename, path + "//" + dir.Name);
                                    break;
                                }
                                else
                                {
                                    Directory.Move(path + "//" + author + "//" + filename,
                                        path + "//" + filename + " (" + duplicateNum + ")");
                                    duplicateNum = 0;
                                    break;
                                }
                            }
                            catch (IOException e)
                            {
                                duplicateNum++;
                            }
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine("폴더가 없습니다.");
            }
        }

        public static void nameFolder(string path)
        {
            string author;
            string filename;
            int duplicateNum;
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
                        duplicateNum = 0;
                        filename = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
                        while (true)
                        {
                            try
                            {
                                if (duplicateNum == 0)
                                {
                                    File.Move(path + "//" + author + "//" + file.Name,
                                        path + "//" + author + "//" + author + " - " + file.Name);
                                    break;
                                }
                                else
                                {
                                    File.Move(path + "//" + author + "//" + file.Name,
                                        path + "//" + author + "//" + author + " - " + filename + " (" + duplicateNum + ")" +
                                        file.Extension);
                                    duplicateNum = 0;
                                    break;
                                }
                            }
                            catch (IOException e)
                            {
                                duplicateNum++;
                            }
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine("폴더가 없습니다.");
            }
        }

        public static void nameFolderRecur(string path, string prePath)
        {
            string filename;
            int duplicateNum;
            if (System.IO.Directory.Exists(path))

            {
                //DirectoryInfo 객체 생성
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
                //해당 폴더에 있는 파일이름을 출력
                foreach (var item in di.GetDirectories())
                {
                    Console.WriteLine(item.Name);
                    if (prePath == null)
                        nameFolderRecur(item.FullName, item.Name);
                    else
                        nameFolderRecur(item.FullName, prePath + "__"+ item.Name);

                }
                foreach (var file in di.GetFiles())
                {
                    duplicateNum = 0;
                    filename = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
                    while (true)
                    {
                        try
                        {
                            if (duplicateNum == 0)
                            {
                                File.Move(path  + "//" + file.Name,
                                    path + "//" + prePath + "__" + di.Name + "__" + file.Name);
                                break;
                            }
                            else
                            {
                                File.Move(path + "//" + file.Name,
                                    path + "//" + prePath + "__" + di.Name + "__" + filename + " (" + duplicateNum + ")" + file.Extension);
                                duplicateNum = 0;
                                break;
                            }
                        }
                        catch (IOException e)
                        {
                            duplicateNum++;
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine("폴더가 없습니다.");
            }
        }

        public static void nameFolderRecur2(string path)
        {
            string filename;
            int duplicateNum;
            string author;

            if (System.IO.Directory.Exists(path))

            {
                //DirectoryInfo 객체 생성
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
                //해당 폴더에 있는 파일이름을 출력
                foreach (var item in di.GetDirectories())
                {
                    Console.WriteLine(item.Name);
                    nameFolderRecur2(item.FullName);

                }
                foreach (var file in di.GetFiles())
                {
                    duplicateNum = 0;
                    filename = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
                    author = Regex.Match(filename, @"^.*\s(?=.*)").Value;
                    Console.WriteLine(filename.Substring(author.Length));
                    while (true)
                    {
                        try
                        {
                            if (duplicateNum == 0)
                            {
                                File.Move(path + "//" + file.Name,
                                    path + "//" + filename.Substring(author.Length) + file.Extension);
                                break;
                            }
                            else
                            {
                                File.Move(path + "//" + file.Name,
                                    path + "//" + filename.Substring(author.Length) + " (" + duplicateNum + ")" + file.Extension);
                                duplicateNum = 0;
                                break;
                            }
                        }
                        catch (IOException e)
                        {
                            duplicateNum++;
                        }
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
