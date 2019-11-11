# AnyDeskAB
AnyDesk AddressBook

This is in a very early stage of development.
Eventually, it should provide a fully featured AddressBook system for [AnyDesk](https://anydesk.com):

At this moment, this is what's working:
* Create groups and sub-groups to organize all connection items
* Easily drag & drop items across groups
* Connect to any remote machine by double clicking any connection item
* Support custom descriptions for each connection item
* Automatically detect when connection items are added, removed or changed within AnyDesk's user interface
* Automatically update AnyDesk's configuration when changes are performed in the addressbook
* A history of up to 10 backups of AnyDesk's configuration file are created whenever the addressbook is changed
* Thumbnails display

Planned for the immediate future:
* A modern UI
* Investigate if there's anyway to detect the online state of the connection items
* ~~Investigate if there's a way to match the thumbnails to the connection items~~
* Try to preserve AnyDesk's connection items order when updating the configuration file
* Allow to manually configure the location of all required resources
* Add support to detect the location of the resources when running under non-Windows platforms
* Cleanup and simplify the code

AnyDesk is a registered trademark of philandro Software GmbH
