using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;






class taskManager
{
    public static void Main(String[] args)
    {
        string input = null;

        Console.WriteLine("Enter your Name");
        User mainUser = new User(Console.ReadLine());
        Console.WriteLine(mainUser.getName());

        mainUser.userTaskList.printTasks();

        while (input != "0")
        {
            Console.Clear();
            Console.WriteLine("Welcome " + mainUser.getName() + ".\nHere is a list of your current tasks:\n");
            mainUser.userTaskList.printTasks();

            Console.WriteLine("\nWhat would you like to do?");
            Console.WriteLine("1: Add Task\n2: Remove Task\n3: Edit Task\n4: Create New Task Group\n5: Delete Task Group\n6: View Task Groups\n7: Open Task Group\n8: Sort Tasks\n0: Exit");
            input = Console.ReadLine();
            if (input == "1")
            {
                Console.WriteLine("\nIs this a repeating task?(y/n)");
                input = Console.ReadLine().ToLower();

                if (input == "y")
                {
                    mainUser.userTaskList.addRepeatingTask();
                }
                else
                {
                    Console.WriteLine("\nIs this a multiday task?(y/n)");
                    input = Console.ReadLine().ToLower();

                    if (input == "y")
                    {
                        mainUser.userTaskList.addMultidayTask();
                    }
                    else
                    {
                        mainUser.addTask();
                    }
                }

                Console.WriteLine("Press Enter to continue.");
            }
            else if (input == "2")
                mainUser.removeTask();
            else if (input == "3")
            {
                Console.WriteLine("\nEnter the name of the task to edit");
                mainUser.editTask(Console.ReadLine());
            }
            else if (input == "4")
            {
                mainUser.createGroup();
            }
            else if (input == "5")
            {
                mainUser.deleteGroup();
            }
            else if (input == "6")
            {
                mainUser.printGroups();
            }
            else if (input == "7")
            {
                Console.WriteLine("\nEnter the name of the group to open");
                mainUser.userTaskList.openGroup(Console.ReadLine());
            }
            else if (input == "8")
            {
                mainUser.userTaskList.sortTasks();
            }
        }
    }
}
   


class User
{
    private string userName;
    public taskList userTaskList;

    public User(string name)
    {
        if (name.Length > 0)
            userName = name;
        else
            userName = "Default User";

        userTaskList = new taskList();
    }

    public void addTask()
    {
        userTaskList.addTask();
    }

    public void removeTask()
    {
        userTaskList.removeTask();
    }

    public void editTask(string name)
    {
        userTaskList.editTask(name);
    }

    public void createGroup()
    {
        userTaskList.createGroup();
    }

    public void deleteGroup()
    {
        userTaskList.deleteGroup();
    }

    public void printGroups()
    {
        userTaskList.printGroups();
    }

    public string getName()
    {
        return userName;
    }
    public void setName(string newName)
    {
        userName = newName;
    }
}


class taskList
{
    protected int taskCount;
    protected int groupCount;
    protected List<Task> tList = new List<Task>();
    protected List<taskGroup> gList = new List<taskGroup>();

    public taskList()
    {
        taskCount = 0;
        groupCount = 0;
    }

    public void addTask()
    {
        Task newTask = new Task();
        tList.Add(newTask);
        taskCount = tList.Count;
        Console.WriteLine("Task Added!\n");
        newTask.printTask();
        Console.WriteLine("Press enter to continue:");
        Console.ReadLine();
    }
    public void addRepeatingTask()
    {
        repeatingTask newRepeatingTask = new repeatingTask();
        tList.Add(newRepeatingTask);
        taskCount = tList.Count;
        Console.WriteLine("Task Added!\n");
        newRepeatingTask.printTask();
        Console.WriteLine("Press enter to continue:");
        Console.ReadLine();
    }

    public void addMultidayTask()
    {
        multidayTask newMultiTask = new multidayTask();
        tList.Add(newMultiTask);
        taskCount = tList.Count;
        Console.WriteLine("Task Added!\n");
        newMultiTask.printTask();
        Console.WriteLine("Press enter to continue:");
        Console.ReadLine();
    }

    public void removeTask()
    {
        Console.WriteLine("Enter the name of the task you would like to remove:");
        string tName = Console.ReadLine();
        tList.RemoveAll(t => t.getTaskName() == tName);
        taskCount = tList.Count;

        foreach (var group in gList)
        {
            group.removeFromGroup(tName);
        }
    }

    public void editTask(string name)
    {
        foreach (var task in tList)
        {
            if (task.getTaskName() == name)
            {
                Console.WriteLine("Enter new Task Name (Leave blank for no change):");
                string newName = Console.ReadLine();
                if (!string.IsNullOrEmpty(newName))
                {
                    task.setTaskName(newName);
                }

                Console.WriteLine("Enter new Task Description (Leave blank for no change):");
                string newDescription = Console.ReadLine();
                if (!string.IsNullOrEmpty(newDescription))
                {
                    task.setTaskDescription(newDescription);
                }

                Console.WriteLine("Enter new Task Priority (1-5) (Leave blank for no change):");
                string priorityInput = Console.ReadLine();
                if (int.TryParse(priorityInput, out int newPriority) && newPriority >= 1 && newPriority <= 5)
                {
                    task.setTaskPriority(newPriority);
                }

                Console.WriteLine("Enter new Task Due Date (yyyy-mm-dd) (Leave blank for no change):");
                string dueDateInput = Console.ReadLine();
                if (DateTime.TryParse(dueDateInput, out DateTime newDueDate))
                {
                    task.setTaskDueDate(newDueDate);
                }

                Console.WriteLine("Is the Task Completed? (true/false) (Leave blank for no change):");
                string completionInput = Console.ReadLine();
                if (bool.TryParse(completionInput, out bool newCompletion))
                {
                    task.setTaskCompletion(newCompletion);
                }

                Console.WriteLine("Enter the name of the group to add the task to (Leave blank for no change):");
                string input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    bool groupFound = false;
                    foreach (var group in gList)
                    {
                        if (group.getGroupName() == input)
                        {
                            group.addToGroup(task);
                            Console.WriteLine("Task added to group");
                            groupFound = true;
                            break;
                        }
                    }
                    if (!groupFound)
                    {
                        Console.WriteLine("No group with that name found");
                    }
                }

                break;
            }
        }
    }

    public void printTasks()
    {
        if (tList.Count == 0)
        {
            Console.WriteLine("No tasks to display.");
            return;
        }
        else
        {
            for (var i = 0; i < tList.Count; i++)
            {
                tList[i].printTask();
                Console.WriteLine("");
            }
        }
    }

    public void sortTasks()
    {
        Console.WriteLine("What would you like to sort by:");
        Console.WriteLine("1: Name\n2: Priority\n3: Due Date");

        string method = Console.ReadLine();
        switch (method)
        {
            case "1":
                tList = tList.OrderBy(t => t.getTaskName()).ToList();
                break;
            case "2":
                tList = tList.OrderBy(t => t.getTaskPriority()).ToList();
                break;
            case "3":
                tList = tList.OrderBy(t => t.getTaskDueDate()).ToList();
                break;
            default:
                Console.WriteLine("Invalid sorting criteria. Please choose 'name', 'priority', or 'duedate'.");
                break;
        }
    }

    public void createGroup()
    {
        taskGroup newGroup = new taskGroup();
        gList.Add(newGroup);
    }

    public void deleteGroup()
    {
        Console.WriteLine("Enter the name of the group you would like to remove:");
        string gName = Console.ReadLine();
        gList.RemoveAll(g => g.getGroupName() == gName);
        groupCount = gList.Count;
    }

    public void printGroups()
    {
        if (gList.Count == 0)
        {
            Console.WriteLine("No groups to display.");
        }
        else
        {
            foreach (var group in gList)
            {
                group.printGroup();
            }
        }
        Console.WriteLine("Press enter to continue:");
        Console.ReadLine();
    }

    public void openGroup(string name)
    {
        var group = gList.FirstOrDefault(g => g.getGroupName() == name);
        if (group != null)
        {
            group.printTasks();
        }
        else
        {
            Console.WriteLine("Group not found.");
        }
    }

    // Getters
    public int getTaskCount()
    {
        return taskCount;
    }
    public int getGroupCount()
    {
        return groupCount;
    }

    // Setters
    public void setTaskCount(int newTaskCount)
    {
        taskCount = newTaskCount;
    }
    public void setGroupCount(int newGroupCount)
    {
        groupCount = newGroupCount;
    }
}






class taskGroup
{
    private string groupName;
    private int taskCount;
    private List<Task> groupList = new List<Task>();

    public taskGroup()
    {
        Console.WriteLine("Enter Group Name:");
        groupName = Console.ReadLine();
        taskCount = 0;
    }

    public void addToGroup(Task newTask)
    {
        groupList.Add(newTask);
        taskCount = groupList.Count;
    }

    public void removeFromGroup(string name)
    {
        groupList.RemoveAll(t => t.getTaskName() == name);
        taskCount = groupList.Count;
    }

    public void printGroup()
    {
        Console.WriteLine($"Group Name: {groupName}");
        Console.WriteLine($"Task Count: {taskCount}");
    }

    public void printTasks()
    {
        if (groupList.Count == 0)
        {
            Console.WriteLine("No tasks to display.");
        }
        else
        {
            foreach (var task in groupList)
            {
                task.printTask();
            }
        }
        Console.WriteLine("Press enter to continue:");
        Console.ReadLine();
    }

    // Getters
    public string getGroupName()
    {
        return groupName;
    }
    public int getTaskCount()
    {
        return taskCount;
    }
    // Setters
    public void setGroupName(string newGroupName)
    {
        groupName = newGroupName;
    }
    public void setTaskCount(int newTaskCount)
    {
        taskCount = newTaskCount;
    }
}

class Task
{
    protected string taskName;
    protected string taskDescription;
    protected int taskPriority;
    protected bool taskCompleted;
    protected DateTime taskDueDate;

    public Task()
    {
        Console.Clear();
        Console.WriteLine("Enter Task Name:");
        taskName = Console.ReadLine();

        Console.WriteLine("Enter Task Description:");
        taskDescription = Console.ReadLine();

        while (true)
        {
            Console.WriteLine("Enter Task Priority (1-5):");
            if (int.TryParse(Console.ReadLine(), out taskPriority) && taskPriority >= 1 && taskPriority <= 5)
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 5.");
            }
        }

        while (true)
        {
            Console.WriteLine("Enter Task Due Date (yyyy-mm-dd):");
            if (DateTime.TryParse(Console.ReadLine(), out taskDueDate))
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid date in the format yyyy-mm-dd.");
            }
        }

        taskCompleted = false;
    }

    public virtual void printTask()
    {
        Console.WriteLine($"Task Name: {taskName}");
        Console.WriteLine($"Task Description: {taskDescription}");
        Console.WriteLine($"Task Priority: {taskPriority}");
        Console.WriteLine($"Task Due Date: {taskDueDate.ToShortDateString()}");
        Console.WriteLine($"Task Completed: {taskCompleted}");
    }

    // Setters
    public void setTaskName(string newName)
    {
        taskName = newName;
    }
    public void setTaskDescription(string newDescription)
    {
        taskDescription = newDescription;
    }
    public void setTaskPriority(int newPriority)
    {
        taskPriority = newPriority;
    }
    public void setTaskCompletion(bool newCompletion)
    {
        taskCompleted = newCompletion;
    }
    public void setTaskDueDate(DateTime newDueDate)
    {
        taskDueDate = newDueDate;
    }

    // Getters
    public string getTaskName()
    {
        return taskName;
    }
    public string getTaskDescription()
    {
        return taskDescription;
    }
    public int getTaskPriority()
    {
        return taskPriority;
    }
    public bool getTaskCompletion()
    {
        return taskCompleted;
    }
    public DateTime getTaskDueDate()
    {
        return taskDueDate;
    }
}

class repeatingTask : Task
{
    private bool[] repeatDays;
    private readonly string[] days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

    public repeatingTask()
    {
        repeatDays = new bool[7];
        Console.WriteLine("Enter the days you want the task to repeat on, separated by commas (e.g., Monday, Wednesday, Friday):");
        string input = Console.ReadLine().ToLower();
        var selectedDays = input.Split(", ").Select(day => day.Trim()).ToList();

        for (int i = 0; i < 7; i++)
        {
            repeatDays[i] = selectedDays.Contains(days[i].ToLower());
        }
    }

    public override void printTask()
    {
        base.printTask();
        Console.WriteLine("Repeats on: " + string.Join(", ", days.Where((day, index) => repeatDays[index])));
    }
}

class multidayTask : Task
{
    private int taskLength;

    public multidayTask()
    {
        Console.WriteLine("Enter the number of days the task will take:");
        taskLength = Convert.ToInt32(Console.ReadLine());
    }
    public override void printTask()
    {
        base.printTask();
        Console.WriteLine("Task Length: " + taskLength + " days");
    }
}




