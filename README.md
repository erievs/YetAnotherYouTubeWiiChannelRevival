# YetAnotherYouTubeWiiChannelRevival

YetAnotherYouTubeWiiChannelRevival is a backend/server for the old YouTube Wii Channel. 

# Design

This whole project is built almost 100% around an auto-updating wrapper for yt-dlp, which should ensure that this project will keep working for as long
yt-dlp keeps gettings updates (the wrapper doesn't break for whatever reason)

It was (somewhat) based off of my eariler Lincoln project, HOWEVER, a good chunk of stuff has been redesigned/is going to be redesigned.

# Credits 
Project:
- YouTube
- Wii
- Bluegrams for YouTubeDLSharp (https://github.com/Bluegrams/YoutubeDLSharp)

YT-2009 Wii:
- SuperrSonic for finding out information of the channel using VP8 for video playback
- Mrt84 helped with packing the WAD and views.
- DX for creating yt2009!

# Contact
- pnj3.0 on Discord
- ksportalcraft@gmail.com (I sometimes check it).

# Support
- Home Page | 90%
- Search Page | 90%
- Video Playback | 70%
- Leanback Lite V3 | Not Started

# Guide

**This guide is a current work in progress, if you are dying to try out this project, the old guide does work for patching the client for the most part, which you can find the older guide here: https://github.com/erievs/yt2009-wii/blob/og-branch/wii_setup.md.**

This guide is written around Linux, and specifically Fedora 44, this guide should apply to pretty much any up to date Linux distro. However I will not be
providing any support for Windows, primarly due to the fact that ffmpeg can be fairly weird on Windows, and not include libvpx (WHICH IS NEEDED!!!!). On Linux is is almost always present. 

## Step 1

Before we can do anything we need to install dotnet, JPEXS, WINE (this will come into use later)! Make sure **dotnet-sdk-10** is installed (https://learn.microsoft.com/en-us/dotnet/core/install/linux-fedora?tabs=dotnet10), and WINE ```dnf install wine```. This should be all you need thankfully, as the wrapper for yt-dlp handles downloading ffmpeg, and such!

## Step 