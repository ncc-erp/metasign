using System;
using System.Collections.Generic;


namespace EC.WebService.Mezon.Dto
{
    public class AuthOauth2Mezon 
    {
        public List<string> aud {  get; set; }
        public long auth_time { get; set; }
        public long iat {  get; set; }
        public string iss { get; set; }
        public long rat { get; set; }
        public string sub { get; set; }

    }
}