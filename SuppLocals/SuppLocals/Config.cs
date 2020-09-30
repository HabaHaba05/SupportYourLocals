using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Security.Credentials;

namespace SuppLocals
{
    public class Config
    {
        public Config() { }


        //BING
        public static readonly CredentialsProvider BING_API_KEY = new ApplicationIdCredentialsProvider("vuOU7tN47KBhly1BAyhi~SKpEroFcVqMGYOJVSj-2HA~AhGXS-dV_H6Ofvn920LLMyvxfUUaLfjpZTD54fSc3WO-qRE7x6225O22AP_0XjDn");


        //GOOGLE
        public static readonly string GOOGLE_API_KEY = "AIzaSyBVs4wsiyCVvFbpPlX-NJyz_fj8db04R78";
        public static readonly string host = "https://maps.googleapis.com";
        public static readonly string path = "/maps/api/place/autocomplete/json";

    }
}
