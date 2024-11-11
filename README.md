# missing-script-finder-unity3d

As your unity3d project grows big. You add, remove, change classes frquently.
Sometimes this results in dead residuals on your game objects along side other components.

You can skim through some objects but if your scene contains hundreds of components then thats a nightmarish situation. 

I wrote this editor class to handle such recent situation that happened to me. It runs through your scene & prefabs and find missing components on them.
It generates a list and give you an ability to select the items directly.

I hope this tool helps my fellow developers in their future projects.

HOW TO USE:
Create a folder named "Editor" anwyere in your assets. Place MissingScriptFinder class there. 
Go to tools menu and select "Missing Script Finder"
