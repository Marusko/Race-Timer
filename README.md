# RR_Timer
Application to show elapsed time from entered start time, with clock, on user-selected screen. Best used with RaceResult presenter as this app can automatically make the timer smaller, so it can fit over the presenter window without pressing `F11` on the presenter window.
### Small timer over the RR presenter after the start
![image](https://user-images.githubusercontent.com/93376571/228545144-ff334b8e-0398-4462-b26d-dd55c3ca1d69.png)
### Fullscreen timer after the start
![image](https://user-images.githubusercontent.com/93376571/228545556-b42deb23-0bb5-4f5d-a0bc-56c5bbab02f0.png)

---
# Features
* Show big timer with even name on result presenting screens when no one has finished
* Show smalll timer when someone has finished
* Automatic switch from big fullscreen timer to small timer
* Manual switching between big and small clock
* Event name and type can be entered manually or using RaceResult Simple API link
* When entered start time is `larger` than current time, instead of timer, clock is shown 
## Automatic switching
  When someone finishes the race, custom output in RR is updated, cashed on second API link. App checks this second link every 15 seconds. When clock switches to small clock, app stops checking the second link. Required custom output list in RR and second API link.
## Choosing screen
  You can choose on which screen will the timer be shown, list of screens is refreshed every 20 seconds. If timer is open, refreshing list of screens is paused.
  
---  
# Dependencies
App requires [.NET7](https://dotnet.microsoft.com/en-us/download) to be installed. Most probably you already have it. If not, it will be downloaded and installed when installing the RR_Timer
  
# Installation
  1. Download [zip file](https://github.com/Marusko/RaceResult_UI_Timer/releases) and extract
  2. Double click on `setup.exe`
  3. You will get `Unknown publisher` warning, press install[^1]
  4. That's it!
  
---
# Preparation
## Using the Timer
If you don't time the race with RaceResult, or don't want to create links simply use the `Timer` tab to setup the timer. Automatic switching of timer windows is disabled. You can still manually switch the windows.
## Using the API Timer
> Only works with RaceResult Simple API
### Main API link
 1. Open the event in RR website > Main Window > Access Rights/Simple API > Simple API 
 2. Select settings from the left dropdown menu, in the details write `EventName,EventType`(see the picture)
![Creating main API link](https://user-images.githubusercontent.com/93376571/228536613-721357e7-d342-403c-9e85-fb77a9ba8633.png)
 3. Click the blue icon on this API under the `Link` column on the left, then copy the link and paste it in `API Timer` tab in the `API link` text box
### List API link
> If you don't enter the List link, the automatic switching of timer windows will be disabled. You can still manually switch the windows.
 1. Open the event in RR website > Output
 2. Copy `Overall results` list and rename it to `APIlist` to not confusing it with other lists
 3. Delete all columns except `Name` and `Bib`
 4. Delete all filters and add **Field**>`Finished`, **Operator**>`=`, **Value**>`1`
![image](https://user-images.githubusercontent.com/93376571/228540036-2f3f8100-1d2b-4020-81ad-52e421be1b04.png)
 5. Go to Main Window > Access Rights/Simple API > Simple API 
 6. Create another API **Type**>`List`, **Details**>`APIlist`(the list we created before), click on `TEXT` and select `JSON`
![Both API links created](https://user-images.githubusercontent.com/93376571/228541684-c9a911d9-743f-4c72-ab33-cae9c19d63ab.png)
  7. Click the blue icon on this API under the `Link` column on the left, then copy the link and paste it in `API Timer` tab in the `List link` text box
  
---
# Using the app
## Left side menu
![Left side menu](https://user-images.githubusercontent.com/93376571/228530691-c5b4fb90-9af8-4f51-9b6f-24476a0c22fe.png)
  * **Timer** - Menu for entering event name, type and start time, and starting the timer
  * **API Timer** - Menu for entering API link for name and type, start time and starting the timer
  * **Timer control** - Menu for switching between fullscreen and small timer, and closing the timer
  * **Settings** - Choosing screen on which will timer be displayed
  * **Informations** - Informations about the app

## Timer
![First tab, with manual settings](https://user-images.githubusercontent.com/93376571/228530375-f73fefdc-867d-4a9b-b7bf-d4de08a8e503.png)
  * **Event name** - Event name that will be displayed with timer
  * **Event type** - Event type that will be dipslayed on big fullscreen clock
  * **Start time** - Define starting time for timer in `HH:MM` format
  * **Open timer** - Open fullscreen timer
  
## API Timer
![image](https://user-images.githubusercontent.com/93376571/228532526-fbd9876b-91a6-4166-a15e-f3a022af403c.png)
  * **API Link** - Here enter the main link to get name and type of the event
  * **List Link** - Here enter the link which points to the output list, for automatic switching timer windows
  * **Start Time** - Define starting time for timer in `HH:MM` format
  * **Open timer** - Open fullscreen timer 

## Timer control
![image](https://user-images.githubusercontent.com/93376571/228533641-dd14037b-f202-4882-acad-71689e410eb2.png)
  * **Minimize** - To manually switch from fullscreen to small timer
  * **Maximize** - To manually switch from small to fullscreen timer
  * **Close timer** - To close the timer window, timer window also close when closing the main window

## Setting
![image](https://user-images.githubusercontent.com/93376571/228534331-986c8708-b147-4177-9cdc-33a0cc21fdd1.png)
  * **Display** - List of connected displays to choose on which the timer window will be opened. Display names here **AREN'T** the same as in Windows settings

---
**Disclaimers** "RR" and "RaceResult" are trademarked by and belong to race result AG I make no claims to these or any assets belonging to race result AG and use them purely for informational purposes only.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.

[^1]: This app does't collect anything from PC or RR. I don't have signning certificate
