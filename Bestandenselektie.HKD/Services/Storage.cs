using System.Text.Json;
using System.IO;

namespace Bestandenselektie.HKD.Services
{
    public class Storage
    {
        private readonly string dataDirectory;

        public Storage()
        {
            dataDirectory = ApplicationDeployment.CurrentDeployment?.DataDirectory ?? Path.GetTempPath();
        }

        public Settings ReadSettings()
        {
            return Read<Settings>("settings.json");
        }

        public void Write(Settings settings)
        {
            Write("settings.json", settings);
        }

        private T Read<T>(string file) where T : class, new()
        {
            string path = Path.Combine(dataDirectory, file);

            if (!File.Exists(path))
            {
                Write(file, new T());
                return new T();
            }

            var contents = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(contents) ?? new T();
        }

        private void Write<T>(string file, T value) where T : class, new()
        {
            string path = Path.Combine(dataDirectory, file);
            string contents = JsonSerializer.Serialize(value);

            File.WriteAllText(path, contents);
        }
    }
}
