using System.Diagnostics;
using System.Web;

namespace PassXYZ.Vault.Views;

public class MarkdownView : HybridWebView.HybridWebView
{
	public MarkdownView()
	{
        HybridAssetRoot = "hybrid_root";
        MainFile = "hybrid_app.html";
    }

    public void DisplayMarkdown(string markdown) 
    {
#if !ANDROID
        string markDownTxt = HttpUtility.JavaScriptStringEncode(markdown);
        Debug.WriteLine($"markDownTxt len={markDownTxt.Length}");
#else
        string markDownTxt = markdown;
        Debug.WriteLine($"Android: markDownTxt len={markDownTxt.Length}");
#endif
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            // Code to run on the main thread
            await InvokeJsMethodAsync("MarkdownToHtml", markDownTxt);
        });
    }
}