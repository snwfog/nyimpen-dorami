Chao Yang
5682061

Note: The project is too large, you can find it on Github at:
https://github.com/snwfog/nyimpen-dorami

Note: I have note change the name of the project, and used Demo1 as the template, so the project is called Demo1, while indeed is my assignment 2 submission.

I have very carefully crafted every sprite, sound files, and fine tune the code and refactor most of the code base as well.

I have included almost all game play that was asked, and much more. Those that I have not include don't means that I didn't have time, or could not implement it, but rather I feel it is too easy, or not worth for a fun game play. 

I have include a lot of player information, such as the elapsed time, 
current score, the amount of bullets, and the "ice cream" timer.

I have also included dialogue system, bullet system, moving enemies with limited AI, 8 directions of movement for players, NPCs, and bullets.

I have fine tune most of the sprite, which is one of the step that took most of the time.

My code are object oriented as much as possible. In the beginning I was relying mostly on constructor for object creation, but later one I realize a static factory method results in a much cleaner code. I also used composition a lot.

Every objects on the screen defines its own hit box, and the collisions are handled automatically.

Bug/glitches:
- The bullet stick at the perimeter of the screen, because I did not increase the window size. I feel it is a minor glitch.
