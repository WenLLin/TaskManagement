using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

namespace Tests
{
    class Program
    {
        static string path = @"C:\Users\kensk\source\repos\Tests\Tests\TaskList.txt";
        static void Main(string[] args)
        {
            while (true)
            {
                Pages();

                ConsoleKeyInfo cki;
                do
                {
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\n1. Add task\t\t2. Select task\t\t\t\t\tExit: ESC\t\t <-- Go Back\t");
                    Console.ResetColor();
                    cki = Console.ReadKey(true);
                    switch (cki.Key)
                    {
                        case ConsoleKey.D1:
                            AddList();
                            break;
                        case ConsoleKey.D2:
                            SelectMenu(path);
                            break;
                        case ConsoleKey.LeftArrow:
                            break;
                        case ConsoleKey.Escape:
                            Environment.Exit(0);
                            break;
                        default:
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine("\n\t\tInvalid Operation, please try again.\t\t");
                            Console.ResetColor();
                            Thread.Sleep(2000);
                            break;
                    }
                    Console.Clear();
                    Pages();
                } while (cki.Key != ConsoleKey.LeftArrow);

                if (cki.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
        }

        static List<string> AddList()
        {
            List<String> taskList = ListGenerator();
            while (true)
            {
                Console.WriteLine("Type in your new task, then press <-- to go back.");

                ConsoleKeyInfo cki;
                cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.LeftArrow)
                {
                    break;
                }
                string addTask = Console.ReadLine();
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(addTask);
                }
                taskList.Add(addTask);
            }
            return taskList;
        }

        static void SelectMenu(string path)
        {
            try
            {
                List<string> taskList = QueueList();
                Console.Write("\nPlease enter the task number: ");

                int selectNum = Convert.ToInt32(Console.ReadLine());
                string selectTask = taskList[selectNum - 1];
                Console.WriteLine("\n1. Do it now\t2. Cross out\t3. Skip\n\n4. Re-enter\t5. Display information");

                ConsoleKeyInfo cki;
                cki = Console.ReadKey(true);
                switch (cki.Key)
                {
                    case ConsoleKey.D1:
                        int alreadyStart = 0;
                        foreach (string item in taskList)
                        {
                            if (item.Contains('☆'))
                            {
                                Console.BackgroundColor = ConsoleColor.Red;
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.WriteLine("\n\t\tFOCUS! One task at a time!\t\t");
                                Console.ResetColor();
                                Thread.Sleep(1500);
                                alreadyStart++;
                                break;
                            }
                        }

                        if (selectTask.Contains('★'))
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine("\n\t\tInvalid Operation, please try agin.\t\t");
                            Console.ResetColor();
                            Thread.Sleep(2000);
                        }
                        else if (alreadyStart == 0)
                        {
                            LineChanger('☆' + selectTask, path, selectNum);
                        }
                        break;
                    case ConsoleKey.D2:
                        if (selectTask.Contains('☆'))
                        {
                            LineChanger(selectTask.Replace('☆', '★'), path, selectNum);
                        }
                        else if (selectTask.Contains('★'))
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine("\n\t\tInvalid Operation, please try agin.\t\t");
                            Console.ResetColor();
                            Thread.Sleep(2000);
                        }
                        else LineChanger('★' + taskList[selectNum - 1], path, selectNum);
                        break;
                    case ConsoleKey.D3:
                        break;
                    case ConsoleKey.D4:
                        taskList.RemoveAt(selectNum - 1);
                        File.WriteAllLines(path, taskList);
                        selectTask = selectTask.Replace('★', 'ᥗ');
                        selectTask = selectTask.Replace('☆', 'ᥗ');
                        selectTask = selectTask.Insert(selectTask.Length, "√");
                        using (StreamWriter writer = new StreamWriter(path, true))
                        {
                            writer.WriteLine(selectTask);
                        }
                        break;
                    case ConsoleKey.D5:
                        int countCrossOut = selectTask.Where(c => c == 'ᥗ').Count();
                        int countReenter = selectTask.Where(c => c == '√').Count();
                        Console.WriteLine($"\nThis task has been crossed out for {countCrossOut} time(s) and re-enter for {countReenter} time(s)");
                        Console.ReadLine();
                        break;
                    default:
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine("\n\tInvalid Command, please try again.");
                        Console.ResetColor();
                        break;
                }
            }
            catch (Exception)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("\n\tInvalid Command, please try again.\t");
                Console.ResetColor();
                Thread.Sleep(1500);
            }
        }
        private static List<string> ListGenerator()
        {
            List<string> taskList = new List<string>();
            using (StreamReader sr = new StreamReader(path))
            {
                while (sr.Peek() >= 0)
                {
                    taskList.Add(sr.ReadLine());
                }
            }

            return taskList;
        }

        static List<string> QueueList()
        {
            List<string> taskList = ListGenerator();
            Queue<string> myQueue = new Queue<string>(taskList);

            do
            {
                if ((taskList != null) && (!taskList.Any()) || (myQueue.Count() == 0))
                {
                    Console.WriteLine("Let's add your first task ! Go to the Main Menu.\n");
                    break;
                }
                else
                {
                    var firstItem = myQueue.Peek();
                    if (firstItem.Contains('★'))
                    {
                        myQueue.Dequeue();
                    }
                    else if (!firstItem.Contains('★'))
                    {
                        break;
                    }
                }
            } while (true);
            string[] taskArray = myQueue.ToArray();
            taskList = taskArray.ToList();
            File.WriteAllLines(path, taskArray);
            return taskList;
        }

        static void LineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, arrLine);
        }

        static void TextColor(List<string> page)
        {
            int i = 1;
            foreach (var item in page)
            {
                if (item.Contains('☆'))
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write($"{i}. ");
                    Console.WriteLine(item.Where(c => c < 128).ToArray());
                    Console.ResetColor();
                    i++;
                    continue;
                }
                else if (item.Contains('★'))
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write($"{i}. ");
                    Console.WriteLine(item.Where(c => c < 128).ToArray());
                    Console.ResetColor();
                    i++;
                    continue;
                }
                else if (item.Contains('ᥗ') || item.Contains('√'))
                {
                    Console.Write($"{i}. ");
                    Console.WriteLine(item.Where(c => c < 128).ToArray());
                    i++;
                    continue;
                }
                Console.WriteLine($"{i}.  {item}");
                i++;
            }
        }

        private static void Pages()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n Task List: \n");
            Console.ResetColor();
            List<string> taskList = QueueList();
            List<string> firstPage = taskList.Take(20).ToList();
            TextColor(firstPage);
            ushort i = 0;
            ushort j = 1;
            Console.WriteLine($"\n\t\t\t\tPage {i + 1}\n\nNext Page: F1\t\t\t\t\tMain Menu: F3\t\tExit: ESC");
            ConsoleKeyInfo cki;

            while (true)
            {
                cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.F1)
                {
                    Console.Clear();
                    Console.WriteLine("Task List:\n");
                    var pageNext = taskList.Skip(20 * (i + 1)).Take(20).ToList();
                    i++;
                    TextColor(pageNext);
                    Console.WriteLine($"\n\t\t\t\tPage {i + 1}\n\nNext Page: F1\t\tPrevious Page: F2\tMenu: F3\t\tExit: ESC");
                }
                else if (cki.Key == ConsoleKey.F2)
                {
                    Console.Clear();
                    i--;
                    Console.WriteLine("Task List:\n");
                    var pageBack = taskList.Skip(20 * ((i + 1) - j)).Take(20).ToList();
                    TextColor(pageBack);
                    Console.WriteLine($"\n\t\t\t\tPage {i + 1}\n\nNext Page: F1\t\tPrevious Page: F2\tMenu: F3\t\tExit: ESC");
                }
                else if (cki.Key == ConsoleKey.F3)
                {
                    break;
                }
                else if (cki.Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
