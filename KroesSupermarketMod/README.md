# Kroes Utilities  
Some quality of life additions. Modify using the config.  

- automatically sets price gun to 2x  
- visual franchise points progress bar  
- custom npc voice lines when hit with broom  
- throwable boxes (can hit NPCs and Players)  
- Euro currency sign compatibility  
- chat commands (TESTING)  

## Chat Commands  
### /npc  
*[client-only]*  
`/npc` resets npc animators.  
`/npc random` starts a random animation clip.  
`/npc <NUMBER>` starts a certain animation clip (0 through 41).  

### /weather  
*[being tested...]*  
`/weather <NUMBER>` sets the weather (0 through 5).  

### /notif  
*[client-only]*  
`/notif1 <TEXT>` create a canvas notification for yourself.  
`/notif2 <TEXT>` create an important notification for yourself.  

# Todo  
- Box Throwing:
    * test box throw physics for clients.  
    * sync box throw physics between clients.  
    * test box throw hitting NPCs and Players for clients.  
- Chat Commands:
    * `/weather`:
        - test client and server requests.  