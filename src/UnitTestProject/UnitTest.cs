using System;
using System.Threading;
using NUnit.Framework;
using System.Windows.Forms;
using System.Collections.Generic;

namespace UnitTestProject
{
    [TestFixture, Category("Regression Tests")]
    public sealed class UnitTest : BaseTest
    {
        [STAThread]
        [Test]
        public void VerifySiteAttributes()
        {            
            string error = "";

            List<Tuple<string, string>> Attributes = new List<Tuple<string, string>>();

            WebBrowser Web = null;

            try
            {
                var th = new Thread(obj => GetAllSiteMetaTags(ref error, Web, Attributes, "META", "NAME", "CONTENT"));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
            }
            catch (Exception err)
            {
                Assert.Fail("Ошибка: \n", err.Message);
            }

            if (error != "")
                Assert.Fail("Тэги не найдены на следующих страницах:\n" + error);
        }

        [STAThread]
        [Test]
        public void VerifyIsGoogleBlocked()
        {
            string error = "";

            WebBrowser Web = null;

            try
            {
                var th = new Thread(obj => VerifySiteTitle(ref error, Web, "Предупреждение о вредоносном ПО"));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
            }
            catch (Exception err)
            {
                Assert.Fail("Ошибка: \n", err.Message);
            }

            if (error != "")
                Assert.Fail("Google заблокирвоал следующие сайты:\n" + error);
        }
    }
}
