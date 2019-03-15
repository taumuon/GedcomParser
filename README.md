# GedcomParser

This repository contains a GEDCOM parser, implemented in C#.
There are two versions of the implementation, one using standard .NET string operations, and the other using the newer Span<T>.

This is to compare performance, as blogged: http://www.taumuon.co.uk/2019/03/reducing-allocations-in-parsing-gedcom-files-using-span.html
