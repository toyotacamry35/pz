using System;
using System.Collections;

namespace ResourcesSystem.Loader
{
    public class ResourceLoadingException : Exception
    {
        private readonly Exception _innerException;
        
        public ResourceLoadingException(Exception innerException, string file, string path = null, int lineNo = 0, int linePos = 0)
        {
            _innerException = innerException;
            File = file;
            LineNo = lineNo;
            LinePos = linePos;
            Path = path;
        }

        public ResourceLoadingException(string message, string file, string path = null, int lineNo = 0, int linePos = 0) : base(message)
        {
            File = file;
            LineNo = lineNo;
            LinePos = linePos;
            Path = path;
        }

        public int LineNo { get; private set; }

        public int LinePos { get; private set; }

        public string Path { get; private set; }

        public string File { get; private set; }

        public override string Message => GameResources.FormatMessage(_innerException?.Message ?? base.Message, File, Path, LineNo, LinePos);

        public override string StackTrace => _innerException?.StackTrace ?? base.StackTrace;

        public override string HelpLink => _innerException?.HelpLink ?? base.HelpLink;

        public override IDictionary Data => _innerException?.Data ?? base.Data;

        public override string Source => _innerException?.Source ?? base.Source;
    }
}