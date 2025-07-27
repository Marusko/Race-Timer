# Race Timer
Application to show elapsed time from entered start time, with clock or individual starts on user-selected screen. Best used with RaceResult presenter as this app can automatically make the timer smaller, so it can fit over the presenter window without pressing `F11` on the presenter window, or show results directly in the app.
### Small timer with WebView RR presenter after the start

![Small timer](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/smallTimer.png)

### Fullscreen timer after the start
![Fullscreen timer](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/bigTimer.png)

### Starts window
![Starts window](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/startsWindow.png)

## Table of content
* [Installation](#installation)
* [Preparation](#preparation)
* [Using the app](#using-the-app)
  * [Left side menu](#left-side-menu)
  * [Starts](#starts)
  * [Finish timer](#finish-timer)
  * [Finish API Timer](#finish-api-timer)
  * [Contests](#contests)
  * [Displaying](#displaying)
  * [Timer control](#timer-control)
  * [Settings](#settings)

---
# Features
* Show a big timer with the event name on the result presenting screens when no one has finished
* Show a small timer when someone has finished
* Automatic switch from the big fullscreen timer to the small timer
* Manual switching between big and small timer
* Show results directly in the app
* Event name and type can be entered manually or using the RaceResult Simple API link
* When start times are `larger` than the current time, instead of a timer, the clock is shown 
* When the timer is on fullscreen, you can choose the position of the timer
* You can import your logo, it best works for a rectangle logo
* You can import your own QR code or provide a link and the app will convert it
* Multiple contests with start times, manual or from API
* Individual start times from CSV file or RaceResult API
## Automatic switching
  When someone finishes the race, the count in RR is updated and cashed on the second API link. The app checks this second link every 20 seconds. When the clock switches to the small clock, the app stops checking the second link.
## Choosing screen
  You can choose on which screen the timer will be shown, the list of screens is refreshed every 20 seconds. If the timer is open, refreshing the list of screens is paused.
## Multiple contests
  You can import contests with names and start times from Race Result, edit them, or create them manually. Elapsed times are scrolled through when the timer window is open.
## Individual starts
  You can import individual starts from CSV file or RaceResult API with auto update. The application will display participant name, countdown to their start with beeps and next participant.
  
---  
# Dependencies
The app requires [.NET8](https://dotnet.microsoft.com/en-us/download) to be installed. Most probably you already have it. If not, it will be downloaded and installed when installing the Race Timer
  
# Installation
[Top](#race-timer)
  1. Download [zip file](https://github.com/Marusko/Race-Timer/releases) and extract
  2. Double-click on `setup.exe`
  3. You will get an `Unknown publisher` warning, press install[^1]
  4. That's it!
  
---
# Preparation
[Top](#race-timer)
## Using the starts
You can import a CSV file with participants and their start times, or use RaceResult API.
### CSV example
CSV file format is `[Bib];[Name];[Start time]`
```
23;Mat 6;20:35:00
52;Mat 7;20:35:15
53;Mat 8;20:35:30
54;Mat 9;20:35:45
55;Mat 10;20:36:00
```
### Starts API link
> You NEED to either choose CSV file or provide API link, or the starts will not work
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Custom** from the left dropdown menu, in the details write or copy `data/list?&fields=Bib,DisplayName,Start.ToD&sort=Start.ToD&listformat=JSON`, and in the label write `starts` :bangbang: **Important** (see the picture)
![Creating starts API link](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/startsApi.png)
3. Click the blue icon on this API under the `Link` column on the right, then copy the link and paste it into the `Finish API Timer` tab in the `API link` text box and click `Load` button :bangbang: Only after enabling starts

## Using the Timer
If you don't time the race with RaceResult or don't want to create links, simply use the `Timer` tab to set up the timer. Automatic switching of timer windows is disabled. You can still manually switch the windows.
## Using the API Timer
> Only works with RaceResult Simple API
### Event API link
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Settings** from the left dropdown menu, in the details write or copy `EventName,EventType`, and in the label write `main` :bangbang: **Important** (see the picture)
![Creating event API link](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWmainAPI.png)
### Count API link
> If you don't enter the Count link, the automatic switching of timer windows will be disabled. You can still manually switch the windows.
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Custom** from the left dropdown menu, in the details write or copy `data/count?filter=[Finished]=1`, and in the label write `count` :bangbang: **Important** (see the picture)
![Creating count API link](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWcountAPI.png)
### Contest API link
> If you don't enter the Contest link, you will have to add and edit contest and start times manually
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Custom** from the left dropdown menu, in the details write or copy `contests/get`, and in the label write `contest` :bangbang: **Important** (see the picture)
![Creating contest API link](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWcontestAPI.png)
### All API link
> If you don't enter this API link, the API timer will not work
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Custom** from the left dropdown menu, in the details write or copy `simpleapi/get`, and in the label write `api` :bangbang: **Important** (see the picture)
![Creating all API link](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWallAPI.png)
 3. Click the blue icon on this API under the `Link` column on the right, then copy the link and paste it into the `Finish API Timer` tab in the `API link` text box and click `Load` button

### The result:

![All API links created](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/NEWphotos/NEWAPI.png)

---
# Using the app
[Top](#race-timer)
## Left side menu
![Left side menu](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/left.png)

[Top](#race-timer)
  * **Starts** - Menu for setting up and controlling individual participant starts
  * **Finish timer** - Menu for entering event name, type, and logo image, and starting the timer
  * **Finish API Timer** - Menu for entering API link, logo image, and starting the timer
  * **Contests** - Menu for adding contests with start times manually and editing them
  * **Displaying** - Menu for setting up results and QR code
  * **Timer control** - Menu for switching between fullscreen and small timer, closing the timer, refreshing the WebView, and setting new start times
  * **Settings** - Choosing the screen on which the timer will be displayed, choosing the timer layout and informations about the app
  * *NTP success / NTP fail* - Status of NTP timer synchronization

## Starts
![First tab, individual starts, setup](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/startsSetupSecond.png)

[Top](#race-timer)
  * **Setup** - Tab for setting up the start times
    * **Enable individual starts** - Enable or disable individual starts
    * **Use list from API** - Switch between using CSV file as source or RaceResult API as source
    * **Select file** - Select the start times CSV file, above the button will be shown the name of the file, enabling the delete button
    * **Delete file** - Delete loaded starts from CSV file
    * **Open Timer** - Opens the starts window on selected screen, disables all tabs and enables **Control**

![First tab, individual starts, control](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/startsControl.png)

  * **Control** - Tab for controlling the start times window
    * **Close timer** - Closes the starts window and clears the start time list
    * **Start times** - Shows next participants that will be starting

## Finish timer
![Second tab, with manual settings](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/timer.png)

[Top](#race-timer)
  * **Event name** - Event name that will be displayed with the timer
  * **Event type** - Event type that will be displayed on the big fullscreen timer
  * **Select image** - Choose an image from the PC to be displayed as the logo, it best works with a rectangle logo, 
                       above the button will be shown the name of the image, enabling the delete button (shared with **API Timer**)
  * **Delete image** - Clear the selected image (shared with **API Timer**)
  * **Open timer** - Open fullscreen timer, disables all tabs and enables **Timer control**
  
## Finish API Timer
![Third tab, with API settings, API timer](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/apiTimer.png)

[Top](#race-timer)
  * **API Timer** - Tab for entering links and setting up the API timer
    * **API Link** - Here enter the API link to get all other APIs or starts API
    * **Load** - Loads all three APIs, loads Contest if possible, and sets the status of the API links below (or single API if using starts)
    * Statuses can be:
      * `MIS` - The link is missing
      * `OFF` - The link is disabled
      * `ERR` - Something went wrong when reading the link
      * `OK` - The link is correct
    * **Select image** - Choose an image from the PC to be displayed as the logo, it best works with a rectangle logo, 
                       above the button will be shown the name of the image, enabling the delete button (shared with **Timer**)
    * **Delete image** - Clear the selected image (shared with **Timer**)
    * **Open timer** - Open fullscreen timer, disables all tabs and enables **Timer control**

![Third tab, with API settings, API cheatsheet](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/apiCheatsheet.png)
  * **API cheat sheet** - Tab with API settings, to quickly find and paste when creating APIs in Race Result

## Contests
![Fourth tab, creating and editing contests](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/contests.png)

[Top](#race-timer)
  * **Add contest** - Adds a new line to the list below
  * **For each contest**
    * **Name** - Name of the contest which will be shown with elapsed time
    * **Start time** - Start time of the contest
    * **Remove** - Removes this contest from the list
   
## Displaying
![Fifth tab, with result settings](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/results2.png)

[Top](#race-timer)
  * **Results** - Tab for setting up the webview results and info panel
    * **Enable WebView** - If checked MiniTimer will have its browser, in which the results will be shown
    * **Link to web page with results** - Here enter the link with results, which will be shown with the MiniTimer
    * **Enable info panel** - If checked MiniTimer will show info panel with custom information - panel will be shown every 3 minutes for 30 seconds
    * **Information to show** - Here enter the information you want to show, you can show multiple information separated by `;`

![Fifth tab, with QR code settings](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/qrCode.png)

  * **QR code** - Tab for setting up the QR code
    * **Generate QR code** - Here enter the link to convert to a QR code
    * **Generate QR** - Generates the QR code and sets it
    * **Select QR** - Choose an image from the PC to be displayed as a QR code, above the button will be shown the name of the image, enabling the delete button
    * **Delete QR** - Clear the selected QR code
    * **Show QR code when minimized** - If checked, MiniTimer will periodically show a QR code, based on input from the two text boxes below in the 1 - 60 range

## Timer control
> Tab is enabled when the timer is open

![Sixth tab, timer control](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/timerControl.png)

[Top](#race-timer)
  * **Reload** - To manually reload WebView, if it is enabled
  * **Minimize** - To manually switch from fullscreen to the small timer
  * **Maximize** - To manually switch from the small to the fullscreen timer
  * **Close timer** - To close the timer window, the timer window also closes when closing the main window
  * **Contest selection** - Here select the contest, for which the start time has to be changed
  * **New start time** - Here enter new start time for the selected contest
  * **Set** - Sets the new start time for selected contest
  * **New information** - Here enter new information to show on info panel
  * **Set** - Sets the new information, new information will be show on next panel show
  * **Current start times** - Table shows currently set start times

## Settings
![Seventh tab, display settings](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/settings.png)

[Top](#race-timer)
  * **Display setting** - Tab for selecting display to show the timer or starts and timer position
    * **Display** - List of connected displays to choose, on which the timer window will be opened. Display names here **sometimes AREN'T** the same as in Windows settings
    * **Timer and image alignment** - List of available alignments 
        * Timer on top - Timer on top, clock in the middle, logo on the bottom, QR code **NOT** displayed
        * Timer on left - Timer and clock on the left side, logo and QR code on the right side
        * Timer on right - Timer and clock on the right side, logo and QR code on the left side

![Seventh tab, info](https://yhoikcyzjxfcerunfwok.supabase.co/storage/v1/object/public/eventifyePictures/RaceTimer/320photos/info.png)     

  * **Information** - Tab with informations about the application

---
:exclamation: **Disclaimers** "RR" and "RaceResult" are trademarked by and belong to race result AG. I make no claims to these or any assets belonging to race result AG and use them purely for informational purposes only.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT.

[^1]: This app doesn't collect anything from PC or RR. I don't have a signing certificate
