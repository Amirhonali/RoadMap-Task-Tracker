using System.Text.Json;
using System.Xml.Linq;
using TaskTracker;

class Program
{
    const string FileName = "tasks.json";

    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine(@"Togri komandani kirgaz misol uchun add  //Moshina yuv , delete, update, list");
            return;
        }

        var command = args[0].ToLower();
        var task = LoadTasks();

        switch (command)
        {
            case "add":
                if (args.Length < 2)
                {
                    Console.WriteLine("Nma qilish kerakligini kirgaz!");
                    return;
                }
                var newTask = new TaskClass
                {
                    Id = task.Count > 0 ? task.Max(t => t.Id) + 1 : 1,
                    Title = args[1]
                };
                task.Add(newTask);
                SaveTask(task);
                Console.WriteLine($"ID: {newTask.Id} \nTitle: {newTask.Title}");
                break;

            case "update":
                if (args.Length < 3 || !int.TryParse(args[1], out int updateId))
                {
                    Console.WriteLine("ID ni krigaz");
                    return;
                }
                var taskToUpdate = task.FirstOrDefault(t => t.Id == updateId);
                if (taskToUpdate != null)
                {
                    taskToUpdate.Title = args[2];
                    SaveTask(task);
                    Console.WriteLine("Obnavleno");
                }
                else
                {
                    Console.WriteLine("Zadacha topilmadi");
                }
                break;

            case "delete":
                if (args.Length < 2 || !int.TryParse(args[1], out int deleteId))
                {
                    Console.WriteLine("Ochirish uchun Id ni krit: ");
                    return;
                }
                var taskToDelete = task.FirstOrDefault(t => t.Id == deleteId);
                if (taskToDelete != null)
                {
                    task.Remove(taskToDelete);
                    SaveTask(task);
                    Console.WriteLine("Zadacha ochirildi");
                }
                else
                {
                    Console.WriteLine($"Zadacha topilmadi");
                }
                break;

            case "start":
                SetStatus(task, args, "inprogress");
                break;
            case "completed":
                SetStatus(task, args, "completed");
                break;
            case "list":
                string filter = args.Length > 1 ? args[1].ToLower() : "all";
                var filteredTasks = filter switch
                {
                    "completed" => task.Where(t => t.Status == "completed"),
                    "pending" => task.Where(t => t.Status == "pending"),
                    "inprogress" => task.Where(t => t.Status == "inprogress"),
                    _ => task
                };
                foreach (var t in filteredTasks)
                    Console.WriteLine($"[{t.Id}] ({t.Status}) {t.Title}");
                break;

            default:
                Console.WriteLine("Notori komanda");
                break;
        }
    }
    static void SetStatus(List<TaskClass> tasks, string[] args, string status)
    {
        if (args.Length < 2 || !int.TryParse(args[1], out int id))
        {
            Console.WriteLine("ID ni kriting.");
            return;
        }

        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            task.Status = status;
            SaveTask(tasks);
            Console.WriteLine($"{id} lik zadachani statusi ozgardi {status}");
        }
        else
        {
            Console.WriteLine("Zadacha topilmadi!!!");
        }
    }

    static List<TaskClass> LoadTasks()
    {
        if (!File.Exists(FileName)) 
            return new List<TaskClass>();

        var json = File.ReadAllText(FileName);

        if (string.IsNullOrWhiteSpace(json)) 
            return new List<TaskClass>();

        return JsonSerializer.Deserialize<List<TaskClass>>(json) ?? new List<TaskClass>();
    }

    static void SaveTask(List<TaskClass> tasks)
    {
        var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FileName, json);
    }
}