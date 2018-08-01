using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineWritingProcess.CmdReadWrite.Cmd
{
    class CmdRead:Cmd
    {
                private string cmd;
        private string idKeySubstr;

        public string Cmd
        {
            get
            {
                return cmd;
            }

            set
            {
                cmd = value;
            }
        }

        public string IdKeySubstr
        {
            get
            {
                return idKeySubstr;
            }

            set
            {
                idKeySubstr = value;
            }
        }

        public enum ReadIdType
        {
            SnRead,
            ImeiRead,
            EidRead,
            IccidRead,
            VersonRead,
        }



        public CmdRead(ReadIdType type, string cmd, string idKeySubstr)
        {
            this.type = type.ToString();
            this.cmd = cmd;
            this.idKeySubstr = idKeySubstr;

            okStr = "OK";
            errorStr = "ERROR";
        }

    }
}
