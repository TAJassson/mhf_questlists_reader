mhf_questlists_reader is a c# form that allows you to manage questlist files.  

I'm sorry for so much bugs.

# Todo
- Load more values such as Equipments, skills, limitation etc.

# Before you use
Below are some important tips for using this repo. Please read them at least once.  
- My reader may not read publicly shared questlist files correctly because it does not have proper structure. Also I don't recommend you to keep using it as it is because the reader doesn't support it for a technical reason. Here's how to fix it.   
Open `list_168.bin` with binary editor(eg.HxD) and check offset 01h. It's a number of quests this file loads. If it is `13`, change it to `0D`, becasue `list_168.bin` has 13 quests in total and it should be described as `0D` in hex but somehow the value is `13` of integers.

- Before you add new quest, deleting these quests is highly recommended from `list_168.bin`.  
![image](https://user-images.githubusercontent.com/89909040/161503024-52d490b4-1a5c-4ead-a501-85fad5a7457d.png)


- You should decrypt quest file itself with a tool called `ReFrontier` before you use `Add` button.
- Each questlist files are able to contain up to 42 quests.
- While you can add new quest to list, you cannot add new list file via editor. Thus, I recommend you not to delete all quests from one of list files. I mean leave at least one quest in list.  
- `Export` button creates new questlist files, and overwrites if there is one there with the same name files.  
- `Save change` button is used to save current selected quest changes.  

# Known issues
- Sometimes `Delete` button doesn't work correctly.

# Build
Don't forget to drop `Stored_Data` folder to the same path where exe created.

# Changelog

## v 1.0
- Initial release.

## v1.1
- Added montser icon names, for Diva quest.  
- Fixed Diva quest load issue.

## v1.1.1
- Fixed header type issue.

## v1.1.2
- Added panel and scrollbar to information section.

## v1.2
- Now load entire questlist folder, not each single file.  
- Added new listbox to show loaded file name.  
- Added a label at the bottom of the form that works like a log.  
- Added `Stored_Data` folder to store binary data. Don't delete this.

## v1.3
- Added `Export` button to create and export new questlist files.  
- Added `Add` button to add a new quest to current selected list.  
- Added `Delete` button to delete selected quest from list.  
- Added 2 boxed to show how many quests you've loaded.  
- Changed to form is resizable.

## v1.3.1
- Fixed few small things.

## v1.4
- Added `Save changes` button to save current selected quest changes.  
- Added textbox to show Map name.  
- Added combobox to show monsters and items name and its ID.  

## v1.4.1
- Add name suggestion to both Monster and Item name box.
- Fixed a problem where pressing the `Open` button would clear the listbox regardless of whether you actually selected the questlist folder or not (in case you accidentally pressed the button).
- Fixed a problem with the quest listbox behaving incorrectly after pressing the `Export` button.
- Fixed a problem in which the name and ID in the target field did not match when a quest was selected.

## v1.4.2
- Fixed a problem in which the quest list was not loaded all the way through in the game when the export button was pressed once, then a new quest was added, and the export button was pressed again.
