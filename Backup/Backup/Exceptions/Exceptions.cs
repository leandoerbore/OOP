using System;

namespace Backup
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

    public class ExceptionFullPointDontFound : Exception
    {
        public ExceptionFullPointDontFound(string message) : base(message) {}
    }

    public class ExceptionBadMode : Exception
    {
        public ExceptionBadMode(string message) : base(message) {}
    }

    public class ExceptionNullList : Exception
    {
        public ExceptionNullList(string message) : base(message) {}
    }
    
}