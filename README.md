# SoundCollector
XNA game where you catching particles for points.

When particles hit the shield (center of screen) game will decrement shield value.

Shield is down = Game over

Score multiplier = Shield value / 10

Control:
- Particles are collected by mouse (fast movement attracts particles)
- Left mouse button (press) turns on "Bonus Lux"
- Right mouse button (hold) turns on "Bonus Shield"
- Space skips current song
- Esc opens menu

Count and size of particles are generated by selected music.

Songs are loaded from Windows Media player library (is necessary to have some songs in the library).

Project can be opened in Visual Studio 
(for Visual Studio 2015 is necessary to install xna support: https://mxa.codeplex.com/releases/view/618279) 

Game is written in XNA framework (C# .NET) and uses DPSF library (http://www.xnaparticles.com/) 

Current version 1.0.0

**Known issues:**
- ~~Game is not playing next song after current song ends~~ (fixed in 0.9.9)
- ~~Texts are sometimes too long (overflowing menu)~~ (fixed in 1.0.0)

**Changes:**
- Version 0.9.9
  - Fixed: Game is not playing next song after current song ends
- Version 1.0.0
  - Fixed: Texts are sometimes too long (overflowing menu)
  - Back button is on opposite side of menu

![alt tag](http://www.itnetwork.cz/images/53403567abf3b_image_0)
