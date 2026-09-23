# Kroes Utilities  
Some quality of life additions. Everything can be changed using the config file.  

### General  
- **Double Price Gun:** the price gun price is set to 200% of market price by default.  
- **Franchise Progress Bar:** a progress bar on the big screen shows how close you are to obtaining the next franchise point.  
- **Euro Currency Symbol:** dollar signs get converted to euros, and points to comma's.  

### Third Person Camera  
- Use **Mouse Scroll Wheel** to change between first- and third person camera.  
- Third person camera **distance** can be changed using the config file.  

### Mini Transport Vehicle  
- Can hold up to **6** more boxes (stacked vertically on top). Set the exact amount using the config file.  
- Driving logic like **max speed, max backward speed, max acceleration, max deceleration, max rotation rate** can be modified using the config file.  

### Experimental
- **Custom Npc Hit Notifications:** changes NPCs message when they are hit. Meant as an inside joke for my friends.  
- **Throwable Boxes:** boxes are thrown instead of dropped, collide with NPCs, players and even recyclers! currently inconsistent.  
- **Chat Commands:** run commands from the chat using "/". See more details below.  

# Chat Commands  
### /npc  
*[client-only]*  
`/npc` resets npc animators.  
`/npc random` starts a random animation clip.  
`/npc <NUMBER>` starts a certain animation clip (0 through 41).  

### /weather  
*[server-only]*  
`/weather <NUMBER>` sets the weather (0 through 5).  

### /notify  
*[client-only]*  
`/notify <NUMBER> <TEXT>` create a canvas notification for yourself (number 0 or 1).  

### /spawn  
*[everyone]*  
`/spawn <TEXT>` spawns a item or prop. Choose ladder, tv, train or wagon. No despawning implemented yet!  