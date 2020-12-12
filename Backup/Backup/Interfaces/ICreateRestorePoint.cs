using System.Collections.Generic;

namespace Backup
{
    public interface ICreateRestorePoint
    {
        public void CreateRestorePointSeparately(string path, FileBackup backup);
        public void CreateRestorePointZip(string path,FileBackup backup);

        public long Sizing(List<string> files);
    }
}