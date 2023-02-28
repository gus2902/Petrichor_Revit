using System;
using System.Threading;

namespace Petrichor_Revit
{
    public enum RequestId : int
    {

        None = 0,

        Test = 1,

        TypeCreate = 2,

        ShowName = 3,

    }

    public class Request
    {
        private int m_request = (int)RequestId.None;


        public RequestId Take()
        {
            return (RequestId)Interlocked.Exchange(ref m_request, (int)RequestId.None);
        }


        public void Make(RequestId request)
        {
            Interlocked.Exchange(ref m_request, (int)request);
        }
    }
}
