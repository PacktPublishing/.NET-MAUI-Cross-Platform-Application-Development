# .NET MAUI Cross-Platform Application Development - Second Edition

.NET MAUI Cross-Platform Application Development, Second Edition, published by Packt

<a href="https://www.packtpub.com/product/.net-maui-cross-platform-application-development/9781800569225"><img src="https://static.packt-cdn.com/products/9781800569225/cover/smaller" alt=".NET MAUI Cross-Platform Application Development" height="256px" align="right"></a>

This is the code repository for .NET MAUI Cross-Platform Application Development, Second Edition, published by Packt.

**Leverage a first-class cross-platform UI framework to build native apps on multiple platforms    **

## What is this book about?

This book is an entry-level .NET MAUI book for mobile developers interested in cross-platform application development with working experience of the .NET Core framework, as well as fresh or junior engineers who’ve just begun their career in mobile app development. Native application developers (desktop) or Xamarin developers who want to migrate to .NET MAUI will also benefit from this book. Basic knowledge of modern object-oriented programming language, such as C#, Java or Kotlin, is assumed.    

This book covers the following exciting features:

* Discover the latest features of .NET 8 that can be used in mobile and desktop app development
* Find out how to build cross-platform apps with .NET MAUI and Blazor
* Integrate third-party libraries and add your own device-specific features
* Discover .NET class unit test using xUnit.net
* Deploy apps in different app stores on mobile as well as desktop

## Instructions and Navigations

All of the project files are organized into folders in the main branch. For example, Chapter03.

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

    StateHasChanged();
    return true;
}
```

The source code can be cloned from `main` branch or downloaded from release area.

There are also branches for each chapters, such as `2nd/chapter01` or `2nd/chapter02` etc.

## Get to Know the Author

**Roger Ye** is a Senior Software Engineering Manager, leading a software development team at EPAM Systems. His team assists clients in developing modern web-based applications that can be deployed in on-premises data centers or the cloud. Before joining EPAM, he served as a Senior Software Engineering Manager at McAfee, where he led a team focused on mobile security application development for Android, iOS, and Windows platforms.
