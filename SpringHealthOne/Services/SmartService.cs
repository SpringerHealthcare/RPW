using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using SpringHealthOne.SpringWS;


namespace SpringHealthOne.Services
{
    public class SmartService
    {
        private SMARTService smartService;

        public SmartService()
    {
            this.smartService = new SMARTService();
        }

        public string GetKey(string username)
        {
            string key, toHash, token;
            token = smartService.AuthToken();
            toHash = username.ToUpper() + "|" + token;
            key = GetMd5Hash(MD5.Create(), toHash) + "|" + username;
            return key;
        }

        public string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public string AuthToken()
        {
            string token = smartService.AuthToken();
            return token;
        }

        public SummaryResultsType GetBasicSearchSummaryResults(int userid, string searchterm, string monthFromDate, string yearFromDate, string monthToDate, string yearToDate, string productType, string journalTitle, string sortBy, string sortOrder)
        {
            bool userIdSpecified = false;
            if (userid != 0)
            {
                userIdSpecified = true;
            }
            return smartService.GetBasicSearchSummaryResults(userid, userIdSpecified, searchterm, monthFromDate, yearFromDate, monthToDate, yearToDate, productType, journalTitle, sortBy, sortOrder);
        }

        public AccountType GetAccountDetails(string key, string username)
        {
            return smartService.GetAccountDetails(key, username);
        }

        public FullResultsType GetLoggedInSearchResults(string key, string searchId, string startRow, string endRow)
        {
            return smartService.GetLoggedInSearchResults(key, searchId, startRow, endRow);
        }

        public FullResultsType GetLoggedOutSearchResults(string key, string searchId, string numRows)
        {
            return smartService.GetLoggedOutSearchResults(key, searchId, numRows);
        }

        public SummaryResultsType GetAdvancedSearchSummaryResults(int userid, string searchTerm, string drug, string monthFromDate, string yearFromDate, string monthToDate, string yearToDate, string productType, string journalTitle, string articleTitle, string device, string indication,string therapyArea, string manufacturer, string author, string doi, string pubmedId, string citation, string sponsoredBy, string sortBy, string sortOrder   )
        {
            bool useridspec = false;
            if(userid!=0){
                useridspec = true;
            }
            return smartService.GetAdvancedSearchSummaryResults(userid, useridspec, searchTerm, drug, monthFromDate, yearFromDate, monthToDate, yearToDate, productType, journalTitle, articleTitle, device, indication, therapyArea, manufacturer, author, doi, pubmedId, citation, sponsoredBy, sortBy, sortOrder);
        }

        public SavedSearchType GetASavedSearch(string key, string searchId)
        {
            return smartService.GetASavedSearch(key, searchId);
        }

        public ArticleType GetArticleDetails(string key, string reprintOddId)
        {
            return smartService.GetArticleDetails(key, reprintOddId);
        }

        public string InsertQuoteRequest(int userId, QuoteRequestType[] requests)
        {
            bool idSpecified = false;
            if(userId!=0){
                idSpecified = true;
            }
            return smartService.InsertQuoteRequest(userId, idSpecified, requests);
        }

        public string InsertFavourites(int userId, int[] reprintOpps)
        {
            bool userIdSpecified = false;
            if(userId!=0){
                userIdSpecified = true;
            }
            return smartService.InsertFavourites(userId, userIdSpecified, reprintOpps);
        }

        public PubMedSummaryResultsType PubMedSearchSummary(string key, string searchId)
        {
            return smartService.PubMedSearchSummary(key, searchId);
        }

        public PubMedFullResultsType PubMedSearchResults(string key, PubMedSummaryResultsType results, string pagestart, string pageend){
            return smartService.PubMedSearchResults(key, results, pagestart, pageend);
        }

        public string  InsertSavedSearch(string searchName, int userId, string searchId)
        {
            bool userIdSpecified = false;
            if (userId != 0)
            {
                userIdSpecified = true;
            }
            return  smartService.InsertSavedSearch(searchName, userId, userIdSpecified, searchId);
        }

        public string[] Drugs(string filter, int numMatches){
            bool numSpecified = false;
            if(numMatches!=0){
                numSpecified = true;
            }
            return smartService.Drugs(filter, numMatches, numSpecified);
    }

        public string[] Journals(string filter, int numMatches)
        {
            bool numSpecified = false;
            if (numMatches != 0)
            {
                numSpecified = true;
            }
            return smartService.Journals(filter, numMatches, numSpecified);
        }

        public string[] Devices(string filter, int numMatches)
        {
            bool numSpecified = false;
            if (numMatches != 0)
            {
                numSpecified = true;
            }
            return smartService.Devices(filter, numMatches, numSpecified);
        }

        public string[] Indications(string filter, int numMatches)
        {
            bool numSpecified = false;
            if (numMatches != 0)
            {
                numSpecified = true;
            }
            return smartService.Indications(filter, numMatches, numSpecified);
        }

        public string[] TherapyAreas(string filter, int numMatches)
        {
            bool numSpecified = false;
            if (numMatches != 0)
            {
                numSpecified = true;
            }
            return smartService.TherapyAreas(filter, numMatches, numSpecified);
        }

        public string[] Manufacturers(string filter, int numMatches)
        {
            bool numSpecified = false;
            if (numMatches != 0)
            {
                numSpecified = true;
            }
            return smartService.Manufacturers(filter, numMatches, numSpecified);
        }

        public string ContactUs(ContactUsType contact)
        {
            return smartService.ContactUs(contact);
        }

        public string UpdateAccountDetails(string key, AccountType account)
        {
            return smartService.UpdateAccountDetails(key, account);
        }

        public string InsertAccount(string key, AccountType account){
            return smartService.InsertAccount(key, account);
        }

        public FullResultsType GetFavourites(string key, int userId)
        {
            bool IdSpecified = false;
            if(userId!=0){
                IdSpecified = true;
            }
            return smartService.GetFavourites(key,userId, IdSpecified);
        }

        public SavedSearchesType GetSavedSearches(string key, int userId)
        {
            bool IdSpecified = false;
            if (userId != 0)
            {
                IdSpecified = true;
            }
            return smartService.GetSavedSearches(key, userId, IdSpecified);
        }

        public string  DeleteSavedSearch(string searchId)
        {
            return smartService.DeleteSavedSearch(searchId);
        }

        public string DeleteFavourite(int userId, int oppIds)
        {
            bool IdSpecified = false;
            if (userId != 0)
            {
                IdSpecified = true;
            }
            bool OppIdSpecified = false;
            if (oppIds != 0)
            {
                OppIdSpecified = true;
            }
            return smartService.DeleteFavourite(userId, IdSpecified, oppIds, OppIdSpecified);
        }

        public string DeactivateAccount(string key, string username)
        {
            return smartService.DeactivateAccount(key, username);
        }
    }
}