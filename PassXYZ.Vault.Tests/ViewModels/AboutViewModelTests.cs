using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Tests.ViewModels
{
    public class AboutViewModelTests
    {
        [Fact]
        public void SetTitleTest()
        {
            string AboutTitle = "AboutTest";
            AboutViewModel viewModel = new();
            viewModel.Title = AboutTitle;
            Assert.Equal(AboutTitle, viewModel.Title);
        }
    }
}
