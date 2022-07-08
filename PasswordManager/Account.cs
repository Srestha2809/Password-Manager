/*
 * Program:         PasswordManager.exe
 * Module:          Account.cs
 * Date:            2022-05-31
 * Author:          Mitchell Hughes - Srestha Bharadwaj
 * Description:     A class that encapsulates both an Account and Password object.
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PasswordManager
{
    class Account
    {
        public string description { get; set; }
        public string userid { get; set; }
        public string loginurl { get; set; }
        public string accountnum { get; set; }
        public Password password { get; set; }

    } // end class Account

    class Password
    {
        public string value { get; set; }
        public int strengthnum { get; set; }
        public string strengthtext { get; set; }
        public string lastreset { get; set; }

    }// end class Password
}
