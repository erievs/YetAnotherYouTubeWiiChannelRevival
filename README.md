# YetAnotherYouTubeWiiChannelRevival

YetAnotherYouTubeWiiChannelRevival is a backend/server for the old YouTube Wii Channel. 

# Design

This whole project is built almost 100% around an auto-updating wrapper for yt-dlp, which should ensure that this project will keep working for as long
yt-dlp keeps gettings updates (the wrapper doesn't break for whatever reason)

It was (somewhat) based off of my eariler Lincoln project, HOWEVER, a good chunk of stuff has been redesigned/is going to be redesigned.

# Quirks

- The ```/wiitv_web``` will not work in a web browser due to CORS on the YouTube video stream URL. It will work fine in a flash projectors. This does not effect the wii or web_flv.

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

Before we can do anything we need to install dotnet, JPEXS, WINE (this will come into use later)! Make sure **dotnet-sdk-10** is installed (https://learn.microsoft.com/en-us/dotnet/core/install/linux-fedora?tabs=dotnet10), JPEXS here https://flathub.org/en/apps/com.jpexs.decompiler.flash, and WINE ```dnf install wine```. This should be all you need thankfully, as the wrapper for yt-dlp handles downloading ffmpeg, and such!

## Step 2

We need to get our **local** IP adressess (not local IP addresses are **NOT** the same as your public IP adresses, they are most of the time something like 192.168.1.xm, these cannot be used to find a general locaction!).

Run the command ```hostname -i```, your IP adress should be the last item, like 192.168.1.220. This will be used to access this backend!

## Step 3 

We now must configure ASPCORE to use our hostname, so go to the ```Properties``` folder, open ```launchSettings.json```, then ```192.168.1.220``` (or whatever it is) to your IP adress you found in Step 2!

## Step 4

Testing to see if it worked, go and ope a terminal to the root of this project (the folder where this .md is) and run ```sudo dotnet run```, we are running this as root as we are running this on port 80 (if you want to run this not as root in ```launchSettings.json``` add :(your port) to ```192.168.1.220```  like ```192.168.1.220:8080```) as I find you have a much better time using port 80 than other ports. It may just be luck for me, but 80 tends to work better in this case.

If it doesn't give you any errrors, and if you visit your ip adress in a web browser it display a page, great job, it worked!

## Step 5 

We are now going to patching the .swfs, which is the real app, since the YouTube Wii Channel is just a flash player with some extra stuff loading ```leanbacklite_wii```.

If you are confused to why they're multiple versions of the same file. This is more or less for testing the web app in a web browser for videoplayback. The _web and _web_flv just use a different codec which works in web browsers and flash projectors. The Wii uses VP8 as its codec which doesn't play nice in web browsers and flash projectors. If you want to patch them just follow the exact same steps as the wii one, there's no difference to you!

Open up JJPEXS (you may need to run ```flatpak run com.jpexs.decompiler.flash``` if you just installed it), and drag and drop ```apiplayer.swf```, ```apiplayer-vflZLm5Vu.swf```, and ```leanbacklite_wii.swf``` from the assets folder into JJPEXS.

Fun Fact: The player is loaded not at when you click ona video but at startup!

## Step 6

Let's start off with the ```leanbacklite_wii.swf``` click on in JPEXs go over to tools click text search and looks for ```192.168.1```, sadly find and replace won't work (IDK why). This is the webb

Double click, on the first result it'll take you there, click 'Edit Actionscript' on the bottom, click save. Repeat untuil you have none left! Make sure to ctrl-f and search in each class as well because sometimes you miss them. 

I'd test in a browser with ruffle, 192.168.220/wiitv (or your ip adress), most of the time if it doesn't work you just missed a spot, ctrl f is your friend! 

Pro Tip: Also you run it in a browser if you have ruffle install and go to /wiitv, and open up the dev console with ctrl-i and it'll tell you 404 errors!

**MAKE SURE TO GO TO FILE AND CLICK SAVE WHEN DONE!!!!!!**

## Step 7 

Let's now do ```apiplayer.swf```, this is slightly different, we don't use the find tool here. This loads the ```apiplayer-vflZLm5Vu.swf``` which is the proper end for the player! This is info for the player something idk. 

Go under ```scripts``` ```frame 1``` ```Do Action``` and edit actionscript, and just change the two instances of 192.168.1.220 (or whatever it is) to yours.

**MAKE SURE TO GO TO FILE AND CLICK SAVE WHEN DONE!!!!!!**

## Step 8

We are going to finish off with ```apiplayer-vflZLm5Vu.swf```, this is the exact same process of step Step 6. Just search and edit! This is the proper player it handles fetchings videos and stuff, however do mind that the UI itself is within the leanbacklite client, not here. This just handles getting the video and some other stuff, the UI elements there.

**MAKE SURE TO GO TO FILE AND CLICK SAVE WHEN DONE!!!!!!**

## Step 9 

Go into your browser and make sure it will attempt to play, it won't because it is VP8, once it converts it'll do something weird if you are using ruffle. But it should try, if you see the circle you are good.

## Step 10 

PS: You may want to relaunch your server, to be safe. I do not know if aspcore loads from the assets folder in the root or a copied assets folder, which if its the latter you'd not have the changes take effect there.

Okay this is the most annoying part in my opion patching the WAD, find a YouTube WAD, find ```ShowMiiWads.exe``` and ```U8Mii.exe``` somewhere on the net, I do not remember where I found mine tbh. 