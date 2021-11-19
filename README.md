# Tweet-DL
A command-line downloader to download images from multiple Tweets or your bookmarks

<img src="/.github/TweetDL.png" width="128px" />

# Installation
## Downloading the executable
### Platform-Dependent (Recommended if .NET SDK installed)
Use this if you already have the .NET SDK installed:
* Download the latest Platform-Depdendent `Tweet-DL.zip` and [configure it like this](#adding-execute-script-only-platform-dependent)
* (WIP) Alternatively, you can install the Platform-Dependent version via the [jitter packet manager](https://github.com/Stridsvagn69420/jitter).

### Pre-compiled (Recommended)
Pre-compiled versions available:  
__Windows:__
* Windows AMD64
* Windows ARM
* Windows ARM64

__Linux:__
* Linux AMD64
* Linux ARM
* Linux ARM64

Just download the one you need from GitHub's release page

### Self-compiled
NOTE: You need to have .NET 6 SDK installed  
(Optionally you can change the `<TargetFramework>` in `Tweet-DL.csproj` to `net5.0` if you already have .NET 5 SDK installed)

Get the [Runtime-ID](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) for your OS and run this command (replace <YOUR_RID> with your Runtime ID):
```sh
dotnet publish -c Release -r <YOUR_RID> --self-contained true -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true -p:PublishTrimmed=True -p:TrimMode=CopyUsed
```

### Clone repository
Clone this repository and use `dotnet run` to run Tweet-DL.  
NOTE: This will compile from source first and then execute the application, thus start-up time will be the longest.

## Setting up config.json
This file needs to manually be added in order for Tweet-DL to work properly. Its location is `<YOUR_HOMEDIR>/.config/tweet-dl/config.json`
```js
{
	"delay": 69, //delay in ms for requests, must not be 0 and set higher if download fails
	"downloadDir": "C:/path/to/download/dir", //path to your prefered download folder
	"userAuth": {
		"cookie": "<YOUR_COOKIE_HEADER_HERE",
		"csrfToken": "<YOUR_X-CSRF-TOKEN_HEADER_HERE>"
	},
	"apiAuth": {
		"bearerToken": "<YOUR_BEARER_HERE>", //Bearer Token of Twitter API Application
	}
}
```
To get the Cookie and X-CSRF-Token header, you need to log-in into Twitter and open your browser's developer tools.
[How to get HTTP headers from Chrome DevTools](https://stackoverflow.com/questions/4423061/how-can-i-view-http-headers-in-google-chrome)

## Configuring PATH
You'll have to either place the independent platform binary/add a symlink linking to the binary inside a folder listed in PATH or [add the folder, which contains the Tweet-DL executeable, to your PATH variable](https://gist.github.com/nex3/c395b2f8fd4b02068be37c961301caa7).

### Adding execute script (only Platform-Dependent)
Instead of placing the binary/making a symlink into a folder listed inside your PATH, you have to add a little script that acts as an entry point:
```sh
#!/bin/sh
cd /path/to/tweetdl
dotnet Tweet-DL.dll "$@"
```

# Usage
## Download Twitter Bookmarks
Just execute the application, no command-line arguments needed!
`tweet-dl`

## Download images from Tweet(s)
If arguments are given, it will download each Tweet given as arguments (either Tweet URL or Tweet ID), e.g.:

Download by Tweet URL:  
`tweet-dl https://twitter.com/ashtom/status/1460488451304738817?s=20`

Download by Tweet ID:  
`tweet-dl 1460488451304738817`

Download multiple Tweets:  
`tweet-dl 1460488451304738817 https://twitter.com/github/status/1460696683705208837?s=20`
