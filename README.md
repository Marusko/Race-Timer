# Race Timer :stopwatch:
Application to show elapsed time from entered start time, with clock, on user-selected screen. Best used with RaceResult presenter as this app can automatically make the timer smaller, so it can fit over the presenter window without pressing `F11` on the presenter window, or show results directly in the app.
### Small timer with WebView RR presenter after the start

![Small timer](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/photos/SmallTimer.png)

### Fullscreen timer after the start
![Fullscreen timer](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/photos/BigTimer.png)

## Table of content
* Installation
* Preparation
* Using the app
  * Left side menu
  * Timer
  * API Timer
  * Contests
  * Results
  * QR Code
  * Timer control
  * Display settings

---
# Features :star:
* Show a big timer with the event name on the result presenting screens when no one has finished
* Show a small timer when someone has finished
* Automatic switch from the big fullscreen timer to the small timer
* Manual switching between big and small clock
* Show results directly in the app
* Event name and type can be entered manually or using the RaceResult Simple API link
* When start times are `larger` than the current time, instead of a timer, the clock is shown 
* When the clock is on fullscreen, you can choose the position of the timer
* You can import your logo, it best works for a rectangle logo
* You can import your own QR code or provide a link and the app will convert it
* Multiple contests with start times, manual or from API
## Automatic switching :on:
  When someone finishes the race, the count in RR is updated and cashed on the second API link. The app checks this second link every 15 seconds. When the clock switches to the small clock, the app stops checking the second link.
## Choosing screen :desktop_computer:
  You can choose on which screen the timer will be shown, the list of screens is refreshed every 20 seconds. If the timer is open, refreshing the list of screens is paused.
## Multiple contests :runner::runner:
  You can import contests with names and start times from Race Result, edit them, or create them manually. Elapsed times are scrolled through when the timer window is open.
  
---  
# Dependencies :wood:
The app requires [.NET7](https://dotnet.microsoft.com/en-us/download) to be installed. Most probably you already have it. If not, it will be downloaded and installed when installing the Race Timer
  
# Installation :cd:
  1. Download [zip file](https://github.com/Marusko/RaceResult_UI_Timer/releases) and extract
  2. Double-click on `setup.exe`
  3. You will get an `Unknown publisher` warning, press install[^1]
  4. That's it!
  
---
# Preparation :gear:
## Using the Timer :hourglass:
If you don't time the race with RaceResult or don't want to create links, simply use the `Timer` tab to set up the timer. Automatic switching of timer windows is disabled. You can still manually switch the windows.
## Using the API Timer :watch:
> Only works with RaceResult Simple API
### Event API link :link:
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Settings** from the left dropdown menu, in the details write or copy `EventName,EventType`, and in the label write `main` :bangbang: **Important** (see the picture)
![Creating event API link](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWmainAPI.png)
### Count API link :link:
> If you don't enter the Count link, the automatic switching of timer windows will be disabled. You can still manually switch the windows.
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Custom** from the left dropdown menu, in the details write or copy `data/count?filter=[Finished]=1`, and in the label write `count` :bangbang: **Important** (see the picture)
![Creating count API link](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWcountAPI.png)
### Contest API link :link:
> If you don't enter the Contest link, you will have to add and edit contest and start times manually
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Custom** from the left dropdown menu, in the details write or copy `contests/get`, and in the label write `contest` :bangbang: **Important** (see the picture)
![Creating contest API link](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWcontestAPI.png)
### All API link :link:
> If you don't enter this API link, the API timer will not work
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Custom** from the left dropdown menu, in the details write or copy `simpleapi/get`, and in the label write `api` :bangbang: **Important** (see the picture)
![Creating all API link](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWallAPI.png)
 3. Click the blue icon on this API under the `Link` column on the right, then copy the link and paste it into the `API Timer` tab in the `API link` text box and click `Load` button

### The result:

![All API links created](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWAPI.png)

---
# Using the app :computer:
## Left side menu :fleur_de_lis:
![Left side menu](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWleftMenu.png)
  * **Timer** - Menu for entering event name, type and logo image, and starting the timer
  * **API Timer** - Menu for entering API link, logo image, and starting the timer
  * **Contests** - Menu for adding contests with start times manually and editing them
  * **Results** - Menu for enabling WebView on MiniTimer and setting result link
  * **QR code** - Menu for setting or generating QR code
  * **Timer control** - Menu for switching between fullscreen and small timer, closing the timer, refreshing the WebView, and setting new start times
  * **Display settings** - Choosing the screen on which the will timer be displayed, choosing the timer layout
  * **Informations** - Informations about the app

## Timer :hourglass_flowing_sand:
![First tab, with manual settings](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWtimer.png)
  * **Event name** - Event name that will be displayed with the timer
  * **Event type** - Event type that will be displayed on the big fullscreen clock
  * **Select image** - Choose an image from the PC to be displayed as the logo, it best works with a rectangle logo, 
                       above the button will be shown the name of the image, enabling the delete button (shared with **API Timer**)
  * **Delete image** - Clear the selected image (shared with **API Timer**)
  * **Open timer** - Open fullscreen timer, disables all tabs and enables **Timer control**
  
## API Timer :watch:
![Second tab, with API settings, API timer](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWAPItimer.png)
  * **API Timer** - Tab for entering links and setting up the API timer
    * **API Link** - Here enter the API link to get all other APIs
    * **Load** - Loads all three APIs, loads Contest if possible, and sets the status of the API links below
    * Statuses can be:
      * `MIS` - The link is missing
      * `OFF` - The link is disabled
      * `ERR` - Something went wrong when reading the link
      * `OK` - The link is correct
    * **Select image** - Choose an image from the PC to be displayed as the logo, it best works with a rectangle logo, 
                       above the button will be shown the name of the image, enabling the delete button (shared with **Timer**)
    * **Delete image** - Clear the selected image (shared with **Timer**)
    * **Open timer** - Open fullscreen timer, disables all tabs and enables **Timer control**

![Second tab, with API settings, API cheatsheet](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWAPIcheat.png)
  * **API cheat sheet** - Tab with API settings, to quickly find and paste when creating APIs in Race Result

## Contests :runner::runner:
![Third tab, creating and editing contests](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWcontest.png)
  * **Add contest** - Adds a new line to the list below
  * **For each contest**
    * **Name** - Name of the contest which will be shown with elapsed time
    * **Start time** - Start time of the contest
    * **Remove** - Removes this contest from the list
   
## Results :grinning:
![Fourth tab, with WebView settings](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWresult.png)
  * **Enable WebView** - If checked MiniTimer will have its browser, in which the results will be shown
  * **Link to web page with results** - Here enter the link with results, which will be shown with the MiniTimer

## QR code :framed_picture:
![Fifth tab, with QR code settings](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWqrCode.png)
  * **Generate QR code** - Here enter the link to convert to a QR code
  * **Generate QR** - Generates the QR code and sets it
  * **Select QR** - Choose an image from the PC to be displayed as a QR code, above the button will be shown the name of the image, enabling the delete button
  * **Delete QR** - Clear the selected QR code
  * **Show QR code when minimized** - If checked, MiniTimer will periodically show a QR code, based on input from the two text boxes below in the 1 - 60 range

## Timer control :wrench:
> Tab is enabled when the timer is open

![Sixth tab, timer control](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWcontrol.png)
  * **Reload** - To manually reload WebView, if it is enabled
  * **Minimize** - To manually switch from fullscreen to the small timer
  * **Maximize** - To manually switch from the small to the fullscreen timer
  * **Close timer** - To close the timer window, the timer window also closes when closing the main window
  * **Contest selection** - Here select the contest, for which the start time has to be changed
  * **New start time** - Here enter new start time for the selected contest
  * **Set** - Sets the new start time for selected contest
  * **Current start times** - Table shows currently set start times

## Display settings :hammer_and_wrench:
![Seventh tab, display settings](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWdisplay.png)
  * **Display** - List of connected displays to choose on which the timer window will be opened. Display names here **sometimes AREN'T** the same as in Windows settings
  * **Timer and image alignment** - List of available alignments 
      * Timer on top - Timer on top, clock in the middle, logo on the bottom, QR code **NOT** displayed
      * Timer on left - Timer and clock on the left side, logo and QR code on the right side
      * Timer on right - Timer and clock on the right side, logo and QR code on the left side

---
:exclamation: **Disclaimers** "RR" and "RaceResult" are trademarked by and belong to race result AG I make no claims to these or any assets belonging to race result AG and use them purely for informational purposes only.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT.

[^1]: This app doesn't collect anything from PC or RR. I don't have a signing certificate
