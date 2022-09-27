using PassXYZ.BlazorUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassXYZ.Vault.Tests
{
    [Collection("Serilog collection")]
    public class ModalDialogTests : TestContext
    {
        SerilogFixture serilogFixture;

        public ModalDialogTests(SerilogFixture serilogFixture)
        {
            this.serilogFixture = serilogFixture;
        }   

        [Fact]
        public void ModalDialogInitTest() 
        {
            string title = "ModalDialog Test";
            var cut = RenderComponent<ModalDialog>(
                parameters => parameters.Add(p => p.Title, title)
                .Add(p => p.CloseButtonText, "Close")
                .Add(p => p.SaveButtonText, "Save"));
            cut.Find("h5").TextContent.MarkupMatches(title);
            serilogFixture.Logger.Debug("ModalDialogInitTest: done");
        }

        [Fact]
        public void ConfirmDialogInitTest() 
        {
            string title = "Deleting";
            var cut = RenderComponent<ConfirmDialog>();
            ConfirmDialog dialog = cut.Instance;
            cut.Find("h5").TextContent.MarkupMatches(title);
            serilogFixture.Logger.Debug("ConfirmDialogInitTest: done");
        }
    }
}
