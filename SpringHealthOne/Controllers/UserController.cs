using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.ServiceModel;
using Microsoft.Web.Mvc.Controls;
using SpringHealthOne.SpringWS;
using SpringHealthOne.Models;
using System.Security.Cryptography;
using SpringHealthOne.Helpers;
using Recaptcha;
using System.Web.Services.Protocols;
using SpringHealthOne.Services;

// This controller accesses the web service to authenticate site users
namespace SpringHealthOne.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user, string search, string roid)
        {

            //if (User.Identity.IsAuthenticated)
            //{

            //}

            if (ModelState.IsValid)
            {
                // curently uses the username / password supplied for testing
                /*user.Password = "Lerxst1234"; // remove line when live
                user.UserName = "Alex.Lifeson"; // remove line when live*/

                // Call to webservice
                SMARTService smartService = new SMARTService();
                TempData["Message"] = ""; //set it to an empy string.
                string password = MD5Helper.GetMd5Hash(MD5.Create(), user.Password);
                try
                {
                    TempData["Message"] = smartService.Login(user.UserName, password);
                }
                catch (Exception ex) // apparently not throwing the expected fault exception. handling argument exception instead.
                {
                    if (ex is FaultException)
                    {
                        TempData["Error"] = "Error Message: " + ex.Message;
                        //TempData["ErrorDetail"] = "Error Detail: " + ex.Detail.errorDetails;
                    }
                    if (ex is ArgumentException)
                    {
                        TempData["Error"] = "Error Message: " + ex.Message;
                    }
                }

                // it woulld be better if the web service returned true / false 
                // not a string message which could be changed and break the login!
                if (TempData["Message"].ToString() == "Login Successful.")
                {
                    TempData["Message"] = "";
                    FormsAuthentication.SetAuthCookie(user.UserName, user.RememberMe);

                    if (search != "" && search != null)
                    {
                        return RedirectToAction("BasicSearch", "Home", new Search { freetext = search });
                    }

                    if (roid != null && roid != "")
                    {
                        //errrr i can't redirect to a post method. needs a rebuild?
                        //redirect to post page

                        if (Request.UrlReferrer.Query.Contains("addToFav=True"))
                        {
                            return RedirectToAction("AddToFavourites", "User", new { roid = roid });
                        }

                        return RedirectToAction("requestQuoteviaURL", "Home", new { roid = roid });

                    }

                    return RedirectToAction("Index", "Home"); // If we got this far, and we where successful, return home.
                }
                else
                {
                    ModelState.AddModelError("", "Login data is incorrect!");
                }
            }
            return View(user);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult MyAccount()
        {
            SmartService smartService = new SmartService();
            AccountType Account = null;
            MyAccount MyAccount = new MyAccount();
            if (Request.IsAuthenticated)
            {
                string key, toHash, token;
                token = smartService.AuthToken();
                if (token == "")
                {

                }
                else
                {
                    string username = User.Identity.Name;
                    toHash = username.ToUpper() + "|" + token;
                    key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;

                    Account = smartService.GetAccountDetails(key, username);
                }

                MyAccount.firstname = Account.firstname;
                MyAccount.lastname = Account.surname;
                MyAccount.email = Account.email;
                MyAccount.organization = Account.organisation;
                MyAccount.jobtitle = Account.jobTitle;
                MyAccount.address = Account.address;
                MyAccount.number = Account.telephone;
                MyAccount.password = Account.password;
                MyAccount.DropDownItems = MyAccount.getCountry(Account.country);
                MyAccount.sales = Account.emailService;

                for (var i = 0; i < Account.areasOfInterest.Count(); i++)
                {
                    MyAccount.data.Add(new UserTermData() { term = Account.areasOfInterest[i].term, remove = false, qualifier = Account.areasOfInterest[i].qualifier });
                }
            }

            return View(MyAccount);
        }

        [HttpPost]
        public ActionResult MyAccount(MyAccount model)
        {


            if (ModelState.IsValid)
            {
                SmartService smartService = new SmartService();
                AccountType Account = new AccountType();


                string key, toHash, token;
                token = smartService.AuthToken();
                if (token == "")
                {

                }
                else
                {
                    // Create a validation key based on the authentication token 
                    //toHash = token;
                    //
                    string username = User.Identity.Name;
                    toHash = username.ToUpper() + "|" + token;
                    key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;

                    //load account so we can preserve the password without the form on the front end.
                    Account = smartService.GetAccountDetails(key, username);

                    Account.username = username; //usernames are emails addresses... | might need changing
                    Account.password = Account.password; //to preserve the password
                    Account.firstname = model.firstname;
                    Account.surname = model.lastname;
                    Account.email = model.email;
                    Account.jobTitle = model.jobtitle;
                    Account.organisation = model.organization;
                    Account.telephone = model.number;
                    //Account.emailService = model.aoitext.ToString();
                    Account.country = model.country;
                    Account.address = model.address;
                    Account.emailService = model.sales;

                    List<TermType> Termlist = new List<TermType>();
                    foreach (UserTermData data in model.data)
                    {
                        if (data.remove == false)
                        {
                            TermType Term = new TermType();
                            Term.term = data.term;
                            Term.qualifier = data.qualifier;
                            Termlist.Add(Term);
                        }
                    }

                    if (model.addterm != null && model.addterm.Trim() != "" )
                    {
                        //This needs to be made a lot better. added for now so we can test
                        List<string> drug = new List<string>(smartService.Drugs(model.addterm, 1));
                        List<string> man = new List<string>(smartService.Manufacturers(model.addterm, 1));
                        List<string> indi = new List<string>(smartService.Indications(model.addterm, 1));
                        List<string> dev = new List<string>(smartService.Devices(model.addterm, 1));

                        TermType Term = new TermType();
                        Term.term = model.addterm;

                        Term.qualifier = "";
                        Term.qualifier = (drug.Count() > 0) ? "Drug" : "";
                        Term.qualifier = (man.Count() > 0) ? "Manufacturer" : Term.qualifier;
                        Term.qualifier = (indi.Count() > 0) ? "Indication" : Term.qualifier;
                        Term.qualifier = (dev.Count() > 0) ? "Device" : Term.qualifier;

                        if (model.linkedterm != null && model.linkedterm.Trim() != "")
                        {
                            Term.term = String.Format("{0} AND {1}",Term.term, model.linkedterm);
                            List<string> drug1 = new List<string>(smartService.Drugs(model.linkedterm, 1));
                            List<string> man1 = new List<string>(smartService.Manufacturers(model.linkedterm, 1));
                            List<string> indi1 = new List<string>(smartService.Indications(model.linkedterm, 1));
                            List<string> dev1 = new List<string>(smartService.Devices(model.linkedterm, 1));

                            if (drug1.Count() > 0)
                            {
                                Term.qualifier = String.Format("{0} AND {1}", Term.qualifier, "Drug");
                            }
                            else if (man1.Count() > 0)
                            {
                                Term.qualifier = String.Format("{0} AND {1}", Term.qualifier, "Manufacturer");
                            }
                            else if (indi1.Count() > 0)
                            {
                                Term.qualifier = String.Format("{0} AND {1}", Term.qualifier, "Indication");
                            }
                            else if (dev1.Count() > 0)
                            {
                                Term.qualifier = String.Format("{0} AND {1}", Term.qualifier, "Device");
                            }
                        }

                        Termlist.Add(Term);
                    }

                    Account.areasOfInterest = Termlist.ToArray();

                    try
                    {
                        Account.emailServiceSpecified = true;
                        TempData["Message"] = smartService.UpdateAccountDetails(key, Account);
                    }
                    catch (Exception ex) // apparently not throwing the expected fault exception. handling argument exception instead.
                    {
                        if (ex is FaultException)
                        {
                            TempData["Error"] = "Error Message: " + ex.Message;
                            //TempData["ErrorDetail"] = "Error Detail: " + ex.Detail.errorDetails;
                        }
                        if (ex is ArgumentException)
                        {
                            TempData["Error"] = "Error Message: " + ex.Message;
                        }
                    }
                }
                return RedirectToAction("MyAccount", "User"); //force it to reload the get request.
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (Request.IsAuthenticated)
            {
                return View("/Home/Index");
            }
            RegistrationUserModel model = new RegistrationUserModel();
            model.DropDownItems = model.getCountry();
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Register(RegistrationUserModel model)
        {
            //time for some inefficiant error checking - needs to be made better
            SmartService smartService = new SmartService();
            ViewBag.DisplayItems = true;

            if (model.password != model.confirm)
            {
                ModelState.AddModelError("confirm", "Your passwords don't match. Please retype your passwords and try again");
            }
            if (!model.tandc)
            {
                ModelState.AddModelError("tandc", "You must tick the Terms & Conditions checkbox to register.");
            }

            if (ModelState.IsValid)
            {
                //build account object and pass to insert registration.

                var sales = Request.Form["sales"];
                bool emailservice = (sales == null) ? false : true;

                AccountType at = new AccountType();
                string hash = smartService.GetMd5Hash(MD5.Create(), model.password);
                at.username = model.email;
                at.password = hash;
                at.firstname = model.firstname;
                at.surname = model.lastname;
                at.email = model.email;
                at.jobTitle = model.jobtitle;
                at.organisation = model.organization;
                at.telephone = model.number;
                at.emailService = model.sales;
                at.country = model.country;

                at.address = model.address;
                at.emailServiceSpecified = true;

                at.emailService = emailservice;

                TermType tt0 = new TermType();
                at.areasOfInterest = new TermType[] { tt0 };

                string key, toHash, token;
                token = smartService.AuthToken();
                if (token == "")
                {

                }
                else
                {
                    // Create a validation key based on the authentication token 
                    toHash = token;
                    key = smartService.GetMd5Hash(MD5.Create(), toHash);
                    try
                    {
                        TempData["Message"] = smartService.InsertAccount(key, at);
                        return Login(new User() { UserName = model.email, Password = model.password }, "", "");
                    }
                    catch (SoapException se)
                    {
                        var msg = se.Detail["ErrorMessage"]["errorMessage"].InnerText;
                        ModelState.AddModelError("Problem", msg);
                        if (msg.ToLower().Contains("account created pending verification"))
                        {
                            ViewBag.DisplayItems = false;
                        }
                    }
                    catch (Exception ex) // apparently not throwing the expected fault exception. handling argument exception instead.
                    {                   
                            TempData["Error"] = "Error Message: " + ex.Message;
                    }
                }
            }
            model.DropDownItems = model.getCountry();
            return View("Index", model);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldpassword, string newpass, string confirmpass)
        {
            if (Request.IsAuthenticated)
            {
                if (newpass == confirmpass)
                {
                    AccountType Account = new AccountType();
                    SmartService smartService = new SmartService();
                    string key, toHash, token;
                    token = smartService.AuthToken();
                    if (token == "")
                    {

                    }
                    else
                    {
                        string username = User.Identity.Name;
                        toHash = username.ToUpper() + "|" + token;
                        key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;

                        Account = smartService.GetAccountDetails(key, username);
                        string hash = smartService.GetMd5Hash(MD5.Create(), oldpassword);
                        if (Account.password == hash)
                        {
                            Account.password = smartService.GetMd5Hash(MD5.Create(), newpass);
                            string success = smartService.UpdateAccountDetails(key, Account);
                            TempData["Message"] = "Password successfully changed.";
                        }
                        else
                        {
                            ModelState.AddModelError("oldpassword", "The password given doesn't match your current password. Please retype your password and try again");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("confirmpass", "Your passwords don't match. Please retype your passwords and try again");
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult Favourites()
        {
            SearchResultsList ResultList = new SearchResultsList();
            try
            {
                if (!Request.IsAuthenticated)
                {
                    //must be logged in to see this page, so kick em out if they arnt.
                    TempData["Error"] = "You must be logged in to view your favourites";
                    return RedirectToAction("Index", "Home");
                }
                SmartService smartService = new SmartService();
                string key, toHash, token;
                token = smartService.AuthToken();
                if (token == "")
                {

                }
                else
                {
                    // Create a validation key based on the current logged in username and authentication token 
                    string username = User.Identity.Name;
                    toHash = username.ToUpper() + "|" + token;
                    key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;

                    AccountType Account = smartService.GetAccountDetails(key, username);
                    FullResultsType results = smartService.GetFavourites(key, Account.userId);

                    if (results.numResults == 0)
                    {

                    }
                    else
                    {

                        foreach (ArticleType at in results.contentTypeResults)
                        { //do this with a model instead
                            SearchResults SearchResults = new SearchResults();
                            SearchResults.articleId = at.articleId;
                            SearchResults.reprintOpprtunityId = at.reprintOpprtunityId;
                            SearchResults.articleTitle = at.articleTitle;
                            SearchResults.authors = at.authors;
                            SearchResults.journalTitle = at.journalTitle;
                            SearchResults.publicationDate = at.publicationDate;
                            SearchResults.citation = at.citation;
                            SearchResults.PDFlink = at.PDFlink;
                            SearchResults.abstractText = at.abstractText;
                            SearchResults.keySentence = at.keySentence;

                            ResultList.SearchResultObjList.Add(SearchResults);
                        }
                    }
                }
            }
            catch (Exception ex) // apparently not throwing the expected fault exception. handling argument exception instead.
            {
                if (ex is FaultException)
                {
                    TempData["Error"] = "Error Message: " + ex.Message;
                    //TempData["ErrorDetail"] = "Error Detail: " + ex.Detail.errorDetails;
                }
                if (ex is ArgumentException)
                {
                    TempData["Error"] = "Error Message: " + ex.Message;
                }
            }
            return View(ResultList);
        }

        [HttpGet]
        public ActionResult SavedSearch()
        {
            SmartService smartService = new SmartService();
            SavedSearchesType results = new SavedSearchesType();
            try
            {
                string key, toHash, token;
                token = smartService.AuthToken();
                if (token == "")
                {

                }
                else
                {
                    // Create a validation key based on the current logged in username and authentication token 
                    string username = User.Identity.Name;
                    toHash = username.ToUpper() + "|" + token;
                    key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;

                    AccountType Account = smartService.GetAccountDetails(key, username);
                    results = smartService.GetSavedSearches(key, Account.userId);

                    //loop through results, check search date and map
                    for (int arraykey = 0; arraykey < results.savedSearchResults.Length; ++arraykey)
                    {
                        DateTime dt = Convert.ToDateTime(results.savedSearchResults[arraykey].searchDate);
                        if (DateTime.Compare(dt, DateTime.Now.AddDays(-7)) < 0)
                        {
                            //if we are in here, then we need to do a new search, and delete the old entry.
                            //first thing to do, delete the old search.
                            try
                            {
                                smartService.DeleteSavedSearch(results.savedSearchResults[arraykey].searchId);
                            }
                            catch (Exception ex) // apparently not throwing the expected fault exception. handling argument exception instead.
                            {
                                TempData["Error"] = "";
                                if (ex is FaultException)
                                {
                                    TempData["Error"] = "Error Message: " + ex.Message;
                                }
                                if (ex is ArgumentException)
                                {
                                    TempData["Error"] = "Error Message: " + ex.Message;
                                }

                                if ((TempData["Error"].ToString()) == "")
                                {
                                    TempData["Error"] = ex.Message;
                                }
                                return View();
                            }

                            //now, do we do a new search based on the old paramiters..
                            try
                            {
                                System.DateTime fromdate = Convert.ToDateTime(results.savedSearchResults[arraykey].fromDate);
                                System.DateTime todate = Convert.ToDateTime(results.savedSearchResults[arraykey].toDate);
                                var userIdSpecified = false;
                                if (Account.userId != 0)
                                {
                                    userIdSpecified = true;
                                }
                                SummaryResultsType searchres = smartService.GetAdvancedSearchSummaryResults(Account.userId, results.savedSearchResults[arraykey].searchTerm, results.savedSearchResults[arraykey].drug, fromdate.Month.ToString(), fromdate.Year.ToString(),
                                    todate.Month.ToString(), todate.Year.ToString(), results.savedSearchResults[arraykey].productType, results.savedSearchResults[arraykey].journalTitle, results.savedSearchResults[arraykey].articleTitle, results.savedSearchResults[arraykey].device, results.savedSearchResults[arraykey].indication,
                                    results.savedSearchResults[arraykey].therapyArea, results.savedSearchResults[arraykey].manufacturer, results.savedSearchResults[arraykey].Author, results.savedSearchResults[arraykey].doi, results.savedSearchResults[arraykey].pubmedId, results.savedSearchResults[arraykey].citation, results.savedSearchResults[arraykey].sponsoredBy, "", "DESC");

                                smartService.InsertSavedSearch(results.savedSearchResults[arraykey].searchName, Account.userId, searchres.search_id);

                                results.savedSearchResults[arraykey].searchId = searchres.search_id;
                            }
                            catch (Exception ex) // apparently not throwing the expected fault exception. handling argument exception instead.
                            {
                                TempData["Error"] = "";
                                if (ex is FaultException)
                                {
                                    TempData["Error"] = "Error Message: " + ex.Message;
                                }
                                if (ex is ArgumentException)
                                {
                                    TempData["Error"] = "Error Message: " + ex.Message;
                                }

                                if ((TempData["Error"].ToString()) == "")
                                {
                                    TempData["Error"] = ex.Message;
                                }
                            }
                            //logic in here to search again and delete old entries.
                        }
                    }
                }
            }
            catch (Exception ex) // apparently not throwing the expected fault exception. handling argument exception instead.
            {
                if (ex is FaultException)
                {
                    TempData["Error"] = "Error Message: " + ex.Message;
                }
                if (ex is ArgumentException)
                {
                    TempData["Error"] = "Error Message: " + ex.Message;
                }
            }
            return View(results);
        }

        public ActionResult AddTermAuto(string term)
        {
            List<string> AccountTerms = new List<string>();
            try
            {
                SMARTService smartService = new SMARTService();

                List<string> recdrug = new List<string>(smartService.Drugs(term, 99999, true));
                //recdrug.Insert(0, "<div class='selectlistlabel'>Drugs</div>");
                if (recdrug.Count() > 0)
                {
                    AccountTerms.AddRange(recdrug);
                }

                List<string> indications = new List<string>(smartService.Indications(term, 99999, true));
                //indications.Insert(0, "<div class='selectlistlabel'>Indications</div>");
                if (indications.Count() > 0)
                {
                    AccountTerms.AddRange(indications);
                }

                List<string> manufact = new List<string>(smartService.Manufacturers(term, 99999, true));
                //manufact.Insert(0, "<div class='selectlistlabel'>Manufacturers</div>");
                if (manufact.Count() > 0)
                {
                    AccountTerms.AddRange(manufact);
                }

                List<string> devices = new List<string>(smartService.Devices(term, 99999, true));
                //manufact.Insert(0, "<div class='selectlistlabel'>Manufacturers</div>");
                if (devices.Count() > 0)
                {
                    AccountTerms.AddRange(devices);
                }
            }
            catch (Exception ex) // apparently not throwing the expected fault exception. handling argument exception instead.
            {
                if (ex is FaultException)
                {
                    TempData["Error"] = "Error Message: " + ex.Message;
                }
                if (ex is ArgumentException)
                {
                    TempData["Error"] = "Error Message: " + ex.Message;
                }
            }
            AccountTerms.Sort();
            return Json(AccountTerms.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeactivateAccount()
        {
            SmartService smartService = new SmartService();
            string key, toHash, token;
            token = smartService.AuthToken();
            string username = User.Identity.Name;
            if (token == "")
            {

            }
            else
            {
                // Create a validation key based on the current logged in username and authentication token 
                toHash = username.ToUpper() + "|" + token;
                key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;
                string deactivate = smartService.DeactivateAccount(key, username);
            }
        
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ForgottenPassword()
        {
            return PartialView("ForgottenPassword");
        }

        [HttpPost]
        public ActionResult ForgottenPassword(string email)
        {
            SMARTService smartService = new SMARTService();
            bool result = true;
            bool fail = true;
            try
            {
                smartService.ForgottenLogin(email, out result, out fail); // not sure this is right, docs not clear.
                if (result == true)
                {
                    TempData["Message"] = "New Password sent successfully.";
                }
                else
                {
                    TempData["Message"] = "Failed to send new password.";
                }

            }
            catch (Exception ex) // apparently not throwing the expected fault exception. handling argument exception instead.
            {
                if (ex is FaultException)
                {
                    TempData["Error"] = "Error Message: " + ex.Message;
                }
                if (ex is ArgumentException)
                {
                    TempData["Error"] = "Error Message: " + ex.Message;
                }
            }
            return View("Login");
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "removeFave")]
        public ActionResult removeFave(int[] removefavorite)
        {
            SmartService smartService = new SmartService();
            string key, toHash, token;
            token = smartService.AuthToken();
            if (token == "")
            {

            }
            else
            {
                // Create a validation key based on the current logged in username and authentication token 
                string username = User.Identity.Name;
                toHash = username.ToUpper() + "|" + token;
                key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;

                AccountType Account = smartService.GetAccountDetails(key, username);
                foreach (int fav in removefavorite)
                {
                    try
                    {
                        TempData["Message"] = smartService.DeleteFavourite(Account.userId, fav); //not working?
                    }
                    catch (Exception ex) // apparently not throwing the expected fault exception. handling argument exception instead.
                    {
                        if (ex is FaultException)
                        {
                            TempData["Error"] = "Error Message: " + ex.Message;
                        }
                        if (ex is ArgumentException)
                        {
                            TempData["Error"] = "Error Message: " + ex.Message;
                        }
                    }
                }
            }

            return RedirectToAction("Favourites");
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "requestQuote")]
        public ActionResult Favquote(string[] quote)
        {
            if (quote == null)
            {
                if (TempData["RequestedQuotes"] != null)
                {
                    quote = (string[])TempData["RequestedQuotes"];
                }
                else
                {
                    return View("Search");
                }
            }

            requestQuoteList QuoteList = new requestQuoteList();
            foreach (var article in quote)
            {
                List<string> idname = article.Split(',').ToList<string>();
                requestQuoteObj reqTempObj = new requestQuoteObj();
                reqTempObj.articleId = idname[0];
                reqTempObj.articleTitle = idname[1];
                QuoteList.requestQuotes.Add(reqTempObj);
            }
            return View("RequestQuote", QuoteList);
        }

        public ActionResult AddToFavourites(int roid)
        {

            if (Request.IsAuthenticated)
            {
                try
                {
                    SmartService smartService = new SmartService();
                    int[] favorite = new int[1]; //InsertFavourites expects an array so, give it one.
                    favorite[0] = roid;

                    string key, toHash, token;
                    token = smartService.AuthToken();
                    string username = User.Identity.Name;
                    toHash = username.ToUpper() + "|" + token;
                    key = smartService.GetMd5Hash(MD5.Create(), toHash) + "|" + username;

                    AccountType Account = smartService.GetAccountDetails(key, username);
                    TempData["Message"] = smartService.InsertFavourites(Account.userId, favorite);
                }
                catch (Exception ex)
                // apparently not throwing the expected fault exception. handling argument exception instead.
                {
                    if (ex is FaultException)
                    {
                        TempData["Error"] = "Error Message: " + ex.Message;
                    }
                    if (ex is ArgumentException)
                    {
                        TempData["Error"] = "Error Message: " + ex.Message;
                    }
                }
                return View();

            }


            return RedirectToAction("Login", "User", new { roid, addToFav = true });


        }
    }
}
