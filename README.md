# Overview
Simple decrypter, encrypter and viewer (see reg-viewer branch and 1.x.x releases) for CodeStage [ACTk](https://codestage.net/uas/actk/)'s ObscuredPrefs.  
# Troubleshooting (mostly for v0.1.0)
### Where do I find the cryptokey?
The cryptokey will **always** (unless it's using older ACTk, in that case use e806f6 as the key) be stored inside the key named as the MD2 hash of "ElonShotMarsWithACar" (9978e9f39c218d674463dab9dc728bd6)
inside the game's registry. There it will be in base64 and can be easily decoded.  
The cryptokey is composed of randomly-generated characters so don't be weirded out when you see it.

### Where is the game's registry folder?
It's at HKEY_CURRENT_USER\SOFTWARE\\(name of company)\\(name of game)  
If you're testing using a Unity project then it will be at HKEY_CURRENT_USER\SOFTWARE\Unity\UnityEditor\\(name of company)\\(name of game)

### It works but it's just giving me gibberish
The game might be using an older version of ACTk.
The system is still the same but the key will always be "e806f6" (unless the developer manually changed it).
Also make sure you placed the cryptokey itself and not its base64 version.

### It's saying the base64 code is invalid
Make sure you didn't put the whole key name (e.g "AAAAAA==\_h542588110")
or you didn't include an invalid character (like the dot at the end of a value,
it is just a trailing character and isn't part of the base64 code)