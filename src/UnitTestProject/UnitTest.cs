using System;
using System.Threading;
using NUnit.Framework;
using System.Windows.Forms;

namespace UnitTestProject
{
    [TestFixture, Category("Regression Tests")]
    public sealed class UnitTest : BaseTest
    {
        [STAThread]
        [Test]
        public void VerifySiteAttributes()
        {            
            try
            {
                var th = new Thread(obj => GetAllSiteMetaTags("META", "NAME", "CONTENT"));
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
            try
            {
                var th = new Thread(obj => VerifySiteTitle("Предупреждение о вредоносном ПО"));
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
