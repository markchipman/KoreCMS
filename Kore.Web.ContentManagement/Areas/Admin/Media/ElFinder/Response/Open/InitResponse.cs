﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using ElFinder.DTO;

namespace ElFinder.Response
{
    [DataContract]
    internal class InitResponse : OpenResponseBase
    {
        private static string[] _empty = new string[0];

        public InitResponse(DTOBase currentWorkingDirectory)
            : base(currentWorkingDirectory)
        {
            Options = new Options();
        }

        public InitResponse(DTOBase currentWorkingDirectory, Options options)
            : base(currentWorkingDirectory)
        {
            Options = options;
        }

        [DataMember(Name = "api")]
        public string Api { get { return "2.0"; } }

        [DataMember(Name = "netDrivers")]
        public IEnumerable<string> NetDrivers { get { return _empty; } }

        [DataMember(Name = "uplMaxSize")]
        public string UploadMaxSize { get; set; }
    }
}