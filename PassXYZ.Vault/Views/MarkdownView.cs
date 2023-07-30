namespace PassXYZ.Vault.Views;

public class MarkdownView : HybridWebView.HybridWebView
{
	public MarkdownView()
	{
        HybridAssetRoot = "hybrid_root";
        MainFile = "hybrid_app.html";
    }
}