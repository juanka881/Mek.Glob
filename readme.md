# Mek.Glob

a [glob](http://en.wikipedia.org/wiki/Glob_(programming) "glob") expansion library for c#. Allows to take a text pattern and get back a list of file system paths that match the expression.  

# Getting Started

TODO

# Usage

TODO

# Features

All matches are expanded against the file system. the file system is traverse and any paths that match the pattern are returned. 

 - `*` 	matches zero or more chars

 - `?` 	matches one char
  
 - `**`	recursively walks the file system and returns all directory paths  

# Notes

### Windows file paths
on windows the directory separator is the `\` char, however glob patterns use `\` char as a escape char therefore all patterns should use the `/` char as the directory separator even on windows. 

### Recursion with `**`
in some cases you may encounter a **unauthorized access exception** when using the `**` operator as it tries to traverse file system paths that the user does cannot access. Currently I have no fix and I'm looking into options to resolve this issue and just ignore these paths.  

# License
This library is released under the **MIT** license.
[http://opensource.org/licenses/MIT](http://opensource.org/licenses/MIT)