# RR_Timer
Application to show elapsed time from entered start time, with clock, on user-selected screen. Best used with RaceResult presenter

# Features
* Show big timer with even name on result presenting screens when no one has finished
* Show smalll timer when someone has finished
* Automatic switch from big fullscreen timer to small timer
* Manual switching between big and small clock
* Event name and type can be entered manually or using RaceResult Simple API link
* When entered start time is `larger` than current time, instead of timer, clock is shown 
## Automatic switching
  When someone finishes, custom output is updated, cashed on second API link. App checks this second link every 15 seconds. When clock switches to small clock, 
  app stops checking the second link. Required custom output list and second API link.
## Choosing screen
  User can choose on which screen will the timer be shown, list of screens is refreshed every 20 seconds. If timer is open, refreshing list of screens is paused.
  
# Installation
  * Download zip file and extract
  * Double click on `setup.exe`
  * You will get `Unknown publisher` warning press install[^1]
  * That's it!

# Using the app
![First tab, with manual settings](https://user-images.githubusercontent.com/93376571/228490736-3a3cc367-aa58-4bb2-9e19-1302bea0777f.png)
  * **Event name** - Event name that will be displayed with timer
  * **Event type** - Event type that will be dipslayed on big fullscreen clock
  * **Start time** - Define starting time for timer in `HH:MM` format
  * **Open timer** - Open fullscreen timer


[^1]: This app does't collect anything from PC or RR. I don't have signning certificate
