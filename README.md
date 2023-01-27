# .NET MAUI Cross-Platform Application Development	
.NET MAUI Cross-Platform Application Development, published by Packt

<a href="https://www.packtpub.com/product/.net-maui-cross-platform-application-development/9781800569225"><img src="https://static.packt-cdn.com/products/9781800569225/cover/smaller" alt=".NET MAUI Cross-Platform Application Development" height="256px" align="right"></a>

This is the code repository for [.NET MAUI Cross-Platform Application Development](https://www.packtpub.com/product/.net-maui-cross-platform-application-development/9781800569225), published by Packt.

**Leverage a first-class cross-platform UI framework to build native apps on multiple platforms	**

## What is this book about?

This book is an entry-level .NET MAUI book for mobile developers interested in cross-platform application development with working experience of the .NET Core framework, as well as fresh or junior engineers who’ve just begun their career in mobile app development. Native application developers (desktop) or Xamarin developers who want to migrate to .NET MAUI will also benefit from this book. Basic knowledge of modern object-oriented programming language, such as C#, Java or Kotlin, is assumed.	

This book covers the following exciting features:

* Discover the latest features of .NET 6 that can be used in mobile and desktop app development
* Find out how to build cross-platform apps with .NET MAUI and Blazor
* Implement device-specific features using .NET MAUI Essentials
* Integrate third-party libraries and add your own device-specific features
* Discover .NET class unit test using xUnit.net and Razor components unit test using bUnit
* Deploy apps in different app stores on mobile as well as desktop

If you feel this book is for you, get your [copy](https://www.amazon.com/dp/180056922X) today!

<a href="https://www.packtpub.com/?utm_source=github&utm_medium=banner&utm_campaign=GitHubBanner"><img src="https://raw.githubusercontent.com/PacktPublishing/GitHub/master/GitHub.png" 
alt="https://www.packtpub.com/" border="5" /></a>


## Instructions and Navigations
All of the project files are organized into folders. For example, Chapter03.

The code will look like the following:

```
private async Task<bool> UpdateItemAsync(string key, string value)
{
    if (listGroupItem == null) return false;
    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
      return false;
    listGroupItem.Name = key;
    listGroupItem.Notes = value;
   if (_isNewItem) {...}
   else {...}
      StateHasChanged();Preface xvii
      return true;
}
```

The source code can be cloned from `main` branch or downloaded from release area.

### Changelog:
- **2022-09-27**: Built using Visual Studio 2022, 17.3.4, using a workaround to resolve build issue below
> [VS 17.3.4 Breaks Android build with - Manifest][1]


[1]: https://github.com/dotnet/maui/issues/10102

**Following is what you need for this book:**

An evolution of Xamarin.Forms, .NET Multi-platform App UI (.NET MAUI) is a cross-platform framework for creating native mobile and desktop apps with C# and XAML. Using .NET MAUI, you can develop apps that’ll run on Android, iOS, macOS, and Windows from a single shared code-base. This step-by-step guide provides a comprehensive introduction to those who are new to .NET MAUI that will have you up to speed with app development using .NET MAUI in no time.	

With the following software and hardware list you can run all code files present in the book (Chapter 01-12).

We also provide a PDF file that has color images of the screenshots/diagrams used in this book. [Click here to download it](https://packt.link/nvY4N).


### Related products <Other books you may enjoy>
* Enterprise Application Development with C# 10 and .NET 6 [[Packt]](https://www.packtpub.com/product/enterprise-application-development-with-c-10-and-net-6-second-edition/9781803232973) [[Amazon]](https://www.amazon.com/Enterprise-Application-Development-NET-professional/dp/1803232978)

* High-Performance Programming in C# and .NET  [[Packt]](https://www.packtpub.com/product/high-performance-programming-in-c-and-net/9781800564718) [[Amazon]](https://www.amazon.com/Mastering-High-Performance-9-0-NET/dp/1800564716)

## Get to Know the Author
**Roger Ye** is a Senior Software Engineering Manager and leading a software development team at EPAM Systems. His team helps clients develop modern web based applications deployed to on-premises data centers or cloud. Before EPAM, he was a senior software engineering manager at McAfee where he led a team working on mobile security application development for Android, iOS and Windows.

### Download a free PDF

 <i>If you have already purchased a print or Kindle version of this book, you can get a DRM-free PDF version at no cost.<br>Simply click on the link to claim your free PDF.</i>
<p align="center"> <a href="https://packt.link/free-ebook/9781800569225">https://packt.link/free-ebook/9781800569225 </a> </p>