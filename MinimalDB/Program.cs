namespace MinimalDB
{
    public record Employee(int Id, string Name, string Workplace)
    {
        public static List<string> Strings => ["Omo", "mo", "o"];
    }

    public record EmployeeV2(Guid Id, string Name, string Workplace);

    public class Test
    {
        public static void Main(string[] args)
        {
            JsonStore v1 = new("database.json");

            v1.Insert(new Employee(default, "Raighne", "Lafarge"));
            v1.Insert(new Employee(24, "Onyeka", "Lafarge"));
            v1.Insert(new Employee(default, "Canice", "Lafarge"));

            v1.Commit();

            var bb = v1.FindAll<Employee>();
            foreach (var item in v1.FindAll<Employee>())
            {
                Console.WriteLine($"{item.Id} {item.Name} {item.Workplace}");
            }
            Console.ReadLine();

            // Get me the employee with Id = 2
            Console.WriteLine(v1.FindByCondition<Employee>(x => x.Name.Contains("cani", StringComparison.InvariantCultureIgnoreCase))?.FirstOrDefault()?.Name);
            //JsonStore v2 = new JsonStore("v2.json");
            //try
            //{
            //    //v2.Insert<EmployeeV2>(new EmployeeV2 { Id = Guid.NewGuid(), Name = "Onyeka", Workplace = "Google" });
            //    v2.Insert<EmployeeV2>(new EmployeeV2 { Name = "Raighne", Workplace = "Google" });
            //    v2.Insert<EmployeeV2>(new EmployeeV2 { Id = Guid.NewGuid(), Name = "Onyeka", Workplace = "Google" });

            //    v2.Commit();

            //    foreach (var item in v2.FindAll<EmployeeV2>())
            //    {
            //        Console.WriteLine($"{item.Id} {item.Name} {item.Workplace}");
            //    }

            //    Console.WriteLine(v2.FindByCondition<EmployeeV2>(x => x.Id == Guid.Parse("68b6705e-960d-40c4-9182-f430afa55e70"))
            //        .FirstOrDefault()
            //        .Name);
            //    Console.ReadLine();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message.ToString());
            //    Console.ReadLine();
            //}
        }
    }
}