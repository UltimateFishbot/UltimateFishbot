UltimateFishbot
===============

A simple World of Warcraft fishbot written in C#.

Introduction

I started this project during my freshman year of college…many, many years ago.
It’s been years since I’ve played Wow and I no longer have the amounts of free
time I used to.  I’ve revised the code a few times since, but I can’t pretend 
it’s a shining example of anything; still, it remains a popular project and I 
hope someone who enjoys the game or enjoys the code will come along and get 
some value from this.

Coding Overview

This is originaly written in VB.NET (I know, I know….). It has been now ported
in C#. You can get a free copy of Visual Studio Express.

The general idea is that the bot works by ‘listening’ to the output from the
game (whereas most bots at the time were reading pixels and trying to find the 
bobber, or reading the game’s memory (which means updating the offsets after 
each patch).  This works with the CoreAudio DLL file – I didn’t write it and 
I don’t believe it works with Windows XP.

The bot uses standard win32 API calls to move the mouse and send keystrokes.  
I’ve never had any problems, but I’ve gotten reports from users saying it 
doesn’t work on their machine.  

It starts by sending the ‘cast’ key, then moving the cursor systematically and 
using the GetCursorInfo api call to detect a change in the icon.  We assume 
that change is the fish bobber.  Then it monitors the sound output (via 
CoreAudio) until there is a change and we assume that is the splash of a fish.  
Then the bot sends the click mouse event.  Rinse.  Repeat.

There are quite a few ‘nice-to-haves’ I never got around to doing….adding 
lures, improving the scanning, ironing out the hearth home functionality…
maybe someone will get a chance.

Thanks to everyone who supported the project and thanks to anyone reading this
for your interest.

Updated for Mists of Pandaria
=============================

Updated by justanotherprogrammer to support MoP items such as the Ancient
Pandaren Fishing Charm and the Angler's Fishing Raft.  Also added support for
people who don't use Auto-loot.

Updated for Warlords of Draenor
===============================

Updated by Henrik Hermansen to support WoD bait.

Updated in C#
===============================

Updated by Pierre Ghiot (Mograine) in C#.

Happy Fishing!

Gorden
ultimatefishbot@gmail.com
www.fishbot.net
