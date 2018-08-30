UltimateFishbot
===============
A simple World of Warcraft fishbot written in C#.

Introduction 

I started this project during my freshman year of college… Many, many years ago.
It’s been years since I’ve played Wow and I no longer have the amounts of free
time I used to.  I’ve revised the code a few times since, but I can’t pretend 
it’s a shining example of anything; still, it remains a popular project and I 
hope someone who enjoys the game or enjoys the code will come along and get 
some value from this.

Coding Overview

This is originaly written in VB.NET (I know, I know….). It has been now ported
to C#. You can get a free copy of Visual Studio Community.

The general idea is that the bot works by ‘listening’ to the output from the
game (whereas most bots at the time were reading pixels and trying to find the 
bobber, or reading the game’s memory - which means updating the offsets after 
each patch). This works with the CoreAudio DLL file – I didn’t write it and 
I don’t believe it works with Windows XP.

The bot uses standard win32 API calls to move the mouse and send keystrokes.  
I’ve never had any problems, but I’ve gotten reports from users saying it 
doesn’t work on their machine.  

It starts by sending the ‘cast’ key, then moving the cursor systematically and 
using the GetCursorInfo api call to detect a change in the icon. We assume that 
that change is the fish bobber. Then it monitors the sound output (via 
CoreAudio) until there is a change above a threshold and we assume that this is the splash of a fish.  
Then the bot sends the click mouse event. Rinse. Repeat.

There are quite a few ‘nice-to-haves’ I never got around to doing… Adding 
lures, improving the scanning, ironing out the hearth home functionality…
Maybe someone will get a chance.

Thanks to everyone who supported the project and thanks to anyone reading this
for your interest.

Updated for Mists of Pandaria & Warlords of Draenor
=============================

Updated by justanotherprogrammer to support MoP items such as the Ancient
Pandaren Fishing Charm and the Angler's Fishing Raft.  Also added support for
people who don't use Auto-loot.

Updated by Henrik Hermansen to support WoD bait.

Updated in C#
===============================

Updated by Pierre Ghiot (Mograine) in C#.

Updated by daniel-widrick & Szabka
===============================
-Hearing optimisation.

-Bobber finding and Hearing is put in parallel threads.

-Bobber finding searches the differences between "before cast" and "after cast" screenshots, than previous bobber positions ordered by occurence.

-If bobber is still not found when fish is heard, it searches 2 seconds more for bobber, so that it still has a possibility to catch fish.

-Bobber recheck on hook, so if bobber moves a little it will find the fish again.

-Has log file for debugging purposes.

-Can capture fishing cursor with hotkey for more precision bobber finding.

-Hungarian translation.

-Bulgarian translation.

-Wow window handle searched when starts fishing(and not when fishbot starts).

-Hotkey actions disabled in settings screen.

-Stop fishing after 10 consecutive fish failures.

Happy Fishing!

Gorden
ultimatefishbot@gmail.com
www.fishbot.net
