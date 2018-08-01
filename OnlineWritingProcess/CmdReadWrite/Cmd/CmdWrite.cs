using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineWritingProcess.CmdReadWrite.Cmd
{
    class CmdWrite:Cmd
    {
                private string cmdPrefix;
        private string cmdSuffix;

        public string CmdPrefix
        {
            get
            {
                return cmdPrefix;
            }

            set
            {
                cmdPrefix = value;
            }
        }

        public string CmdSuffix
        {
            get
            {
                return cmdSuffix;
            }

            set
            {
                cmdSuffix = value;
            }
        }

        public enum WriteIdType
        {
            SnWrite,
            ImeiWrite,
        }

        public CmdWrite(WriteIdType type, string cmdPrefix, string cmdSuffix)
        {
            this.type = type.ToString();
            this.cmdPrefix = cmdPrefix;
            this.cmdSuffix = cmdSuffix;

            okStr = "OK";
            errorStr = "ERROR";
        }

    }
}
