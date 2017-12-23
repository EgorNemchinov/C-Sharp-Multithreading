# Parallel programming on C#

In most cases running _Program.cs_ prints comparison of simple and parallel version.

### [QuickSort](https://github.com/ImmortalTurtle/C-Sharp-Multithreading/tree/master/QuickSort)
Recursive QuickSort is easily parallelized since each time we split range into two non-intersecting ones.

### [Prime Numbers](https://github.com/ImmortalTurtle/C-Sharp-Multithreading/tree/master/PrimeNumbers)
To get boolean mask of prime numbers up to _X_:
 - Split range _(0, X)_ into _N_ parts, where _N_ - amount of threads to be run.
 - Run _N_ threads, each one checks all the numbers in it's range one by one.
 
The speed boost depends on _N_ and the way you split the range into _N_ parts.

### [Folder MD5 Hash](https://github.com/ImmortalTurtle/C-Sharp-Multithreading/tree/master/MD5Hash)
_Define_: MD5(_folder_) = MD5(_folderName_ + _{concatenation of all MD5 hashes of files in the folder}_)

Hashes of files in the folder are calculated parallel.

### [RedBlackTree](https://github.com/ImmortalTurtle/C-Sharp-Multithreading/tree/master/RedBlackTree)
Global queue for operations.
Many consecutive searches(_Find_) in the tree are run parallel. _Remove_ and _Insert_ operations are run one-by-one.

Unit tests for checking validity of RBTree itself and for checking whether parallel RBTree works the same.

Increase in speed is achieved when _Insert_ & _Remove_ operations are very rare.

### [WebRuler](https://github.com/ImmortalTurtle/C-Sharp-Multithreading/tree/master/WebRuler)
Program prints the length of web page at given URL, then does the same for all the URLs contained in the page, i.e. recursively run for all the links in html source of initial page.

Program is stopped once it reaches certain depth in recursiveness.

_Async_ allows not to actively wait for the page to be downloaded.

### [Atomic Snapshots](https://github.com/ImmortalTurtle/C-Sharp-Multithreading/tree/master/Snapshots)
Implemented Wait-free algorithm for taking atomic snapshots of shared memory. 
Logging to check validity.
