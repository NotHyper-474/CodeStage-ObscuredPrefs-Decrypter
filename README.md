# Overview
Simple decrypter, encrypter and viewer for CodeStage's AntiCheat toolkit.  
Primarily made for [Slendytubbies 3](https://zeoworks.com/games/Slendytubbies3.html)
but can also work for other games, as long as they use [ACTk](https://codestage.net/uas/actk/)

# Troubleshooting


### Where is the game's registry folder?
It's at HKEY_CURRENT_USER\SOFTWARE\\(name of company)\\(name of game)  
If you're testing using a Unity project then it will be at HKEY_CURRENT_USER\SOFTWARE\Unity\UnityEditor\\(name of company)\\(name of game)

### It works but it's just giving me gibberish (or just some keys are gibberish)
The game/keys might be using (or are from) an older version of ACTk.
The system is still the same but the key will always be "e806f6"  
~Also make sure you placed the cryptokey itself and not its base64 version.~  
The Viewer can automatically find the key.

### It decrypts the key but shows "SAVES_TAMPERED"
Normally that happens when ACTk's system cannot
decrypt the value (be it not being able to decode the
base64 or some bytes missing or even some bytes being
***tampered*** with.)
