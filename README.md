# RR_Timer :stopwatch:
Application to show elapsed time from entered start time, with clock, on user-selected screen. Best used with RaceResult presenter as this app can automatically make the timer smaller, so it can fit over the presenter window without pressing `F11` on the presenter window.
### Small timer over the RR presenter after the start
![Small timer](https://user-images.githubusercontent.com/93376571/231474167-d6fef6d4-5f84-4a07-8235-e185eabd58f1.png)
### Fullscreen timer after the start
![Fullscreen timer](https://user-images.githubusercontent.com/93376571/231473878-d7227c38-7b78-4fc7-a69a-0356eda6dd0b.png)

---
# Features :star:
* Show big timer with event name on result presenting screens when no one has finished
* Show small timer when someone has finished
* Automatic switch from big fullscreen timer to small timer
* Manual switching between big and small clock
* Event name and type can be entered manually or using RaceResult Simple API link
* When entered start time is `larger` than current time, instead of timer, clock is shown 
* When clock is on fullscreen, you can choose position of timer
* You can import your logo, best works for rectangle logo
* You can import your own QR code or provide a link and app will convert it
## Automatic switching :on:
  When someone finishes the race, custom output in RR is updated, cashed on second API link. App checks this second link every 15 seconds. When clock switches to small clock, app stops checking the second link. Required custom output list in RR and second API link.
## Choosing screen :desktop_computer:
  You can choose on which screen will the timer be shown, list of screens is refreshed every 20 seconds. If timer is open, refreshing list of screens is paused.
  
---  
# Dependencies :wood:
App requires [.NET7](https://dotnet.microsoft.com/en-us/download) to be installed. Most probably you already have it. If not, it will be downloaded and installed when installing the RR_Timer
  
# Installation :cd:
  1. Download [zip file](https://github.com/Marusko/RaceResult_UI_Timer/releases) and extract
  2. Double click on `setup.exe`
  3. You will get `Unknown publisher` warning, press install[^1]
  4. That's it!
  
---
# Preparation :gear:
## Using the Timer :hourglass:
If you don't time the race with RaceResult, or don't want to create links, simply use the `Timer` tab to setup the timer. Automatic switching of timer windows is disabled. You can still manually switch the windows.
## Using the API Timer :watch:
> Only works with RaceResult Simple API
### Main API link :link:
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Settings** from the left dropdown menu, in the details write `EventName,EventType`(see the picture)
![Creating main API link](https://user-images.githubusercontent.com/93376571/228536613-721357e7-d342-403c-9e85-fb77a9ba8633.png)
 3. Click the blue icon on this API under the `Link` column on the left, then copy the link and paste it in `API Timer` tab in the `API link` text box
### List API link :link:
> If you don't enter the List link, the automatic switching of timer windows will be disabled. You can still manually switch the windows.
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select **Custom** from the left dropdown menu, in the details write or copy                                          
 `data/list?&fields=DisplayName,Bib&filter=[Finished]=1&listformat=CSV`(see the picture)
![Creating list API link](https://user-images.githubusercontent.com/93376571/231474800-6a6ee8b3-abef-43d6-b3e0-c5db0518bf66.png)
 3. Click the blue icon on this API under the `Link` column on the left, then copy the link and paste it in `API Timer` tab in the `List link` text box

The result:

![Both API links created](https://user-images.githubusercontent.com/93376571/231475126-e2dcf0e8-1e55-491b-a289-f7e1293c755a.png)

---
# Using the app :computer:
## Left side menu :fleur_de_lis:
![Left side menu](https://user-images.githubusercontent.com/93376571/230731126-1956e20c-5936-432f-ac1b-63a3a9583d9b.png)
  * **Timer** - Menu for entering event name, type and start time, logo image and starting the timer
  * **API Timer** - Menu for entering API link for name and type, logo image, start time and starting the timer
  * **QR code** - Menu for setting or generating QR code
  * **Timer control** - Menu for switching between fullscreen and small timer, and closing the timer
  * **Display settings** - Choosing screen on which will timer be displayed, choosing timer layout
  * **Informations** - Informations about the app

## Timer :hourglass_flowing_sand:
![First tab, with manual settings](https://user-images.githubusercontent.com/93376571/230731257-6a90e409-3abb-4fe5-907f-8ebab6c186cd.png)
  * **Event name** - Event name that will be displayed with timer
  * **Event type** - Event type that will be dipslayed on big fullscreen clock
  * **Start time** - Define starting time for timer in `HH:MM` format
  * **Select image** - Choose image from PC to be displayed as logo, best works with rectangle logo, 
                       above the button will be showed name of the image, enables delete button (shared with **API Timer**)
  * **Delete image** - Clear the selected image (shared with **API Timer**)
  * **Open timer** - Open fullscreen timer, disables all settings and enables **Timer control**
  
## API Timer :watch:
![Second tab, with API settings](https://user-images.githubusercontent.com/93376571/230731402-ec35e965-4210-462e-8cdc-7bec217f3050.png)
  * **API Link** - Here enter the main link to get name and type of the event
  * **List Link** - Here enter the link which points to the output list, for automatic switching of the timer windows
  * **Start Time** - Define starting time for timer in `HH:MM` format
  * **Select image** - Choose image from PC to be displayed as logo, best works with rectangle logo, 
                       above the button will be showed name of the image, enables delete button (shared with **Timer**)
  * **Delete image** - Clear the selected image (shared with **Timer**)
  * **Open timer** - Open fullscreen timer, disables all settings and enables **Timer control**
  
## QR code :framed_picture:
![Third tab, with QR code settings](https://user-images.githubusercontent.com/93376571/230731543-92e096e0-58f3-49c5-abfa-60dcdab6ac20.png)
  * **Generate QR code** - Here enter link to convert to QR code
  * **Generate QR** - Generates the QR code and sets it
  * **Select QR** - Choose image from PC to be displayed as QR code, above the button will be showed name of the image, enables delete button
  * **Delete QR** - Clear the selected QR code
  * **Show QR code when minimized** - If checked, minimized timer will periodically shows QR code, based on input from the two text boxes bellow in 1 - 60 range

## Timer control :wrench:
> Tab is enabled when timer is open

![Fourth tab, timer control](https://user-images.githubusercontent.com/93376571/230731967-12806b3f-0cee-45c0-b32f-028cf60fa9e6.png)
  * **Minimize** - To manually switch from fullscreen to small timer
  * **Maximize** - To manually switch from small to fullscreen timer
  * **Close timer** - To close the timer window, timer window also close when closing the main window

## Display settings :hammer_and_wrench:
![Fifth tab, display settings](https://user-images.githubusercontent.com/93376571/230732033-6d9b08f7-11d8-4c1d-9fc5-4e82508e432c.png)
  * **Display** - List of connected displays to choose on which the timer window will be opened. Display names here **AREN'T** the same as in Windows settings
  * **Timer and image alignment** - List of available alignments 
      * Timer on top - Timer on top, clock in the middle, logo on the bottom, QR code **NOT** displayed
      * Timer on left - Timer and clock on the left side, logo and QR code on the right side
      * Timer on right - Timer and clock on the right side, logo and QR code on the left side

---
:exclamation: **Disclaimers** "RR" and "RaceResult" are trademarked by and belong to race result AG I make no claims to these or any assets belonging to race result AG and use them purely for informational purposes only.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.

[^1]: This app does't collect anything from PC or RR. I don't have signning certificate
