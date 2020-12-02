using System;
using System.Linq;
using System.Net;
using System.Reflection;
using ApiFrameWork;
using ApiFrameWork.Schema;
using ApiFrameWork.Utility;

namespace ApiFrameWork
{
    class Program
    {
        static void Main(string[] args)
        {
            // eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJzc28iLCJwcml2YXRlQ2xhaW0iOnsidXNlclJvbGUiOiJDVVNUT01FUiIsIm1vZGFsaXR5IjoiT1RQIiwic3RhdGUiOiJ0ZXN0IiwibWZhVHlwZSI6IkFVVEgiLCJjY3NJZCI6IjEyMyIsImNvdW50cnlDb2RlIjoiSEsiLCJidXNpbmVzc0NvZGUiOiJHQ0IiLCJjaGFubmVsSWQiOiJNQksiLCJ0aW1lU3RhbXAiOiIyMDE5LTEwLTEwVDEyOjAwOjAwIiwiand0IjoiZXlKaGJHY2lPaUpJVXpJMU5pSjkuZXlKemRXSWlPaUp6YzI4aUxDSndjbWwyWVhSbFEyeGhhVzBpT25zaWRYTmxjbEp2YkdVaU9pSkRWVk5VVDAxRlVpSXNJblZ6WlhKSlpDSTZJakV5TXpRMU5pSXNJbTF2WkdGc2FYUjVJam9pVDFSUUlpd2ljM1JoZEdVaU9pSjBaWE4wSWl3aWJXWmhWSGx3WlNJNklrRlZWRWdpTENKd1lYSjBibVZ5U1dRaU9pSkpiblpsYzNSRGJHOTFaQ0lzSW1OamMwbGtJam9pTVRJeklpd2lZM1Z6ZEc5dFpYSk9kVzFpWlhJaU9pSXdNREEwT0RZNE9EWWlMQ0pqYjNWdWRISjVRMjlrWlNJNklraExJaXdpWW5WemFXNWxjM05EYjJSbElqb2lSME5DSWl3aVkyaGhibTVsYkVsa0lqb2lUVUpMSWl3aVkyOXVjM1Z0WlhKUGNtZERiMlJsSWpvaVUxTkJTRXRIUTBkSlRsWkZVMVJEVEU5VlJDSXNJblJwYldWVGRHRnRjQ0k2SWpJd01Ua3RNVEF0TVRCVU1USTZNREE2TURBaUxDSmpiM0p5Wld4aGRHbHZia2xrSWpvaU1USXpORFVpZlN3aWJtSm1Jam94TlRrM056UXdNREUzTENKcGMzTWlPaUpqYVhScExtTnZiU0lzSW1WNGNDSTZNVFU1TnpjMU5UQXhOeXdpYVdGMElqb3hOVGszTnpRd01ERTNMQ0pxZEdraU9pSTROVEkwWTJVNU9DMHhNRFExTFRSak1qRXRZamN3TnkweVltTmhZelpsTlRJM09UZ2lmUS5YSjE5R0lLNTRGb1FtT2xKbGhuZHFDcC1vYkRhVkYtcUo1N21ZaEJ1UHRZIiwiY29ycmVsYXRpb25JZCI6IjEyMzQ1In0sIm5iZiI6MTU5Nzc0MDAxNywiaXNzIjoiY2l0aS5jb20iLCJleHAiOjE1OTc3NTUwMTcsImlhdCI6MTU5Nzc0MDAxNywianRpIjoiNTAyNjhiODgtZjQ5Ny00ZDJhLTkzMjgtZWFiOTg0M2EwZjAwIn0.LDpqGzn2QtXFPss0EkqjzNZjbRt730COMkCBg55_QaM
            new Launch().Config(t =>
                         {

                             t.LogFolderPath = "Result";

                         }).Start();

        }



    }


}
