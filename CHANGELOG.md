## Batteries [1.1.1]
- Changed batteries to no longer be conductive as it made the stormy weather system throw errors

## Batteries [1.1.0]
- Batteries now spawn outside of the scrap pool and work just like keys
	- Added config value batteryMaxSpawns which determines the maximum number of batteries that can spawn per moon
	- The spawning system currently generates a random number from 1-150 batteryMaxSpawns times and if batteryRarity is greater or equal it spawns
	- Feel free to let me know if this should be tweaked or given more configurability on Discord
- Revamped the entire use battery system to support controller and VR thanks to DaXcess
	- Removed the useBatteyKeybind config value
	- Added the useBatteryKeybinds config value which is a string instead of an integer
	- **Note that any custom keybind you had set before must be manually transferred to the new config value**
- Fixed batteries purchased from the shop having a scrap value
- Fixed a bug where you could not turn on an empty flashlight after using a battery on it

## Batteries [1.0.5]
- Natural spawning of batteries can now be disabled by setting the batteryRarity config value to 0
- Batteries can now be configured to appear in the shop with a price as defined by the batteryShopValue config value
- The charging coil can now be disabled using the disableChargingCoil config value to make batteries the only charge source

## Batteries [1.0.4]
- Fixed batteries not saving, despawning, selling for any value, and other shenanigans due to not being marked as scrap
	- I originally wanted for batteries to be like keys, but making it function as a full scrap seems to fix several issues
	- Feel free to let me know on Discord if you think it should be like a key or full scrap, I would like it to function however the community sees fit
- Fixed the use battery tool tip not being added/removed properly with multiple players

## Batteries [1.0.3]
- Made the use battery sound effect Mono instead of Stereo
- Changed audio mixing system to use LethalLib's mixing fix method
- Fixed battery sounds being global to all players with no 3D dropoff

## Batteries [1.0.2]
- Changed the audio source of the use battery sound to the audio source of the item you are holding

## Batteries [1.0.1]
- Fixed the mod not checking for empty item slots on a battery use
- Fixed being able to use a battery while using the charging station

## Batteries [1.0.0]
- Initial release
