namespace MinimalDB;
public class FileAccess
{
    public static string GetJsonFromDb(string filename)
    {
        string? path = filename.Split('\\').Length > 1
            ? filename
            : $"{Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName}\\{filename}";
        if (!File.Exists(path))
        {
            var stream = File.Open(path, FileMode.Create);

            stream.Dispose();

            File.WriteAllText(path, "{}");
        }

        return File.ReadAllText(path);
    }

    public static void WriteJsonToDb(string filename, string json)
    {
        var path = $"{Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName}\\{filename}";

        if (!File.Exists(path))
        {
            var stream = File.Open(path, FileMode.Create);

            stream.Dispose();
        }

        File.WriteAllText(path, json);
    }
}