using Dalfsen.Core.Contracts.Services;

namespace Dalfsen.Core.Services
{
    public class Exporter : IExporter
    {
        private readonly ISet<string> paths;

        public Exporter()
        {
            paths = new HashSet<string>();
        }

        public void Add(FileInfo path)
        {
            paths.Add(path.FullName!);
        }

        public bool Contains(FileInfo path)
        {
            return paths.Contains(path.FullName!);
        }

        public void Remove(FileInfo path)
        {
            paths.Remove(path.FullName!);
        }
    }
}
