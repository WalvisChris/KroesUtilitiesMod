# Kroes Utilities  
Some quality of life additions. Modify using the config.  

- PriceGun: automatically set price to 2x market price.  
- Franchise points now show a progress bar.    
- Throwable boxes:  
    * can hit NPCs and Players.  
    * can be thrown against recycler and trash bin (works with perks).  
- Euro currency symbol compatibility for price displays.  
- Third person camera using the mouse scroll wheel.  
- chat commands (TESTING!).  

## Chat Commands  
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

# Todo  
- Vehicle driving.  
- Custom prefab spawning across all clients.  