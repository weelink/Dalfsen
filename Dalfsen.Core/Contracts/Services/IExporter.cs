namespace Dalfsen.Core.Contracts.Services
{
    public interface IExporter
    {
        void Add(FileInfo path);
        void Remove(FileInfo path);
        bool Contains(FileInfo path);
    }
}
