using System.Text.Json;

namespace WebSale.Services
{
    public class FileReader
    {
        public static List<T> ReadJsonFiles<T>(string folderPath)
        {
            var data = new List<T>();
            var files = Directory.GetFiles(folderPath, "*.json");

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var items = JsonSerializer.Deserialize<Dictionary<string, T>>(json);
                if (items != null)
                {
                    data.AddRange(items.Values);
                }
            }

            return data;
        }
    }
}
