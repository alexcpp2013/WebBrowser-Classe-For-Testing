using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;

namespace UnitTestProject
{
    public abstract class BaseTest
    {
        protected enum TypeParameter { Good, Nothing, Error }

        //------------------------------------------------

        [TestFixtureSetUp]
        virtual public void ClassInitialize()
        {
            ;
        }

        [TestFixtureTearDown]
        virtual public void ClassClean()
        {
            ;
        }

        [SetUp]
        public void TestOpen()
        {
            TestCleanUp();
        }

        [TearDown]
        public void TestCleanUp()
        {
            ;
        }

        //------------------------------------------------

        protected bool Navigate(WebBrowser Web, String address)
        {
            if (String.IsNullOrEmpty(address)) return false;
            if (address.Equals("about:blank")) return false;
            if (!address.StartsWith("http://") &&
                !address.StartsWith("https://"))
            {
                address = "http://" + address;
            }

            try
            {
                Web.Navigate(new Uri(address));
                return true;
            }
            catch (System.UriFormatException err)
            {
                Assert.Fail("Ошибка в uri. \n" + err.Message);
                return false;
            }
            catch (Exception err)
            {
                Assert.Fail("Ошибка при переходе к документу. \n" + err.Message);
                return false;
            }
        }

        protected void GetAttributes(WebBrowser Web, List<Tuple<string, string>> Attributes, 
            string head = "META", string name = "NAME", string content = "CONTENT")
        {
            Attributes.Clear();

            var elems = Web.Document.GetElementsByTagName(head);
            try
            {
                if (elems != null)
                {
                    foreach (HtmlElement elem in elems)
                    {
                        String nameStr = elem.GetAttribute(name);
                        if (nameStr != null && nameStr.Length != 0)
                        {
                            String contentStr = elem.GetAttribute(content);
                            Attributes.Add(new Tuple<string, string>(nameStr, contentStr));
                        }
                    }
                }
                else
                    Assert.Fail("Не удалось считать страницу указанног осайта.");
            }
            catch (Exception err)
            {
                Assert.Fail("Ошибка во время париснга документа. \n" + err.Message);
            }
        }

        /*protected void StartNewBrowser(string url)
        {
            ;
        }*/

        private void SetWebBrowserOptions(WebBrowser Web)
        {
            Web.ScriptErrorsSuppressed = true;
            Web.Visible = false;
        }

        protected void ClearWebBrowser(WebBrowser Web)
        {
            if (Web != null)
                Web.Dispose();
            Web = null;
        }

        protected void LoadSite(WebBrowser Web, string url)
        {
            if (Navigate(Web, url) != true)
                Assert.Fail("Не корректный url.");

            while (Web.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
        }

        protected bool FindAttribute(List<Tuple<string, string>> Attributes, string tag)
        {
            bool flag = false;

            if (tag != "")
            {
                Parallel.ForEach(Attributes,
                    (curValue, loopstate) =>
                    {
                        if (tag.ToLower() == curValue.Item1.ToLower())
                        {
                            loopstate.Stop();
                            flag = true;
                            return;
                        }
                    });
            }

            return flag;
        }

        protected void GetAllSiteMetaTags(ref string error, WebBrowser Web, 
            List<Tuple<string, string>> Attributes, 
            string head = "META", string name = "NAME", string content = "CONTENT")
        {
            error = "";

            try
            {
                ClearWebBrowser(Web);
                using (Web = new WebBrowser())
                {
                    SetWebBrowserOptions(Web);

                    foreach (var el in SiteItems.Sites)
                    {
                        LoadSite(Web, el);
                        GetAttributes(Web, Attributes, head, name, content);

                        AddNewAttributesToList(Attributes);
                        foreach (var tag in SiteItems.WebAttributes)
                        {
                            if (!FindAttribute(Attributes, tag))
                            {
                                error += "\nСайт: " + el + "\t Тэг: " + tag;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Application.ExitThread();   // Stops the thread
            }
        }

        private void AddNewAttributesToList(List<Tuple<string, string>> Attributes)
        {
            foreach (var el in Attributes)
            {
                if (!SiteItems.WebAttributes.Contains(el.Item1.ToLower()))
                {
                    SiteItems.WebAttributes.Add(el.Item1.ToLower());
                }
            }
        }

        protected void VerifySiteTitle(ref string error, WebBrowser Web, string title)
        {
            error = "";

            try
            {
                ClearWebBrowser(Web);
                using (Web = new WebBrowser())
                {
                    SetWebBrowserOptions(Web);

                    foreach (var el in SiteItems.Sites)
                    {
                        string newaddress = el;
                        if (!el.StartsWith("http://") &&
                            !el.StartsWith("https://"))
                        {
                            newaddress = "http://" + el;
                        }
                        LoadSite(Web, "https://www.google.com.ua/interstitial?url=" + 
                                 newaddress);
                        
                        //or Like str OR Containe str
                        string tmp = GetDocumentTitle(Web);
                        var t = Web.Document;
                        if(tmp == title)
                            error += "\nСайт: " + el;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Application.ExitThread();   // Stops the thread
            }
        }

        protected string GetDocumentTitle(WebBrowser Web)
        {
            return Web.DocumentTitle;
        }
    }
}
