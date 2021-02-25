Preface
=======

Hollow Knight Modding generally requires changing the game's code using an assembly editor such as dnSpy.   While this works, it means that each time the game releases a new version, all mods had to be recoded by hand.  The Hollow Knight API aims to remove this requirement by exposing hooks to perform most needed operations without having to rewrite decompiled code.

We use the MonoMod patcher to greatly reduce the effort in patching the assembly.  Go check it out! https://github.com/0x0ade/MonoMod

Building The API
============================
Building the API is fairly straightforward.

1. Clone this!
2. Go to one of the directories listed below and copy it's contents to the `Vanilla` folder in this repository. (Create the Vanilla folder if it does not exist.)
  * Windows: `%HollowKnightGameInstallPath%/hollow_knight_Data/Managed/`
  * Linux: `~/.steam/steam/steamapps/common/Hollow Knight/hollow_knight_Data/Managed/`
  * Mac: `~/Library/Application Support/Steam/steamapps/common/Hollow Knight/hollow_knight.app/hollow_knight_Data/Managed/`
3. Open the solution in Visual Studio or Rider (You can also just use msbuild/xbuild)
4. Set the build configuration to Debug.
5. The patched assembly should be in `RepoPath/OutputFinal/hollow_knight_Data/Managed/` (There is also a zip file in `RepoPath/ModdingAPI.zip` ready to upload to Google Drive)
6. Copy `Assembly-CSharp.dll` to `%HollowKnightGameInstallPath%/hollow_knight_Data/Managed/`
To use msbuild:
1. Navigate to the root of the project
2. Run `msbuild -p:Configuration=Debug /restore`

Contributors
=======
Original Authors:  
MyEyes / Firzen  
Seresharp  

Contributors:  
iamwyza  
esipode  
Kerr1291  
fifty-six  
natis1  
Ayugradow  
Katie  
SFGrenade
