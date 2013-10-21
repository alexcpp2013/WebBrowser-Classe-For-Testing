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
                var th = new Thread(obj => GetAllAttributesFromSite());
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
    }
}
