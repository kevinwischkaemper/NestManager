using System;
using System.Collections.Generic;

namespace NestManagerLibrary
{
    public class RefreshEventArguments : EventArgs
    {
        public List<Nest> NestList { get; private set; }

        public RefreshEventArguments(List<Nest> nestlist)
        {
            NestList = nestlist;
        }
    }
}