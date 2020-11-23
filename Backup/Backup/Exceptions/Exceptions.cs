using System;

namespace Backup
{
    public class Exceptions
    {
        public class ExceptionInvalidIndexOfBackup : Exception
        {
            public ExceptionInvalidIndexOfBackup(string message) : base(message) {}
        }

        public class ExceptionDirectoryDoesNotExist : Exception
        {
            public ExceptionDirectoryDoesNotExist(string message) : base(message) {}
        }
        
        public class ExceptionFileDoesNotExist : Exception
        {
            public ExceptionFileDoesNotExist(string message) : base(message) {}
        }
        
        public class ExceptionFileExist : Exception
        {
            public ExceptionFileExist(string message) : base(message) {}
        }
        
        public class ExceptionDirectoryExist : Exception
        {
            public ExceptionDirectoryExist(string message) : base(message) {}
        }
    }
}