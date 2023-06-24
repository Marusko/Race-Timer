# Race Timer :stopwatch:
Application to show elapsed time from entered start time, with clock, on user-selected screen. Best used with RaceResult presenter as this app can automatically make the timer smaller, so it can fit over the presenter window without pressing `F11` on the presenter window.
### Small timer over the RR presenter after the start
![Small timer](https://github.com/Marusko/Race-Timer/assets/93376571/2749d93a-f3ab-4ba2-9d26-129fcf602039)

### Fullscreen timer after the start
![Fullscreen timer](https://github.com/Marusko/Race-Timer/assets/93376571/b97f476e-b3c1-47a5-8714-feb9a815f2aa)

---
# Features :star:
* Show a big timer with the event name on the result presenting screens when no one has finished
* Show a small timer when someone has finished
* Automatic switch from the big fullscreen timer to the small timer
* Manual switching between big and small clock
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
If you don't time the race with RaceResult, or don't want to create links, simply use the `Timer` tab to set up the timer. Automatic switching of timer windows is disabled. You can still manually switch the windows.
## Using the API Timer :watch:
> Only works with RaceResult Simple API
### Main API link :link:
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Settings** from the left dropdown menu, in the details write `EventName,EventType`(see the picture)
![Creating main API link](https://user-images.githubusercontent.com/93376571/228536613-721357e7-d342-403c-9e85-fb77a9ba8633.png)
 3. Click the blue icon on this API under the `Link` column on the right, then copy the link and paste it into the `API Timer` tab in the `API link` text box
### Count API link :link:
> If you don't enter the Count link, the automatic switching of timer windows will be disabled. You can still manually switch the windows.
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Custom** from the left dropdown menu, in the details write or copy `data/count?filter=[Finished]=1`(see the picture)
![Creating count API link](https://github.com/Marusko/Race-Timer/assets/93376571/a815a921-59e7-454e-98ba-30f4314fb850)
 3. Click the blue icon on this API under the `Link` column on the right, then copy the link and paste it into the `API Timer` tab in the `Count link` text box
### Contest API link :link:
> If you don't enter the Contest link, you will have to add and edit contest and start times manually
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Custom** from the left dropdown menu, in the details write or copy `contests/get`(see the picture)
![Creating contest API link](https://github.com/Marusko/Race-Timer/assets/93376571/bd5711f7-ad8f-43c7-a98d-a5cd1be2e273)
 3. Click the blue icon on this API under the `Link` column on the right, then copy the link and paste it into the `Contests` tab in the `Contests link` text box

The result:

![All API links created](https://github.com/Marusko/Race-Timer/assets/93376571/ebfc47f2-5557-4e51-b435-bdf2d493a3df)

---
# Using the app :computer:
## Left side menu :fleur_de_lis:
![Left side menu](https://github.com/Marusko/Race-Timer/assets/93376571/ac4c6759-1ee3-4d91-aa4e-8ce7ddbbfd65)
  * **Timer** - Menu for entering event name, type and start time, logo image, and starting the timer
  * **API Timer** - Menu for entering API link for name and type, logo image, and starting the timer
  * **Contests** - Menu for adding contests with start times, manually or from API, and editing them
  * **QR code** - Menu for setting or generating QR code
  * **Timer control** - Menu for switching between fullscreen and small timer, and closing the timer
  * **Display settings** - Choosing the screen on which the will timer be displayed, choosing the timer layout
  * **Informations** - Informations about the app

## Timer :hourglass_flowing_sand:
![First tab, with manual settings](https://github.com/Marusko/Race-Timer/assets/93376571/620e9ab8-4e12-4f8a-9e94-82cf4cedb422)
  * **Event name** - Event name that will be displayed with the timer
  * **Event type** - Event type that will be displayed on the big fullscreen clock
  * **Select image** - Choose an image from the PC to be displayed as the logo, it best works with a rectangle logo, 
                       above the button will be shown the name of the image, enabling the delete button (shared with **API Timer**)
  * **Delete image** - Clear the selected image (shared with **API Timer**)
  * **Open timer** - Open fullscreen timer, disables all settings and enables **Timer control**
  
## API Timer :watch:
![Second tab, with API settings, API timer](https://github.com/Marusko/Race-Timer/assets/93376571/5af42eb8-0686-43ac-b39a-2db15c5850e6)
  * **API Timer** - Tab for entering links and setting up the API timer
    * **API Link** - Here enter the main link to get the name and type of the event
    * **List Link** - Here enter the link which points to the counter, for automatic switching of the timer windows
    * **Select image** - Choose an image from the PC to be displayed as the logo, it best works with a rectangle logo, 
                       above the button will be shown the name of the image, enabling the delete button (shared with **Timer**)
    * **Delete image** - Clear the selected image (shared with **Timer**)
    * **Open timer** - Open fullscreen timer, disables all settings and enables **Timer control**

![Second tab, with API settings, API timer](https://github.com/Marusko/Race-Timer/assets/93376571/4ab47cc5-1435-4ab5-ac00-5fd969526047)
  * **API cheat sheet** - Tab with API settings, to quickly find and paste when creating API in Race Result

## Contests :runner::runner:
![Third tab, creating and editing contests](https://github.com/Marusko/Race-Timer/assets/93376571/41439e55-1e66-4922-9607-885e20c8c549)
  * **Contests link** - Here enter the Contests link to load all contests from RR
  * **Load** - Reads the Contests link and adds loaded contests to the list below, you can then edit them
  * **Add start time** - Adds a new line to the list below
  * **For each contest**
    * **Name** - Name of the contest which will be shown with elapsed time
    * **Start time** - Start time of the contest
    * **Remove** - Removes this contest from the list

## QR code :framed_picture:
![Fourth tab, with QR code settings](https://github.com/Marusko/Race-Timer/assets/93376571/025d6df9-a3bc-4357-b76f-1ab3e2d31ff7)
  * **Generate QR code** - Here enter the link to convert to a QR code
  * **Generate QR** - Generates the QR code and sets it
  * **Select QR** - Choose an image from the PC to be displayed as a QR code, above the button will be shown the name of the image, enabling the delete button
  * **Delete QR** - Clear the selected QR code
  * **Show QR code when minimized** - If checked, minimized timer will periodically show a QR code, based on input from the two text boxes below in the 1 - 60 range

## Timer control :wrench:
> Tab is enabled when the timer is open

![Fifth tab, timer control](https://github.com/Marusko/Race-Timer/assets/93376571/b930120a-55c6-432d-b0fd-935f54cf19d7)
  * **Minimize** - To manually switch from fullscreen to the small timer
  * **Maximize** - To manually switch from the small to the fullscreen timer
  * **Close timer** - To close the timer window, the timer window also close when closing the main window

## Display settings :hammer_and_wrench:
![Sixth tab, display settings](https://github.com/Marusko/Race-Timer/assets/93376571/aef1a5ce-2d42-435c-b6c5-f2a1e47d4098)
  * **Display** - List of connected displays to choose on which the timer window will be opened. Display names here **AREN'T** the same as in Windows settings
  * **Timer and image alignment** - List of available alignments 
      * Timer on top - Timer on top, clock in the middle, logo on the bottom, QR code **NOT** displayed
      * Timer on left - Timer and clock on the left side, logo and QR code on the right side
      * Timer on right - Timer and clock on the right side, logo and QR code on the left side

---
:exclamation: **Disclaimers** "RR" and "RaceResult" are trademarked by and belong to race result AG I make no claims to these or any assets belonging to race result AG and use them purely for informational purposes only.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT.

[^1]: This app doesn't collect anything from PC or RR. I don't have a signing certificate
