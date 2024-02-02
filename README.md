# About
 rEFInd theme editor is app to easy-made themes for [rEFInd](https://cecekpawon.github.io/RefindPkg/refind/index.html). 
# Install
Just download and unzip archive for your system. In Linux you always need to add "Executable" flag to "rEFInd theme editor.x86_64" file
For windows just run "rEFInd theme editor.exe"
# How to use

After run, you'll see next window:

![](https://i.imgur.com/B14jn0c.png[/img])

Default config - config for new projects
Create new theme - creating new empty project with default config. All projects shown on left side of window, press on it to open. 

### Project options
![](https://i.imgur.com/MLcOFaR.png)

1 - Target screen Height and Wight
2 - config for this theme
3 - Open image importer
4 - Preview of background and selectors
6 - Selector of OS for demo (OS appear only when you upload image for it)
7 - Selector of functions for demo (function appear only when you upload image for it)
Close project - return to first window
Project folder - open folder with project, require for  [How to install theme](https://github.com/SLywnow/refind-theme-editor/blob/master/README.md "How to install theme")
Show theme - open demo of current theme with selected OS and functions

### Image importer
![](https://imgur.com/Hkfig3m.png)

Here you can load any icon by pressing "Select" button. It automatically make copy of image and put in with right name into project's folder. You can import **only** png files. Selector look like this:
![](https://imgur.com/NpXNnCm.png)


### Config
![](https://imgur.com/IuH1Em2.png)

Here you can select size of big and small icons and enable/disable function buttons (buy you'll still can select them in demo)

### Demo
![](https://imgur.com/TyaQtVb.png)

When you open demo, it shows as full screen for better resolution. You can move here by arrows (same as in rEFInd itself). Remember, that here you'll see only OS and functions that was green in selectors
Demo needs to make pretty screenshots of your theme. Press Screenshot and when you'll hear photo sound, then screenshot done. You can see it by pressing "Open folder" button
To exit and return window it's default size press "Exit"

# How to install theme
### Auto
Auto install still in todo

### Manually
1) Open project's folder by press "Project folder" button and go to the top folder
2) Copy by command line or by any explorer theme's folder to /boot/EFI/refind/themes
3) Add line to the end to refind.conf in /boot/EFI/refind/ `include themes/theme-name/theme.conf` where theme-name is your theme's name. In case of this example it will be `include themes/test/theme.conf`
4) If you was have theme before, remove all `include` lines except new one
5) Restart pc and enjoy!