# Tweet-DL
<img src=".github/TweetDL.png" width="96px" />
A command-line downloader to download images from multiple Tweets or your bookmarks  

<hr>

## **NOTE: This repository is deprecated!**  
This was one of my first serious coding projects and I hate to see that it does not work anymore. It has not for more than a year now and I don't want to bother with the Twitter/X API again, especially since I don't use it very often anymore.  
I'll still keep it here for historical and educational purposes, but if I were to re-make it, I'd do it properly (use the GraphQL API like you're supposed to, proper formatting and structuring, etc.), but also add Mastodon and Blueksky. Probably won't happen though.

<hr>

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
