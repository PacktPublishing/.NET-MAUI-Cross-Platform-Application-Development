﻿using PassXYZ.BlazorUI;
using PassXYZ.Vault.Tests.BlazorUI.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassXYZ.Vault.Tests.BlazorUI
{
    public class ModelDialogTests : TestContext
    {
        [Fact]
        public void ModalDialogInitTest() 
        {
            string title = "ModalDialog Test";
            var cut = RenderComponent<ModalDialog>(
                parameters => parameters.Add(p => p.Title, title)
                .Add(p => p.CloseButtonText, "Close")
                .Add(p => p.SaveButtonText, "Save"));
            cut.Find("h5").TextContent.MarkupMatches(title);
            Debug.WriteLine($"{cut.Markup}");
        }

        [Fact]
        public void ConfirmDialogInitTest() 
        {
            string title = "Deleting";
            var cut = RenderComponent<ConfirmDialog>();
            ConfirmDialog dialog = cut.Instance;
            cut.Find("h5").TextContent.MarkupMatches(title);
            Debug.WriteLine($"{cut.Markup}");
        }
    }
}
