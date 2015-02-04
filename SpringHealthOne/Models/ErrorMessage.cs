using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace SpringHealthOne.Models
{
    [DataContract]
    public class ErrorMessage
    {
        bool _success = true;
        string _errorMessage = "";
        string _errorDetails = "";

        /// <summary>
        ///     Flag indicating success or failure of service method request.
        /// </summary>
        [DataMember]
        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        /// <summary>
        ///     Exception error message as thrown by service method.
        /// </summary>
        [DataMember]
        public string errorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        /// <summary>
        ///     Exception detailed error message as thrown by service method.
        /// </summary>
        [DataMember]
        public string errorDetails
        {
            get { return _errorDetails; }
            set { _errorDetails = value; }
        }
    }
}