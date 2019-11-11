# AnyDeskAB
AnyDesk AddressBook

This is in a very early stage of development.
Eventually, it should provide a fully featured AddressBook system for [AnyDesk](https://anydesk.com):

At this moment, this is what's working:
* Create groups and sub-groups to organize all connection items
* Easily drag & drop items across groups
* Connect to any remote machine by double clicking a connection item
* Support custom descriptions for each connection item
* Automatically detect when connection items are added, removed or changed from AnyDesk's user interface
* Automatically update AnyDesk's configuration when changes are made in the addressbook
* A backup of up to 10 configuration files is kept whenever changes are made from within AnyDeskAB
* Thumbnails display for each connection item (if available)

Planned for the immediate future:
* A modern UI
* Investigate if there's anyway to detect the online state of the connection items
* ~~Investigate if there's a way to match the thumbnails to the connection items~~
* Try to preserve AnyDesk's connection items order when updating the configuration file
* Allow to manually configure the location of all required resources
* Add support to detect the location of the resources when running under non-Windows platforms
* Cleanup and simplify the code

[AnyDesk](https://anydesk.com/) is a registered trademark of [philandro Software GmbH](https://philandro.com/)
