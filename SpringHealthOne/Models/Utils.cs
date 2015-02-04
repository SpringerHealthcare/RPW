using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using SpringHealthOne.Services;
using SpringHealthOne.SpringWS;

namespace SpringHealthOne.Models
{
    public class Utils
    {
        public bool UserAdmin(string username)
        {
            SmartService smartService = new SmartService();
            string key, toHash, token;
            token = smartService.AuthToken();
            if (token == "")
            {

            }
            else
            {
                toHash = username.ToUpper() + "|" + token;
                key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;

                AccountType Account = smartService.GetAccountDetails(key, username);
                if (Account.contentAdmin)
                {
                    return true;
                }
            }
            return false;
        }
    }
}