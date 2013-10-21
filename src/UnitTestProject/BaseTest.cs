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
        protected string error = "";

        protected List<Tuple<string, string>> Attributes = new List<Tuple<string, string>>();

        protected List<string> Sites = new List<string>()
            {
                "http://kpi.ua/"
            };

        protected List<string> WebAttributes = new List<string>()
            {
                "abstract",
                "author",
                "content-language",
                "content-style-type",
                "content-type",
                "copyright",
                "description",
                "designer",
                "document-state",
                "expires",
                "generator",
                "google",
                "google-site-verification",
                "imagetoolbar",
                "keywords",
                "language",
                "msnbot",
                "mssmarttagspreventparsing",
                "pics-label",
                "pragma",
                "publisher",
                "rating",
                "refresh",
                "reply-to",
                "resource-type",
                "revisit",
                "revisit-after",
                "robots",
                "set-cookie",
                "subject",
                "title",
                "url",
                "vw96.objecttype",
                "window-target"
            };

        protected WebBrowser Web = null;

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

        protected bool Navigate(String address)
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

        protected void GetAttributes()
        {
            Attributes.Clear();

            var elems = Web.Document.GetElementsByTagName("META");
            try
            {
                if (elems != null)
                {
                    foreach (HtmlElement elem in elems)
                    {
                        String nameStr = elem.GetAttribute("NAME");
                        if (nameStr != null && nameStr.Length != 0)
                        {
                            String contentStr = elem.GetAttribute("CONTENT");
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

        private void SetWebBrowserOptions()
        {
            Web.ScriptErrorsSuppressed = true;
            Web.Visible = false;
        }

        protected void ClearWebBrowser()
        {
            if (Web != null)
                Web.Dispose();
            Web = null;
        }

        protected void LoadSite(string url)
        {
            if (Navigate(url) != true)
                Assert.Fail("Не корректный url.");

            while (Web.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
        }

        protected bool FindAttribute(string tag)
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

        protected void GetAllAttributesFromSite()
        {
            error = "";

            try
            {
                ClearWebBrowser();
                using (Web = new WebBrowser())
                {
                    SetWebBrowserOptions();

                    foreach (var el in Sites)
                    {
                        LoadSite(el);
                        GetAttributes();
                        foreach (var tag in WebAttributes)
                        {
                            if (!FindAttribute(tag))
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
    }
}
