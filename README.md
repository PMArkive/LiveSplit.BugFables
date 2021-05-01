# LiveSplit.BugFables
Autosplitter component for Bug Fables: The Everlasting Sapling for use in LiveSplit.

>Only the 1.1.0 version of the game is supported

This autosplitter supports the following features:

* Auto start when starting a new file; this works regardless of starting the file from the MYSTERY? set seed prompt or the filename prompt.
* Auto reset; only done in conjunction with auto start if the timer was already started
* Auto split with the option to disable or enable any splits from a given category. Currently, this feature is only supported for the glitchless and any% category. It is possible to automatically generate splits based on your autosplitter selections.
* Automatically split when the screen is fully faded to white before the credits marking the end of the run.

## Installation instructions
This autosplitter component is currently live in the LiveSplit's autosplitter catalogue and can be enabled within LiveSplit itself. To do so:
1. Open LiveSplit
2. Right click on LiveSplit and select "Edit Splits..."
3. Set the game to either "Bug Fables: The Everlasting Sapling" or "Bug Fables". The following message should appear above the splits: "Autosplitter for Bug Fables: The Everlasting Sapling (By Aldelaro5)"
4. Click the "Activate" button next to the message observed in step 3
5. To configure the autosplitter to your preferences, click the "Settings" button next to "Activate". The "Website" button opens this readme in your browser.

## Usage instructions

This autosplitter can operate in 2 modes:

* Start and end only: Only check for the start and end of the run.
* Select Splits: Enables splits used in the glitchless or any% category. Also allow you to set the current splits to the autosplitter selection.

 CAUTION: Using "Set Current Splits" will clear your current splits including their name and time. You may need to remove the splits and readd them in the layout for the new splits to appear.

The splits's name are designed to work best using the "Subsplits" layout as opposed to the "Splits" layout since they are grouped per chapter.

NOTE: This autosplitter will not work with a modified mono.dll (for use with DnSpy) or when the BepInEx mod loader is installed.

## Additional resources
If you have any questions or issues to report, you can do so in the [Bug Fables speedrunning discord server](https://discord.gg/tHAQWpF).

For any information and resources regarding speedrunning Bug Fables, you can consult the [Speedrun.com page of Bug Fables](https://www.speedrun.com/bug_fables)
