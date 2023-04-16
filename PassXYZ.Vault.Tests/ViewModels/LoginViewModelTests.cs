using Microsoft.Maui.Controls;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Core.UnitTests;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Tests.ViewModels
{
    public class LoginViewModelTests : ShellTestBase
    {
        Microsoft.Maui.Controls.Application app;

        public LoginViewModelTests() 
        {
            TestShell shell = new TestShell();
            var abougPage = new ShellItem { Route = "AboutPage" };
            var page = MakeSimpleShellSection("Readme", "content");
            abougPage.Items.Add(page);
            shell.Items.Add(abougPage);
            app = Substitute.For<Microsoft.Maui.Controls.Application>();
            app.MainPage = shell;
        }

        [Fact]
        public void LoginCommandTest() 
        {
            LoginViewModel vm = new();
            vm.LoginCommand.Execute(null);
        }
    }
}
