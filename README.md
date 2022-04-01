mhf_questlists_reader is a c# form that allows you to read questlist files.  
Not 100% functional, but you can still use it.  
Also it may help you to understand questlist and quest file, becasue both have similar structure.
I'm planning to add an function that add new quest to questlist easily, but I can't promise that.

# Before you use
Below are some important tips for using this repo. Please read them at least once.  
- My reader may not read publicly shared questlist files correctly because it does not have the proper structure. Also I don't recommend you to keep using it as it is because the reader doesn't support it for a technical reason. Here's a how to fix it.   
Open `list_168.bin` with binary editor(eg.HxD) and check offset 01h. It's a number of quests this file loads. If it is `13`, change it to `0D`, becasue `list_168.bin` has 13 quests in total and it should be described as `0D` in hex but somehow the value is `13` of integers.
# Known issues
- Sometimes unable to load Sub A text correctly

# Changelog

## v 1.0
Initial release.

## v1.1
Added montser icon names, for Diva quest.  
Fixed Diva quest load issue.

## v1.1.1
Fixed header type issue.

## v1.1.2
Added panel and scrollbar to information section.

## v1.2
Now load entire questlist folder, not each single file.  
Added new listbox to show loaded file name.  
Added a label at the bottom of the form that works like a log.  
Added `Stored_Data` folder to store binary data. Don't delete this.
