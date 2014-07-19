FreeEverything
==============

FreeEverything is a utility to scan file/folder in hard disk and delete them(using everything SDK).


## Installation

Download the [FreeEverything.zip] (https://github.com/fresky/FreeEverything/blob/master/FreeEverything.zip) and unzip, then run the FreeEverything.exe.


## Usage
1. Click the "New" and "Delete" button to create or delete the filter.
1. Select each filter, set the name, search regular expression, include/exclude folder, and specify search the file or search the directory.
1. Click the "Scan" button to search based on the checked filters.
1. Click the "Calculate Size" button to calculate how much space taken by the checked search result.
1. Click the "Free" button to free the checked search result.

## Snapshot

![Free Everything Snapshot](https://raw.githubusercontent.com/fresky/FreeEverything/master/FreeEverything.png)

## Requirements

Please install [.NET Framework 4.5](http://msdn.microsoft.com/library/vstudio/5a4x27ek)

## Credits

FreeEverything used the [Everything SDK](http://support.voidtools.com/everything/Main_Page), [Everything](http://www.voidtools.com/) is a super fast desktop search engine for Windows.

## License

FreeEverything is released under the MIT License. See the bundled LICENSE file for details.

## Chang Log

1. 07/19/2014	using async
1. 07/12/2013	add progress bar, spin waiter, prepare for the next async version
1. 07/02/2013	update to .net framework 4.5 and remove the dependency on mvvmlight
1. 08/24/2012	initial version
