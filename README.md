mhf_questlists_reader is a c# form that allows you to read questlist files.  
Not 100% functional, but you can still use it.  
Also it may help you to understand questlist and quest file, becasue both have similar structure.
I'm planning to add an function that add new quest to questlist easily, but I can't promise that.

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
