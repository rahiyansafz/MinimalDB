﻿namespace MinimalDB;

public record Employee(int Id, string Name, string Workplace)
{
    public static List<string> Departments => ["HR", "Engineering", "Marketing"];
}

public record EmployeeV2(Guid Id, string Name, string Workplace);

public record EmployeeV3(string Id, string Name, string Workplace);

public record EmployeeV4(long Id, string Name, bool IsOld);

public class Test
{
    public static void Main()
    {
        JsonStore v2 = new("v2.json");
        try
        {
            //v2.CreateTable<EmployeeV3>();
            //v2.Insert<EmployeeV2>(new EmployeeV2 { Name = "Rahiyan", Workplace = "MPower" });
            //v2.Insert<EmployeeV2>(new EmployeeV2 { Id = Guid.Parse("b79856bc-a4a6-407e-952e-16ffe105ddb2"), Name = "Ifty", Workplace = "Binary Quest" });
            //v2.Insert<EmployeeV2>(new EmployeeV2 { Id = Guid.Parse("b79856bc-a4a6-407e-952e-16ffe105ddb2"), Name = "Ifty", Workplace = "Binary Quest" });
            //v2.Insert<EmployeeV2>(new EmployeeV2 { Name = "Ifty", Workplace = "Binary Quest" });

            //v2.CreateTable<Employee>();
            //v2.InsertOne<Employee>(new Employee { Id = 10, Name = "Rahiyan", Workplace = "MPower" });
            //v2.InsertMultiple<Employee>(new List<Employee> { new Employee { Name = "Rahiyan", Workplace = "MPower" }, new Employee { Id = 10, Name = "Ifty", Workplace = "Binary Quest" }, new Employee { Name = "Mintu", Workplace = "IQVIA" }, new Employee { Name = "Rishad", Workplace = "Doodles" }, new Employee { Name = "Mubasshir", Workplace = "MPower" } });
            v2.UpdateByCondition(x => true, new Employee(default, "Replacement", "Google"));
            //v2.DeleteByCondition<Employee>(x => x.Id == 10);

            //v2.CreateTable<EmployeeV2>();
            //v2.InsertOne<EmployeeV2>(new EmployeeV2 { Id = Guid.Parse("b79856bc-a4a6-407e-952e-16ffe105ddb2"), Name = "Rahiyan", Workplace = "MPower" });
            //v2.InsertMultiple<EmployeeV2>(new List<EmployeeV2> { new EmployeeV2 { Id = Guid.Parse("b79856bc-a4a6-407e-952e-16ffe105ddb3"), Name = "Ifty", Workplace = "Binary Quest" }, new EmployeeV2 { Name = "Rahiyan", Workplace = "MPower" } });
            //v2.UpdateByCondition<EmployeeV2>(x => x.Id == Guid.Parse("b79856bc-a4a6-407e-952e-16ffe105ddb2"), new EmployeeV2 { Name = "Replacement", Workplace = "Google" });
            //v2.DeleteOne<EmployeeV2>(Guid.Parse("b79856bc-a4a6-407e-952e-16ffe105ddb2"));

            //v2.CreateTable<EmployeeV4>();
            //v2.InsertOne<EmployeeV4>(new EmployeeV4 { Id = 3, Name = "Rahiyan", IsOld = false });
            //v2.InsertMultiple<EmployeeV3>(new List<EmployeeV3> { new EmployeeV3 { Id = "b79856bc-a4a6-407e-952e-16ffe105ddb3", Name = "Ifty", Workplace = "Binary Quest" }, new EmployeeV3 { Name = "Rahiyan", Workplace = "MPower" } });
            //v2.UpdateByCondition<EmployeeV4>(x => x.Id == 0, new EmployeeV4 { Name = "Replacement" });
            //v2.DeleteOne<EmployeeV4>(3);

            //v2.InsertOne<EmployeeV3>(new EmployeeV3 { Id = "f58efbc7-b546-4d7f-9e80-d7bee606ef12", Name = "Rishad", Workplace = "Doodles" });
            //v2.InsertOne<Employee>(new Employee { Name = "Mubasshir", Workplace = "MPower" });

            //v2.InsertMultiple<Employee>(new List<Employee> { new Employee { Name = "Rahiyan", Workplace = "MPower" }, new Employee { Id = 10, Name = "Ifty", Workplace = "Binary Quest" }, new Employee { Name = "Mintu", Workplace = "IQVIA" }, new Employee { Name = "Rishad", Workplace = "Doodles" }, new Employee { Name = "Mubasshir", Workplace = "MPower" } });
            //v2.Commit();

            //v2.DeleteOne<Employee>(4);

            //v2.DeleteByCondition<EmployeeV4>(x => true);

            v2.Commit();

            //v2.DeleteOne<Employee>(3);
            //v2.Commit();
            foreach (var item in v2.FindAll<Employee>())
            {
                Console.WriteLine($"{item.Id} {item.Name}");
            }

            Console.WriteLine(string.Join(',', v2.FindByCondition<Employee>(x => x.Id == 10 || x.Id == 11)));
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message.ToString());
            Console.ReadLine();
        }
    }
}
