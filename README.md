# Libris
*from Latin "Ex libris", lit. "From the library of [person]"*    
  
A .NET Core and Common Language Runtime implementation of the Minecraft multiplayer server.   
  
Libris aims to feature similar levels of plugin support comparable to major Minecraft server technologies, like Spigot, Paper, and the Bukkit family. Libris supports .NET Standard to allow plugins written in your .NET language of choice. (C#, F#, Visual Basic)  
Libris also has a [Discord server](https://discord.gg/RsRps9M) you can join, for help or discussion (or memes).

### Goals
The lead developer of this project is [Abyssal](http://github.com/abyssal).  
The project only aims to support the latest mainstream release of Minecraft, which is 1.14.4 (protocol version 498) as of July 27th, 2019.  
**Completed**
- [x] Broadcast server on 25565 and send MOTD, player count, and player data
- [x] Respond to client latency test
  
**Near**
- [ ] Load an existing Minecraft map and spawn players in the map
  
**Future**
  
**Far future**
- [ ] 1-1 feature parity with the Notchian (vanilla) server
- [ ] Minecraft map generation

### Acknowledgements
- Without [wiki.vg](https://wiki.vg/Main_Page), this project would not have been possible.
- Libris is inspired by Java wrappers and patches for the vanilla client, like [SpigotMC](https://www.spigotmc.org/), [Bukkit](https://bukkit.org/), and [Paper](https://papermc.io/). However, it is important to emphasis that Libris does not wrap the vanilla server, but rather implements the protocol that it uses, bypassing the Java Virtual Machine entirely.

### Copyright
Libris application and server Copyright (c) 2019 abyssal512, licensed under the MIT License  
Minecraft, the Minecraft client, the Minecraft server, and all images, logos, branding and other media, are copyright of Mojang A.B.
