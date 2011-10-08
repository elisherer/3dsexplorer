
Saving a SAVE Flash progress:
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
+ MemoryMap, Journal & Blockmap are in the context.
+ CRC16 of the blockmap is possible through CRC16.cs

Image Hash: (0x10 @ 0x0000)
+ Changes from one save to another.
- Algorithm is unknown.

DISA Hash: (0x20 @ 0x006C into the DISA block)
+ Changes from one save to another.
+ Understood only for "only SAVE-Partition" files (files w/o DATA Partition)
+ Algorithm - SHA256
- Hash of files with DATA partitions isn't understood. (didn't find with brute force)

Partition Hash: (0x20 @ 0x010C into Parition entry)
+ Changes from one save to another.
? Algorithm - Probably SHA256 becuase it's 32 Bytes.
- Isn't understood. (didn't find with brute force)

Partition Hash Table Header: (0x40 @ 0x0000 into the partition)
+ Changes from one save to another.
- Algorithm is unknown.
? Probably doesn't matter.

Algorithm Proposal:
^^^^^^^^^^^^^^^^^^^
Required stuff:
* Image binary (byte array)
* Key Binary (byte array - 512 bytes)
* Blockmap + Journal + MemoryMap
* Disa & Partition table

After changing a file in the partition.
1. Go over the partitions and rehash the hashtable entries (that aren't 0)
2. (Unknwon) Then make the 'Partition Hash Table Header'
3. (Unknown) Go over the Partition table and and make the proper hash (for SAVE & DATA).
4. (Partly) Hash the proper partition table into the DISA struct.
5. Rearrange the 0x1000 blocks according to the MemoryMap.
6. Chechsum every 0x1000 block according to 3DBrew.
7. XOR the whole thing with the key-file.